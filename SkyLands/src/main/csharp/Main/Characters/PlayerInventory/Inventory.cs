using System;
using System.Collections.Generic;
using System.Linq;
using Mogre;

using Game.BaseApp;
using Game.World.Generator;
using Game.GUIs;

namespace Game.CharacSystem
{
    public class Inventory
    {
        private CraftingMgr mCraftingMgr;
        private Slot[,] mInventory;
        private int[] mYValues;
        public Dictionary<byte, string> MagicCubes { get; private set; }

        public Inventory()
        {
            this.mCraftingMgr = new CraftingMgr();
            this.mInventory = new Slot[10, 4];
            this.mYValues = new int[] { 3, 1, 2, 0 };    // Check the line representing the selectBar at first
            this.MagicCubes = new Dictionary<byte, string> { { 252, "fire.png" }, { 253, "waterCube.png" }, { 254, "crystal.png" } };
        }

        public void OnCraft()
        {
            byte[,] ingredients = new byte[3,3];
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    int id = this.GetIdFromIndex(40 + x + y*3);
                    ingredients[x, y] = (byte)(id == 0 ? 255 : id);
                }
            }

            byte result = this.mCraftingMgr.getCraftingResult(ingredients);
            if (result == 255) { return; }

            OgreForm.webView.ExecuteJavascript("setBlockAt(49, '" + VanillaChunk.staticBlock[VanillaChunk.byteToString[result]].getItemTexture() +
                                    "', " + (result == 13 ? 4 : 1) + ")");
        }

        public void Add(byte item, int amount = 1)
        {
            Slot s = new Slot(amount, item);
            foreach (int y in this.mYValues)
            {
                for (int x = 0; x < 10; x++)
                {
                    if (this.mInventory[x, y] != null && this.mInventory[x, y].item == item)
                    {
                        this.addAt(s, x, y);
                        return;
                    }
                }
            }

            this.addAt(s, this.getFreeSlot());
        }

        public void GetInventoryModification()
        {
            foreach (int y in this.mYValues)
            {
                for (int x = 0; x < 10; x++)
                {
                    int index = x + y * 10;
                    Slot s = null;
                    string sAmount = OgreForm.webView.ExecuteJavascriptWithResult("getAmountAt(" + index + ")");
                    if (sAmount != "")
                    {
                        int amount = int.Parse(sAmount);
                        byte id = this.GetIdFromIndex(index);
                        s = new Slot(amount, id);
                    }
                    this.mInventory[x, y] = s;
                }
            }

            /* get crafting table */
            for (int i = 40; i <= 48; i++)
            {
                string sAmount = OgreForm.webView.ExecuteJavascriptWithResult("getAmountAt(" + i + ")");
                if (sAmount != "")
                {
                    int amount = int.Parse(sAmount);
                    this.Add(this.GetIdFromIndex(i), amount);
                }
            }
        }

        private byte GetIdFromIndex(int index)
        {
            string imgName = OgreForm.webView.ExecuteJavascriptWithResult("getImageAt(" + index + ")");
            string[] val = imgName.Split('/');
            imgName = val[val.Length - 1];
            if (imgName == "blank.png") { return 0; }
            byte id = this.MagicCubes.Keys.FirstOrDefault(b => this.MagicCubes[b] == imgName);
            return id != default(byte) ? id : VanillaChunk.textureToBlock[imgName].getId();
        }

        public Slot addAt(Slot s, Vector2 pos) { return this.addAt(s, (int)pos.x, (int)pos.y); }
        public Slot addAt(Slot s, int x, int y) {
            if(x < 0 || y < 0 || x > 9 || y > 3) { throw new IndexOutOfRangeException();          }
            if(s.amount < 0)                     { throw new ArgumentException("Invalid amound"); }
            if (this.mInventory[x, y] == null || s.item != this.mInventory[x, y].item)
            {
                Slot tmp = this.mInventory[x, y];
                this.mInventory[x, y] = s;

                // Update selectBar
                if (y == 3) {
                    Selector.SetMaterialAt(x, this.MagicCubes.ContainsKey(s.item) ? this.MagicCubes[s.item] :
                        VanillaChunk.staticBlock[VanillaChunk.byteToString[s.item]].getItemTexture());
                }

                return tmp;
            }

            if(this.mInventory[x, y].amount + s.amount > 64) {
                s.amount = this.mInventory[x, y].amount + s.amount - 64;
                this.mInventory[x, y].amount = 64;
                return s;
            } else { this.mInventory[x, y].amount += s.amount; return null; }
        }

        public void removeAt(int x, int y, int amount)
        {
            if (x < 0 || y < 0 || x > 9 || y > 3) { throw new IndexOutOfRangeException(); }
            if (this.mInventory[x, y] == null)    { return; }
            if (this.mInventory[x, y].amount - amount == 0)
            {
                this.mInventory[x, y] = null;
                if (y == 3) { Selector.SetMaterialAt(x, "blank.png"); }
            }
            else
                this.mInventory[x, y].amount -= amount;
        }

        public Vector2 getFreeSlot() {
            foreach (int y in this.mYValues) { for (int x = 0; x < 10; x++) { if (this.mInventory[x, y] == null) { return new Vector2(x, y); } } }
            return new Vector2(-1, -1);
        }

        public Slot getSlot(int x, int y) {
            if(x < 0 || y < 0 || x > 9 || y > 3) { throw new IndexOutOfRangeException(); }
            return this.mInventory[x, y];
        }

        public byte getItemTypeAt(int x, int y) {
            if(x < 0 || y < 0 || x > 9 || y > 3) { throw new IndexOutOfRangeException(); }
            return this.mInventory[x, y].item;
        }

        public int getAmountAt(int x, int y) {
            if(x < 0 || y < 0 || x > 9 || y > 3) { throw new IndexOutOfRangeException(); }
            return this.mInventory[x, y].amount;
        }
    }
}
