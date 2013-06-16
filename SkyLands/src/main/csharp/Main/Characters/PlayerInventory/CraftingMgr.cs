using System;
using System.Collections.Generic;

using Mogre;

namespace Game.CharacSystem {
    public class CraftingMgr
    {
        private CraftTree mCraftTree;
        private Dictionary<byte[,], byte> mRecipes;
        
        public CraftingMgr()
        {
            this.mCraftTree = new CraftTree();
            //this.addRecipe(7, "#", "#", (byte)4);  // ConstructionBlock
            this.mRecipes = new Dictionary<byte[,], byte>();
            this.mRecipes.Add(new byte[,] { { 4, 255, 255 }, { 255, 255, 255 }, { 255, 255, 255 } }, 13);   // Planks
            this.mRecipes.Add(new byte[,] { { 13, 255, 13 }, { 255, 255, 255 }, { 13, 255, 13 } }, 7);  // ConstructionBlock
        }

        public void addRecipe(byte id, params object[] o) {
            if(o.Length < 3 || !(o[0] is string)) { throw new ArgumentException("Must have at least 3 argument"); }

            string[] lines = new string[this.getNumLines(o)];
            for(int i = 0; i < lines.Length; i++) {
                //if (o[i] is String && o[i + 1].GetType() != typeof(int)) { lines[i] = (string)o[i]; }
                lines[i] = (string)o[i];
            }

            for(int i = 0; i < lines.Length; i++) {
                for(int j = 0; j < lines[i].Length; j++) {
                    this.mCraftTree.add(this.fromCharToByte(lines[i][j], o), new CraftTree());
                }
                this.mCraftTree.add(255, new CraftTree());
            }
        }

        private byte fromCharToByte(char c, object[] o) {
            if(c == ' ') { return 255; } //Air
            else {
                for(int i = 0; i < o.Length - 1; i++) {
                    if (o[i] is string && ((string)o[i]) == c.ToString() && o[i + 1].GetType() == typeof(byte)) { return (byte)o[i + 1]; }
                }
            } throw new ArgumentException("You did not specify char : " + c);
        }


        private int getNumLines(object[] o) {
            int num = 0;
            for(int i = 0; i < o.Length - 1; i++) {
                if (o[i] is String && o[i + 1].GetType() != typeof(byte)) { num++; }
            }
            return num;
        }

        
        public byte getCraftingResult(byte[,] craftingGrid) {
            foreach (KeyValuePair<byte[,], byte> keyValuePair in this.mRecipes)
            {
                int y = 0, x = 0;
                for (; y < 3; y++)
                    for (x = 0; x < 3; x++)
                        if (keyValuePair.Key[x, y] != craftingGrid[x, y]) { y = 4; x = 4; }
                if(y == 3 && x == 3 ) { return keyValuePair.Value; }
            }
            return 255;
            /*if(craftingGrid.GetLength(0) != 3 && craftingGrid.GetLength(1) != 3) { throw new ArgumentException("Crafting must have 9 slots"); }

            Vector2 begin = Vector2.ZERO, end = Vector2.ZERO;

            for(int i = 0; i < 3;  i++) { for(int j = 0; j < 3;  j++) { if(craftingGrid[j, i] != 255) { begin = new Vector2(i, j); break; } } }
            for(int i = 2; i >= 0; i--) { for(int j = 2; j >= 0; j--) { if(craftingGrid[j, i] != 255) { end   = new Vector2(i, j); break; } } }

            List<byte> bytes = new List<byte>();

            for(int i = (int)begin.x; i < (int)end.x; i++) {
                for(int j = (int)begin.y; j < (int)end.y; j++) {
                    bytes.Add(craftingGrid[i, j]);
                }
                bytes.Add(255);
            }
            return findRecipe(bytes, 0, this.mCraftTree);*/
        }

        public byte findRecipe(List<byte> b, int i, CraftTree c) {
            if(i >= b.Count) { return c.Value; }

            if(c.hasChild(b[i])) { return this.findRecipe(b, i + 1, c[b[i]]); }
            if(b[i] == 255 && c.hasChild(b[i + 1])) { return this.findRecipe(b, i + 2, c[b[i + 1]]); }

            return 255;
        }
    }
}
