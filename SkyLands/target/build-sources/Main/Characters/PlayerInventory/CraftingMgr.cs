﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

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

        
        public byte getCraftingResult(byte[,] craftingGrid) {
            if(craftingGrid.GetLength(0) != 3 && craftingGrid.GetLength(1) != 3) { throw new ArgumentException("Crafting must have 9 slots"); }

            Vector2 begin = Vector2.ZERO, end = Vector2.ZERO;

            for(int i = 0; i < 3;  i++) { for(int j = 0; j < 3;  j++) { if(craftingGrid[j, i] != 255) { begin = new Vector2(i, j); break; } } }
            for(int i = 2; i >= 0; i--) { for(int j = 2; j >= 0; j--) { if(craftingGrid[j, i] != 255) { end   = new Vector2(i, j); break; } } }

            List<byte> b = new List<byte>();

            for(int i = (int)begin.x; i < (int)end.x; i++) {
                for(int j = (int)begin.y; j < (int)end.y; j++) {
                    b.Add(craftingGrid[i, j]);
                }
                b.Add(0);
            }
            return 255;
        }

        public byte findRecipe(List<byte> b, int i, CraftTree c) {
            if(i >= b.Count) { return c.Value; }

            if(c.hasChild(b[i])) { return this.findRecipe(b, i + 1, c[b[i]]); }
            if(b[i] == 255 && c.hasChild(b[i + 1])) { return this.findRecipe(b, i + 2, c[b[i + 1]]); }

            return 255;
        }
    }
}