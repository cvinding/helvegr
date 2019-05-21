using System;
using System.Collections.Generic;

namespace Helvegr
{
    public class Flags {
        
        public string command;

        private readonly string[] commands = { "--he" };
        private readonly string[] subCommands = { "smtp", "hpop" };

        public Dictionary<string, string> arguments;

        public void Parse(string[] args) {

            command = args[0];

            // Check if non of the commands or sub commands is set
            if(Array.IndexOf(commands, command) == -1 && Array.IndexOf(subCommands, command) == -1) {

                // Show help message
                ShowHelpMessage("--help", 1);

            // Check if sub commands is set
            } else if(Array.IndexOf(subCommands, command) >= 0) { 

                // Check if --help flag is set
                if(Array.IndexOf(args, "--he") >= 0) {
                    ShowHelpMessage(command, 0);
                }

            // Check if --help flag is set
            } else if(command == "--he") {

                ShowHelpMessage("--help", 0);
            }

            List<Flag> flags = GetFlags(command);

            arguments = new Dictionary<string, string>();

            for (int i = 1; i < args.Length; i++) {

                bool exists = true;

                foreach(Flag flag in flags) {

                    if(flag.name != args[i]) {
                        exists = false;
                    }
                }

                // If the flag is a valid flag add it to the Dictionary, else terminate the program
                if (Array.IndexOf(flags, args[i]) >= 0) {
                    string memberName = args[i].Substring(2, 1).ToUpper() + args[i].Substring(3);

                    arguments.Add(memberName, args[i + 1]);
                    i++;
                } else {
                    Console.WriteLine("Unrecognized flag '" + args[i] + "' \n");
                    ShowHelpMessage(command, 1);
                }

            }

        }

        private List<Flag> GetFlags(string ID) {
            if(ID == "smtp") {

                return new List<Flag> {
                    new Flag("--to", false),
                    new Flag("--from", false),
                    new Flag("--subject", false),
                    new Flag("--message", false),
                    new Flag("--cc", false),
                    new Flag("--bcc", false)
                }; 

            } else {

                return new List<Flag> {
                    new Flag("--username", false, "--password"),
                    new Flag("--password", false, "--username"),
                    new Flag("--stat", false),
                    new Flag("--list", false),
                    new Flag("--retrieve", false),
                    new Flag("--delete", false),
                    new Flag("--start", false, "--end"),
                    new Flag("--end", false, "--start")
                };

            }

        }

        private void ShowHelpMessage(string command, int status) {

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
