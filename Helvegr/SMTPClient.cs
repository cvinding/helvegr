using System;
using System.Net.Security;
using DnsClient;

namespace Helvegr {

    public class SMTPClient : TCPClient {

        private readonly EmailTags email;
        private readonly Flags flags;

        public SMTPClient(Flags flags, int portNumber, string clientCertificatePath, QueryType type) : base((new EmailTags(flags.arguments))._Domain, portNumber, clientCertificatePath, type) {
            this.flags = flags;
            this.email = new EmailTags(flags.arguments);
        }

        // The method for handling all our SMTP communication with our SMTP server
        protected override void StartClient(SslStream stream) {

            // Check if start message is sent
            if (!StreamRead(stream).Contains("220")) {
                throw new Exception("Missing start message");
            }

            // Send an EHLO message
            StreamWrite(stream, "EHLO");

            // Check if the SMTP server's response was correct
            if (!StreamRead(stream).Contains("250")) {
                throw new Exception("Unexpected EHLO answer");
            }

            // Send a DATA message
            StreamWrite(stream, "DATA");

            // Check if the SMTP server's response was correct
            if (!StreamRead(stream).Contains("354")) {
                throw new Exception("Unexpected DATA answer");
            }

            // Send the EMAIL data
            StreamWrite(stream, email.CreateMail());

            // Check if the SMTP server understood the EMAIL data
            if (!StreamRead(stream).Contains("250")) {
                throw new Exception("Unexpected return code");
            }

            //QUIT the transaction
            StreamWrite(stream, "QUIT");
        }

    }
}