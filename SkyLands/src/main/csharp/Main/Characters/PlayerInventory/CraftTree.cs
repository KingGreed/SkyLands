using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Geo;

namespace Game.Characters.PlayerInventory {
    public class CraftTree {
        private byte mVal = 255;
        private bool mEndOfLine = false;
        public  byte Value { get { return this.mVal; } }
        public Dictionary<byte, CraftTree> childs = new Dictionary<byte,CraftTree>();
        public CraftTree this[byte index] {
            get { return this.childs[index]; }
        } 

        public CraftTree(bool endOfLine = false) { this.mEndOfLine = endOfLine; }
        public CraftTree(byte value, bool endOfLine = false) { this.mVal = value; this.mEndOfLine = endOfLine; }

        public CraftTree add(byte b, CraftTree c) {
            if(this.childs.ContainsKey(b)) {
                if(c.Value != 255 && this[b].Value != 255) { throw new Exception("Recipe already exists"); } 
                else if(this[b].Value == 255 && c.Value != 255) { this[b].setValue(c.Value); }
                return this[b];
            }
            else { this.childs.Add(b, c); return c; }
        }

        public void setValue(byte b) { this.mVal = b; }
        public bool hasChild(byte key) { return this.childs.ContainsKey(key); }
    }
}
