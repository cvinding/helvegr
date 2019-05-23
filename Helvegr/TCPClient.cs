using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using DnsClient;

namespace Helvegr {

    public class TCPClient {

        // Collection of certificates the servers use
        private readonly X509CertificateCollection clientCertificates;

        // FQDN
        private readonly string serverHostName;

        // Port number
        private readonly int portNumber;

        // Used for setting important variables
        public TCPClient(string hostname, int portNumber, string clientCertificatePath, QueryType type) {
            try {
                // Find and return all certs for the client
                clientCertificates = new X509CertificateCollection(GetClientCertificates(clientCertificatePath));

                // Find the server hostname
                serverHostName = GetServerHostName(hostname, type);

                // Set the port number
                this.portNumber = portNumber;

            } catch (Exception ex) {
                Console.WriteLine(ex); 
            }
        }

        // Start our TCPClient
        public void Start() {
            // Find the server
            TcpClient server = new TcpClient(serverHostName, portNumber); //new TcpClient(serverHostName, portNumber);

            // Start the sslstream
            SslStream stream = new SslStream(server.GetStream(), false, (a, b, c, d) => true);

            // Authenticate this end as a sslstream client
            stream.AuthenticateAsClient(serverHostName.TrimEnd('.'), clientCertificates, SslProtocols.Tls12, false);

            try {

                // Start the client
                StartClient(stream);

            } catch (Exception ex) {
                Console.WriteLine(ex);
            }

            // Close sslstream
            stream.Close();
        }

        // Write the stream
        protected void StreamWrite(SslStream stream, string message) {
            var output = Encoding.UTF8.GetBytes(message + "\n");
            stream.Write(output);
        }

        // Read from the stream
        protected string StreamRead(SslStream stream) {
            byte[] buffer = new byte[1024];

            int n = stream.Read(buffer, 0, buffer.Length);

            string _message = Encoding.UTF8.GetString(buffer, 0, n);
            
            return _message;
        }

        // Override this method when creating a generalization of this class
        protected virtual void StartClient(SslStream stream) { }

        // GetClientCertificates() is used for finding all the certificates needed to contact the server
        private X509Certificate[] GetClientCertificates(string clientCertificatePath) {
            // Find the directory
            DirectoryInfo directory = new DirectoryInfo(clientCertificatePath);

            // Find the files
            FileInfo[] Files = directory.GetFiles("*.pfx"); //Getting PFX certs

            // If there is no files: throw exception
            if(Files.Length <= 0) {
                throw new Exception("No certificates found");
            }

            // Create a X509Certificate array
            X509Certificate[] certificates = new X509Certificate[Files.Length];

            // Add each certificate to our X509Certificate array
            for (int i = 0; i < Files.Length; i++) {
                X509Certificate2 certificate = new X509Certificate2(Files[i].ToString());
                certificates[i] = certificate;
            }

            // Return the certificate array
            return certificates;
        }

        // GetServerHostName() is used to return the full server hostname
        private string GetServerHostName(string hostname, QueryType type) {
            // Create a DNS lookup
            LookupClient lookup = new LookupClient();

            var result = lookup.Query(hostname, type);

            // Test to see if we can ping
            Ping ping = new Ping();

            for (int i = 0; i < result.Answers.Count; i++) {

                string DNSHost = result.Answers[i].ToString().Split(" ")[4];

                PingReply reply = ping.Send(DNSHost, 60*1000);

                if(reply.Status == IPStatus.Success) {
                    return DNSHost;
                }

            }
            
            // If there is no mailserver throw an exception
            throw new Exception("No mail server available");
        }


    }
}
