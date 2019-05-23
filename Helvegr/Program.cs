using System;
using DnsClient;
using System.Text.RegularExpressions;

namespace Helvegr {

    class Program {

        static void Main(string[] args) {
        
            // Take all the arguments and parse them into Flags class
            Flags flags = new Flags();
            flags.Parse(args);

            string certPath = "/opt/helvegr/certs/";

            try {

                string isInteractive;

                int port;

                // Check if the port is a valid integer
                if (!int.TryParse(flags.arguments["Port"], out port)) {
                    Console.WriteLine("--port must be a valid integer");
                    flags.ShowHelpMessage(flags.command, 1);
                }

                // If smtp are set && --interactive are not set, else if check if hpop are set
                if (flags.command == "smtp" && !flags.arguments.TryGetValue("--interactive", out isInteractive)) {
                    // Create an instance of SMTP client and send the email
                    SMTPClient smtp = new SMTPClient(flags, port, certPath, QueryType.MX);
                    // Start the client
                    smtp.Start();

                } else if (flags.command == "hpop" && !flags.arguments.TryGetValue("--interactive", out isInteractive)) {

                    string username = flags.arguments["Username"];

                    // Regex email address
                    var regex = "(?:[a-z0-9!#$%&'*+\\/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+\\/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])";
                    Match match = Regex.Match(username, regex, RegexOptions.IgnoreCase);

                    // if regex match is successful
                    if(!match.Success) {
                        Console.WriteLine("--username must be a valid email address");
                        flags.ShowHelpMessage(flags.command, 1);
                    }

                    // Start hpop server
                    POPClient hpop = new POPClient(flags, port, certPath, QueryType.MX);
                    hpop.Start();

                } else if (flags.arguments.TryGetValue("--interactive", out isInteractive)) {
                    // INTERACTIVE MODE, ONLY FOR SHOWCASING THE PROTOCOLS AND MAIL SERVER... ;))))))))

                    Console.Write("Enter hostname: ");
                    flags.arguments.Add("Username", "noone@" + Console.ReadLine());

                    Console.Write("Enter port number: ");
                    if(!int.TryParse(Console.ReadLine(), out port)) {
                        Console.WriteLine("Port number is not a valid integer");
                        Environment.Exit(1);
                    }

                    if (flags.command == "smtp") {

                        flags.arguments.Add("To", flags.arguments["Username"]);
                        flags.arguments.Add("From", "-1");
                        flags.arguments.Add("Subject", "-1");

                        // Create an instance of SMTP client and send the email
                        SMTPClient smtp = new SMTPClient(flags, port, certPath, QueryType.MX);
                        // Start the client
                        smtp.Start();

                    } else if (flags.command == "hpop") {

                        POPClient hpop = new POPClient(flags, port, certPath, QueryType.MX);

                        hpop.Start();
                    }

                }

            } catch (Exception ex) {
                Console.WriteLine(ex);
            }

        }
    }
}
