using System;

namespace Helvegr {

    class Program {

        static void Main(string[] args) {

            EmailTags email = new EmailTags {
                to = "christian@hellmail.dk",
                from = "kent@hellmail.dk",
                subject = "Hej med dig"
            };

            string emailDomain = email.to.Split("@")[1];

            SMPTClient client = new SMPTClient(emailDomain, "/home/chri656v/HellMail/certs/", email);


        }
    }
}
