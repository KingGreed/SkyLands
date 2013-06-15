using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Script {
    class Item {
        public Vector3 loc;
        public string s;

        public Item(Vector3 l, string s) {
            this.loc = l;
            this.s = s;
        }
    }
}
