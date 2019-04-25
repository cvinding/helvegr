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

        private readonly X509CertificateCollection clientCertificates;

        private readonly string serverHostName;

        public TCPClient(string hostname, string clientCertificatePath, QueryType type) {
            try {
                // Find and return all certs for the client
                clientCertificates = new X509CertificateCollection(GetClientCertificates(clientCertificatePath));

                serverHostName = GetServerHostName(hostname, type);

            } catch (Exception ex) {
                Console.WriteLine(ex); 
            }

        }

        public void Start() {

            TcpClient server = new TcpClient(serverHostName, 25);

            SslStream stream = new SslStream(server.GetStream(), false, APP_CertificateValidation);

            stream.AuthenticateAsClient(serverHostName.TrimEnd('.'), clientCertificates, SslProtocols.Tls12, false);

            StartClient(stream);

            stream.Close();
        }

        protected void StreamWrite(SslStream stream, string message) {
            var output = Encoding.UTF8.GetBytes(message + "\n");
            stream.Write(output);
        }

        protected string StreamRead(SslStream stream) {
            byte[] buffer = new byte[1024];

            int n = stream.Read(buffer, 0, buffer.Length);

            string _message = Encoding.UTF8.GetString(buffer, 0, n);
            
            return _message;
        }


        protected virtual void StartClient(SslStream stream) { }

        private X509Certificate[] GetClientCertificates(string clientCertificatePath) {
            DirectoryInfo directory = new DirectoryInfo(clientCertificatePath);

            FileInfo[] Files = directory.GetFiles("*.pfx"); //Getting PFX certs

            if(Files.Length <= 0) {
                throw new Exception("No certificates found");
            }

            X509Certificate[] certificates = new X509Certificate[Files.Length];

            for (int i = 0; i < Files.Length; i++) {
                X509Certificate2 certificate = new X509Certificate2(Files[i].ToString());
                certificates[i] = certificate;
            }

            return certificates;
        }

        private string GetServerHostName(string hostname, QueryType type) {
            LookupClient lookup = new LookupClient();

            var result = lookup.Query(hostname, type);
            
            Ping ping = new Ping();

            for (int i = 0; i < result.Answers.Count; i++) {

                string DNSHost = result.Answers[i].ToString().Split(" ")[4];

                PingReply reply = ping.Send(DNSHost, 60*1000);

                if(reply.Status == IPStatus.Success) {
                    return DNSHost;
                }

            }

            throw new Exception("No mail server available");
        }


        private static bool APP_CertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
            if(sslPolicyErrors == SslPolicyErrors.None) { return true; }
            if(sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors) { return true; }
            Console.WriteLine("*** SSL Error: " + sslPolicyErrors.ToString());
            return false;
        }


    }
}
