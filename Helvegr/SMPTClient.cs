using System;
namespace Helvegr {

    public class SMPTClient : TCPClient {



        public SMPTClient(string hostname, string clientCertificatePath, EmailTags email) : base(hostname, clientCertificatePath) {

        }
    }
}
