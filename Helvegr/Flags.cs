using System;
using System.Collections.Generic;

namespace Helvegr
{
    public class Flags {

        public string command;

        // List of all commands
        private readonly string[] commands = { "--he" };

        // List of all sub commands
        private readonly string[] subCommands = { "smtp", "hpop" };

        // Argument list
        public Dictionary<string, string> arguments;

        // Parse() is used for parsing the CLI arguments
        public void Parse(string[] args) {

            command = args[0];

            // Check if non of the commands or sub commands are set
            if(Array.IndexOf(commands, command) == -1 && Array.IndexOf(subCommands, command) == -1) {

                // Show help message
                ShowHelpMessage("--help", 1);

            // Check if sub commands are set
            } else if(Array.IndexOf(subCommands, command) >= 0) { 

                // Check if --help flag are set
                if(Array.IndexOf(args, "--he") >= 0) {
                    ShowHelpMessage(command, 0);
                }

            // Check if --help flag are set
            } else if(command == "--he") {

                ShowHelpMessage("--help", 0);
            }

            Dictionary<string, Flag> flags = GetFlags(command);

            arguments = new Dictionary<string, string>();

            for (int i = 1; i < args.Length; i++) {

                Flag currentFlag;

                // If the flag is a valid flag add it to the Dictionary, else terminate the program
                if (flags.TryGetValue(args[i], out currentFlag)) {
                    string memberName = args[i].Substring(2, 1).ToUpper() + args[i].Substring(3);

                    Console.WriteLine(currentFlag.name);

                    /*
                    if (currentFlag.required) {
                        if (Array.IndexOf(args, currentFlag.name) == -1) {
                            Console.WriteLine("Missing required argument: " + currentFlag.name);
                            ShowHelpMessage(command, 1);
                        }
                    }*/

                    if (currentFlag.link != null) {
                        if (Array.IndexOf(args, currentFlag.link) == -1) {
                            Console.WriteLine("Using " + args[i] + " requires the use of " + currentFlag.link);
                            ShowHelpMessage(command, 1);
                        }
                    }

                    if (currentFlag.canBeTrue) {

                        if (!arguments.TryAdd(memberName, "true")) {
                            Console.WriteLine(args[i] + " already used, each flag can only be used once");
                            ShowHelpMessage(command, 1);
                        }

                    } else if (args.Length >= i + 2) {
                    
                        if(!arguments.TryAdd(memberName, args[i + 1])) {
                            Console.WriteLine(args[i] + " already used, each flag can only be used once");
                            ShowHelpMessage(command, 1);
                        }
                        i++;
                    
                    } else {
                        Console.WriteLine(args[i] + " requires a value");
                        ShowHelpMessage(command, 1);
                    }

                } else {
                    Console.WriteLine("Unrecognized flag '" + args[i] + "' \n");
                    ShowHelpMessage(command, 1);
                }

            }

            //--to

            //To

            foreach(KeyValuePair<string, Flag> pair in flags) {

                string memberName = pair.Value.name.Substring(2, 1).ToUpper() + pair.Value.name.Substring(3);

                string value;

                if (pair.Value.required && !arguments.TryGetValue(memberName, out value)) {
                    Console.WriteLine("Missing required argument: " + pair.Key);
                    ShowHelpMessage(command, 1);
                }

            }

        }

        private Dictionary<string, Flag> GetFlags(string ID) {
            if(ID == "smtp") {
            
                return new Dictionary<string, Flag> {
                    { "--to", new Flag("--to") },
                    { "--from", new Flag("--from") },
                    { "--subject", new Flag("--subject") },
                    { "--message", new Flag("--message") },
                    { "--cc", new Flag("--cc") },
                    { "--bcc", new Flag("--bcc") }
                };

            } else {

                return new Dictionary<string, Flag> {
                    {"--username", new Flag("--username") { link = "--password", required = true } },
                    {"--password", new Flag("--password") { link = "--username", required = true } },
                    {"--stat", new Flag("--stat")},
                    {"--list", new Flag("--list")},
                    {"--retrieve", new Flag("--retrieve")},
                    {"--delete", new Flag("--delete")},
                    {"--start", new Flag("--start") { link = "--end" } },
                    {"--end", new Flag("--end") { link = "--start" } },
                };

            }

        }

        public void ShowHelpMessage(string command, int status) {

            if(command == "--help") {

                Console.WriteLine(
                    "helvegr is used to interact with the hellmaild service\n\n" +
                    "usage: helvegr [--help] [<smtp|hpop> <args>]\n\n" +
                    "subcommand help: helvegr <smtp|hpop> <--help>\n"
                );

            } else if (command == "smtp") {

                Console.WriteLine(
                    "helvegr is used to interact with the hellmaild service\n\n" +
                    "the smtp subcommand of helvegr is used to send mail to the hellmaild SMTP service\n\n" +
                    "usage: helvegr smtp [--help] <--to=<email>> <--from=<email>> <--subject=<string>> [--message=<string>] [--cc=<\"email,email..\">] [--bcc=<\"email,email..\">]\n"
                );

            } else if (command == "hpop") {

                Console.WriteLine(
                    "helvegr is used to interact with the hellmaild service\n\n" +
                    "the hpop subcommand of helvegr is used to manage the user's mailbox from the hellmaild HPOP service\n\n" +
                    "usage: helvegr hpop [--help] [--stat=<RECIEVED|SENT>] [--list=<RECEIVED|SENT> [<--start=<index>> <--end=<index>>]] [--retrieve=<id string>] [--delete=<id string>]\n\n" +
                    "details:\n" +
                    "  --retrieve\tRetrieve a string of mails. The 'id string' parameter is a string of all the email ids you want to retrieve, e.g. \"1 22 37 41\" is a string of mail ids.\n" +
                    "  --delete\tDelete one or more mails. The 'id string' parameter is a string of all the email ids you want to delete, e.g. \"1 22 37 41\" is a string of mail ids. \n"
                );
            
            }

            Environment.Exit(status);
        }

    }
}
