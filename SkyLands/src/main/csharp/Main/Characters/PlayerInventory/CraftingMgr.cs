using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Characters.PlayerInventory {
    class CraftingMgr {
        private CraftTree c = new CraftTree();
        public CraftingMgr() {

        }

        public void addRecipe(byte id, params object[] o) {
            if(o.Length < 3 || !(o[0] is string)) { throw new ArgumentException("Must have at least 3 argument"); }

            string[] l = new string[this.getNumLines(o)];
            for(int i = 0; i < l.Length; i++) {
                if(o[i] is String && !(o[i + 1] is byte)) { l[i] = (string)o[i]; }
            }

            CraftTree temp = c;
            for(int i = 0; i < l.Length; i++) {
                for(int j = 0; j < l[i].Length; j++) {
                    temp = c.add(this.fromCharToByte(l[i][j], o), new CraftTree());
                }
                temp = c.add(0, new CraftTree());
            }
        }

        private byte fromCharToByte(char c, object[] o) {
            if(c == ' ') { return 255; } //Air
            else {
                for(int i = 0; i < o.Length - 1; i++) {
                    if(o[i] is string && ((string)o[i]) == c.ToString() && o[i + 1] is byte) { return (byte)o[i + 1]; }
                }
            } throw new ArgumentException("You did not specify char : " + c);
        }


        private int getNumLines(object[] o) {
            int num = 0;
            for(int i = 0; i < o.Length - 1; i++) {
                if(o[i] is String && !(o[i + 1] is byte)) { num++; }
            }
            return num;
        }

        
        public byte getCraftingResult(byte[] b) {
            if(b.Length != 9) { throw new ArgumentException("Crafting must have 9 slots"); }
            CraftTree temp = this.c;
            for(int i = 0; i < b.Length; i++) {

                if(temp.childs.ContainsKey(b[i]) && temp[b[i]].Value == 255) { temp = temp[b[i]]; }
                else if(temp.childs.ContainsKey(b[i]) && temp[b[i]].Value != 255) { return temp[b[i]].Value; }
                else { return 255; }
            }
            return 255;
        }
    }
}
