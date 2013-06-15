using System;
using System.Collections.Generic;

namespace Game.CharacSystem {
    public class CraftTree {
        private byte mVal;
        private Dictionary<byte, CraftTree> mChilds;

        public byte Value { get { return this.mVal; } }
        public CraftTree this[byte index] { get { return this.mChilds[index]; } }

        public CraftTree(byte value = 255)
        {
            this.mVal = value;
            this.mChilds = new Dictionary<byte, CraftTree>();;
        }

        public CraftTree add(byte b, CraftTree c) {
            if(this.mChilds.ContainsKey(b)) {
                if(c.Value != 255 && this[b].Value != 255) { throw new Exception("Recipe already exists"); } 
                else if(this[b].Value == 255 && c.Value != 255) { this[b].setValue(c.Value); }
                return this[b];
            }
            else { this.mChilds.Add(b, c); return c; }
        }

        public void setValue(byte b) { this.mVal = b; }
        public bool hasChild(byte key) { return this.mChilds.ContainsKey(key); }
    }
}
