using System;
using DnsClient;
using System.Reflection;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Helvegr {

    class Program {

        static void Main(string[] args) {

            string[] flags = { "--to", "--from", "--subject", "--message" };

            Dictionary<string, string> values = new Dictionary<string, string>();

            // Find all the available flags the user has set
            for(int i = 0; i < args.Length; i++) {
            
                // If the flag is a valid flag add it to the Dictionary, else terminate the program
                if (Array.IndexOf(flags, args[i]) >= 0) {
                    string memberName = args[i].Substring(2,1).ToUpper() + args[i].Substring(3);
                    values.Add(memberName, args[i + 1]);
                    i++;
                } else {
                    Console.WriteLine("Unrecognized flag '" + args[i] + "'");
                    Environment.Exit(1);
                }

            }

            // Create an instance of EmailTags, and add the users email tags
            EmailTags email = new EmailTags(values);

            try {
                // Create an instance of SMTP client and send the email
                SMTPClient client = new SMTPClient(email, "/opt/helvegr/certs/", QueryType.MX);
                // Start the client
                client.Start();
      
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
          

           /* byte[] bytes = Encoding.Unicode.GetBytes("^a{V[bNwYq5d>gw&c4:fw(>~5'Et<SFc;hDpL8\\!");
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = string.Empty;
            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }

            Console.WriteLine(hashString);*/
            /*Flags flags = new Flags();

            flags.Parse(args);*/

        }
    }
}
