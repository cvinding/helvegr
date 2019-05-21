using System;
namespace Helvegr {

    public class Flag {

        public readonly string name;
        public readonly bool canBeTrue;
        public readonly string link;

        public Flag(string name, bool canBeTrue) {
            this.name = name;
            this.canBeTrue = canBeTrue;
            this.link = null;
        }

        public Flag(string name, bool canBeTrue, string link) {
            this.name = name;
            this.canBeTrue = canBeTrue;
            this.link = link;
        }

    }
}
