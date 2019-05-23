using System;
using System.Net.Security;
using DnsClient;

namespace Helvegr {

    public class SMTPClient : TCPClient {

        private EmailTags email;
        private Flags flags;

        public SMTPClient(Flags flags, int portNumber, string clientCertificatePath, QueryType type) : base((new EmailTags(flags.arguments))._Domain, portNumber, clientCertificatePath, type) {
            this.flags = flags;
        }

        // The method for handling all our SMTP communication with our SMTP server
        protected override void StartClient(SslStream stream) {

            string isInteractive;

            // Check if --interactive are set
            if (flags.arguments.TryGetValue("--interactive", out isInteractive)) {
                Interactive(stream);
                stream.Close();
                Environment.Exit(0);
            }

            email = new EmailTags(flags.arguments);

            // Check if start message is sent
            if (!StreamRead(stream).Contains("220")) {
                Console.WriteLine("Missing server greeting");
                Environment.Exit(1);
            }

            // Send an EHLO message
            StreamWrite(stream, "EHLO");

            // Check if the SMTP server's response was correct
            if (!StreamRead(stream).Contains("250")) {
                Console.WriteLine("Unexpected EHLO answer");
                Environment.Exit(1);
            }

            // Authenticate
            StreamWrite(stream, "AUTH PLAIN " + flags.arguments["Password"]);

            // check if authentication succeed
            if (!StreamRead(stream).Contains("235")) {
                Console.WriteLine("Could not authenticate with server");
                Environment.Exit(1);
            }

            // Send a DATA message
            StreamWrite(stream, "DATA");

            // Check if the SMTP server's response was correct
            if (!StreamRead(stream).Contains("354")) {
                Console.WriteLine("Unexpected DATA answer");
                Environment.Exit(1);
            }

            // Send the EMAIL data
            StreamWrite(stream, email.CreateMail());

            // Check if the SMTP server understood the EMAIL data
            if (!StreamRead(stream).Contains("250")) {
                Console.WriteLine("Unexpected DATA return code");
                Environment.Exit(1);
            }

            //QUIT the transaction
            StreamWrite(stream, "QUIT");
        }

        // Interactive() is used for Interactive mode
        private void Interactive(SslStream stream) {

            string input, output;
            bool breakEarly = false;

            Console.Write(StreamRead(stream));

            while (true) {

                if (breakEarly) {
                    break;
                }

                input = Console.ReadLine();

                if (input == "QUIT") {
                    breakEarly = true;
                }

                StreamWrite(stream, input);

                output = StreamRead(stream);

                Console.Write(output);

            }

        }

    }
}