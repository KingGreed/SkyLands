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
            string[] l = new string[3];
            l[0] = (string)o[0];
            if(o[2] is byte) { l[1] = "   "; l[2] = "   "; }
            else {
                l[1] = (string)o[1];
                if(o[3] is byte) { l[2] = "   "; }
                else { l[2] = (string)o[2]; }
            }
            byte[] ke;

            for(int i = 0; i < this.parse(l[0]).Length; i++) {
                ke = new byte[3];
                ke[0] = this.fromCharToByte(this.parse(l[0])[i][0], o);
                ke[1] = this.fromCharToByte(this.parse(l[0])[i][1], o);
                ke[2] = this.fromCharToByte(this.parse(l[0])[i][2], o);

                CraftTree c1 = this.c.add(ke, new CraftTree());

                for(int j = 0; j < this.parse(l[1]).Length; j++) {

                    ke[0] = this.fromCharToByte(this.parse(l[1])[j][0], o);
                    ke[1] = this.fromCharToByte(this.parse(l[1])[j][1], o);
                    ke[2] = this.fromCharToByte(this.parse(l[1])[j][2], o);

                    CraftTree c2 = c1.add(ke, new CraftTree());

                    for(int k = 0; k < this.parse(l[2]).Length; k++) {

                        ke[0] = this.fromCharToByte(this.parse(l[2])[k][0], o);
                        ke[1] = this.fromCharToByte(this.parse(l[2])[k][1], o);
                        ke[2] = this.fromCharToByte(this.parse(l[2])[k][2], o);
                        CraftTree c3 = new CraftTree(id);

                        c2.childs.Add(ke, c3);
                    }
                }

            }
        }

        public byte getCraftingResult(byte[] b) {
            if(b.Length != 9) { throw new ArgumentException("Crafting must have 9 slots"); }

            if(this.c.childs.ContainsKey(new byte[3] { b[0], b[1], b[2] })) {
                if(this.c[new byte[3] { b[0], b[1], b[2] }].childs.ContainsKey(new byte[3] { b[3], b[4], b[5] })) {
                    if(this.c[new byte[3] { b[0], b[1], b[2] }][new byte[3] { b[3], b[4], b[5] }].childs.ContainsKey(new byte[3] { b[6], b[7], b[8] })) {
                        return this.c[new byte[3] { b[0], b[1], b[2] }][new byte[3] { b[3], b[4], b[5] }][new byte[3] { b[6], b[7], b[8] }].Value;
                    }
                }
            }
            return 255;
        }
        private byte fromCharToByte(char c, object[] o) {
            if(c == ' ') { return 255; } //Air
            else {
                for(int i = 0; i < o.Length - 1; i++) {
                    if(o[i] is string && ((string)o[i]) == c.ToString() && o[i + 1] is byte) { return (byte)o[i + 1]; }
                }
            } throw new ArgumentException("You did not specify char : " + c);
        }

        private string[] parse(string s) {
            if(s.Length == 0) { return new string[] {"   "}; }
            string[] str = new string[4 - s.Length];

            if(s.Length == 3) { str[0] = s; }
            else if(s.Length == 2) {
                str[0] = " " + s;
                str[1] = s + " ";
            } else if(s.Length == 1) {
                str[0] = "  " + s;
                str[1] = " " + s + " ";
                str[2] = s + " ";
            }

            return str;
        }
    }
}
