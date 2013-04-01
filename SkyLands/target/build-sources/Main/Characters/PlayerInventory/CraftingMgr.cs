using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Characters.PlayerInventory {
    public class CraftingMgr {
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

        public int getBegining(byte[] craftingGrid) { for(int i = 0; i < 9; i++)  { if(craftingGrid[i] != 255) { return i; } } return -1; }
        public int getEnd(byte[] craftingGrid)      { for(int i = 8; i != 0; i--) { if(craftingGrid[i] != 255) { return i; } } return -1; }
        
        public byte getCraftingResult(byte[] craftingGrid) {
            if(craftingGrid.Length != 9) { throw new ArgumentException("Crafting must have 9 slots"); }

            int begin = getBegining(craftingGrid), end = getEnd(craftingGrid);

            

            return 255;
        }
    }
}
