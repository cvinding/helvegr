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

            string username;

            if(!arguments.TryGetValue("Username", out username)) {
                //throw new Exception("Missing required arguments: --username and --password");
                Console.WriteLine("Missing required arguments: --username and --password");
                flags.ShowHelpMessage("hpop", 1);
            }

        }

    }

}
