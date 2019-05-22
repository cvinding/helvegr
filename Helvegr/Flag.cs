using System;
namespace Helvegr {

    public class Flag {

        public string name;
        public bool canBeTrue { get; set; } = false;
        public string link { get; set; } = null;
        public bool required { get; set; } = false;

        public Flag(string name) {
            this.name = name;
        }

        // Flag() is used for setting a flag and canBeTrue value
        /* public Flag(string name, bool canBeTrue) {
             this.name = name;
             this.canBeTrue = canBeTrue;
             this.link = null;
         }

         // Flag() overload is used for adding a link between two links
         public Flag(string name, bool canBeTrue, string link) {
             this.name = name;
             this.canBeTrue = canBeTrue;
             this.link = link;
         }
         */
    }
}
