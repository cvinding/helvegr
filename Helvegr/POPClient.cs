using System;
using System.Collections.Generic;
using System.Net.Security;
using DnsClient;

namespace Helvegr {

    public class POPClient : TCPClient {

        private readonly Dictionary<string, string> arguments;
        private readonly Flags flags;
        
        public POPClient(Flags flags, int portNumber, string clientCertificatePath, QueryType type) : base(flags.arguments["Username"].Split("@")[1], portNumber, clientCertificatePath, type) {
            this.flags = flags;
            this.arguments = flags.arguments;
        }

        protected override void StartClient(SslStream stream) {

            string isInteractive;

            // Check if --interactive are set
            if(flags.arguments.TryGetValue("--interactive", out isInteractive)) {
                Interactive(stream);
                stream.Close();
                Environment.Exit(0);
            }

            // Check if start message is sent
            if (!StreamRead(stream).Contains("+OK HPOP")) {
                Console.WriteLine("Missing server greeting");
                Environment.Exit(1);
            }

            // Authenticate 
            StreamWrite(stream, "APOP " + flags.arguments["Username"] + " " + flags.arguments["Password"]);

            // Check if successful
            if(!StreamRead(stream).Contains("+OK maildrop")) {
                Console.WriteLine("Authentication failed");
                flags.ShowHelpMessage(flags.command, 1);
            }

            string argumentValue;

            string startIndex, endIndex;

            string memberName;

            string action = "";

            //Find the action operation
            foreach (KeyValuePair<string, Flag> pair in flags.flags) {

                memberName = pair.Key.Substring(2, 1).ToUpper() + pair.Key.Substring(3);

                if (pair.Value.isAction && flags.arguments.TryGetValue(memberName, out argumentValue)) {

                    if (memberName == "List") {
                        if (flags.arguments.TryGetValue("Start", out startIndex)) {
                            endIndex = flags.arguments["End"];

                            action = memberName.Substring(0, 4).ToUpper() + " " + argumentValue + " " + startIndex + " " + endIndex;
                            break;
                        } else {
                            action = memberName.Substring(0, 4).ToUpper() + " " + argumentValue;
                            break;
                        }

                    } else {
                        action = memberName.Substring(0, 4).ToUpper() + " " + argumentValue;
                        break;
                    }
                }

            }
            // Execute action operation
            StreamWrite(stream, action);

            // Read response
            string output = StreamRead(stream);

            // Check if error
            if (output.Contains("-ERR")) {
                Console.Write(output);
                StreamWrite(stream, "QUIT");
                Environment.Exit(1);
            }

            // Read all lines until +DONE
            while (!output.Contains("+DONE")) {
                output += StreamRead(stream);
            }

            Console.Write(output);

            // Quit
            StreamWrite(stream, "QUIT");

        }

        //Interactive() is a method for the Interactive mode, used for showcasing the client and server
        private void Interactive(SslStream stream) {

            string input, output;
            bool breakEarly = false;

            Console.Write(StreamRead(stream));

            while (true) {

                if(breakEarly) {
                    break;
                }

                input = Console.ReadLine();
                
                if(input == "QUIT") {
                    breakEarly = true;
                }

                StreamWrite(stream, input);

                output = StreamRead(stream);

                while (!output.Contains("+DONE") && !output.Contains("+OK maildrop locked and ready") && !output.Contains("-ERR") && !output.Contains("+OK hellmaild HPOP server signing off (maildrop empty)")) {
                    output += StreamRead(stream);
                }

                Console.Write(output);

            }

        }

    }

}
