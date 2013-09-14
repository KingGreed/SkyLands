using System;
using System.Collections.Generic;

using Mogre;

namespace Game.CharacSystem {
    public class CraftingMgr {
        private CraftTree mCraftTree;

        public CraftingMgr() {
            this.mCraftTree = new CraftTree();
            this.addRecipe(13, new Object[] { "XXX", "X", (byte)4 });                          // Planks
            this.addRecipe(7,  new Object[] { "X X", "", "X X", "X", (byte)13 });              // ConstructionBlock
            this.addRecipe(7,  new Object[] { "XX", "XX", "X", (byte)3 });                     // ConstructionBlock
            this.addRecipe(22, new Object[] { "X X", " X ", "X X", "X", (byte)6 });            // Glass
            this.addRecipe(22, new Object[] { "X X", " X ", "X X", "X", (byte)10 });           // Glass
            this.addRecipe(22, new Object[] { "", " X ", " Y ", "X", (byte)6, "Y", (byte)4 }); // Levitator


        }
        /* Call examples :
        *  this.addRecipe(23, new Object[] { "XXX", "X", 21 });
        *  this.addRecipe(23, new Object[] { "XX", "X", "X", 21 });
        *  this.addRecipe(24, new Object[] { "XXX", "XYX", "ZZY", "X", 21, "Y", 18, "Z", 16 });
        *  With 21 18 and 16 being the id of item X, Y and Z
        *  Note : Items can only be represented with 1 char
        */
        public void addRecipe(byte resultId, params object[] o) {
            if (o.Length < 3 || !(o[0] is string)) { throw new ArgumentException("Must have at least 3 argument"); }

            string[] lines = new string[this.getNumLines(o)];

            string val;
            for (int i = 0; i < lines.Length; i++) {
                val = (string)o[i];
                if (val.Length == 3) {
                    lines[i] = val;
                }
                else if (val.Length < 3) {
                    for (; val.Length < 3; ) {
                        val += " ";
                    }
                    lines[i] = val;
                }
                else {
                    throw new ArgumentException("Argument {0} is too long");
                }
            }

            CraftTree c = this.mCraftTree;
            for (int i = 0; i < lines.Length; i++) {
                for (int j = 0; j < lines[i].Length; j++) {
                    if (i == lines.Length - 1 && j == lines[i].Length - 1) {//last one has the result
                        c = c.add(this.fromCharToByte(lines[i][j], o), new CraftTree(resultId));
                    }
                    else {
                        c = c.add(this.fromCharToByte(lines[i][j], o), new CraftTree());
                    }

                }

                if (i != lines.Length - 1) {
                    c = c.add(255, new CraftTree());
                }
            }
        }

        private byte fromCharToByte(char c, object[] o) {
            if (c == ' ') { return 255; } //Air
            else {
                for (int i = 0; i < o.Length; i++) {
                    if (o[i].GetType() == typeof(byte) && ((string)o[i - 1])[0] == c) { return (byte)o[i]; }
                }
            } throw new ArgumentException("You did not specify char : " + c);
        }


        private int getNumLines(object[] o) {
            for (int i = 0; i < o.Length; i++) {
                if (o[i].GetType() == typeof(byte)) { return i - 1; }
            }
            throw new Exception("Expected at least one item to be defined");
        }


        public byte getCraftingResult(byte[,] craftingGrid) {
            if (craftingGrid.GetLength(0) != 3 && craftingGrid.GetLength(1) != 3) { throw new ArgumentException("Crafting must have 9 slots"); }

            Vector2 min = new Vector2(3, 3), max = new Vector2();
            for (int y = 0; y < 3; y++) {
                for (int x = 0; x < 3; x++) {
                    if (craftingGrid[x, y] != 255) {
                        if (y < min.y) { min.y = y; }
                        if (y > max.y) { max.y = y; }

                        if (x < min.x) { min.x = x; }
                        if (x > max.x) { max.x = x; }
                    }
                }
            }



            List<byte> bytes = new List<byte>();

            for (int y = (int)min.y; y <= (int)max.y; y++) {
                for (int x = (int)min.x; x <= (int)max.x; x++) {
                    bytes.Add(craftingGrid[x, y]);
                }
                if (y != max.y) {
                    bytes.Add(255);
                }
            }
            return findRecipe(bytes, 0, this.mCraftTree);
        }

        private byte findRecipe(List<byte> b, int i, CraftTree c) {
            if (i >= b.Count) { return c.Value; }

            if (c.hasChild(b[i])) { return this.findRecipe(b, i + 1, c[b[i]]); }
            return 255;
        }
    }
}
