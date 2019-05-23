using System;
namespace Helvegr {

    public class Flag {

        public string name;

        public bool canBeTrue { get; set; } = false;

        public string link { get; set; } = null;

        public bool required { get; set; } = false;
        public bool isAction { get; set; } = false;

        public string defaultValue { get; set; } = null;
 
        public Flag(string name) {
            this.name = name;
        }

    }
}
