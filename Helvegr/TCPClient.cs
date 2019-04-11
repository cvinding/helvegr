using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using DnsClient;

namespace Helvegr {

    public class TCPClient {

        private readonly X509CertificateCollection clientCertificates;

        public TCPClient(string hostname, string clientCertificatePath) {
            clientCertificates = new X509CertificateCollection(GetClientCertificates(clientCertificatePath));



            LookupClient lookup = new LookupClient();

            var result = lookup.Query(hostname, QueryType.ANY);

            Console.WriteLine(result.Answers[0]);

        }

        private X509Certificate[] GetClientCertificates(string clientCertificatePath) {
            DirectoryInfo directory = new DirectoryInfo(clientCertificatePath);

            FileInfo[] Files = directory.GetFiles("*.pfx"); //Getting PFX certs

            X509Certificate[] certificates = new X509Certificate[Files.Length];

            for (int i = 0; i < Files.Length; i++) {
                X509Certificate2 certificate = new X509Certificate2(Files[i].ToString());
                certificates[i] = certificate;
            }

            return certificates;
        }

    }
}
