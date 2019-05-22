using System;
using System.Collections.Generic;
using System.Reflection;

namespace Helvegr {

    public class EmailTags {

        public string _Domain { get; set; }

        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }

        // Optional mail tags
        public string Cc { get; set; }
        public string Bcc { get; set; }

        public string Message { get; set; }

        private readonly string[] Required = { "To" , "From", "Subject" };

        // Set the email tags dynamically 
        public EmailTags(Dictionary<string, string> values) {

            string tempValue = String.Empty;
            
            foreach (var prop in this.GetType().GetProperties()) {

                if (prop.Name.StartsWith("_", StringComparison.CurrentCulture)) {
                    continue;
                }
                
                if (!values.TryGetValue(prop.Name, out tempValue)) {

                    if (Array.IndexOf(this.Required, prop.Name) >= 0) {
                        throw new Exception("Required missing parameter: " + prop.Name);
                    }

                    continue;
                }

                prop.SetValue(this, tempValue);
            }

            _Domain = To.Split("@")[1];
        }

        // Create the EMAIL from the email tags
        public string CreateMail() {
            string mail = String.Empty;

            mail += "To: " + To + "\n";
            mail += "From: " + From + "\n";
            mail += "Subject: " + Subject + "\n";

            if(Cc != null) {
                mail += "Cc: " + Cc + "\n";
            }

            if (Bcc != null) {
                mail += "Bcc: " + Bcc + "\n";
            }

            mail += "\n";

            if(Message != null) {
                mail += Message + "\n";
            }
           
            mail += ".\n";

            return mail;
        }

    }
}
