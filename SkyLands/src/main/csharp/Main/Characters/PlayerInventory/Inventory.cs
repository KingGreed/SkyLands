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
        public static Dictionary<byte, string> MagicCubes = new Dictionary<byte, string> { { 252, "fire.png" }, { 253, "waterCube.png" }, { 254, "crystal.png" } };
        
        private CraftingMgr mCraftingMgr;
        private Slot[,] mInventory;
        private int[] mYValues;
        private bool mMainInventory;

        public int X { get; private set; }
        public int Y { get; private set; }

        public Inventory(int x, int y, int[] yValues, bool mainInventory)
        {
            this.mCraftingMgr = new CraftingMgr();
            this.X = x;
            this.Y = y;
            this.mInventory = new Slot[this.Y, this.X];
            this.mYValues = yValues;    // Check the line representing the selectBar at first
            this.mMainInventory = mainInventory;
        }

        public void OnCraft()
        {
            if (!this.mMainInventory) { return; }

            byte[,] ingredients = new byte[3,3];
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    int id = this.GetIdFromIndex(40 + x + y*3);
                    ingredients[y, x] = (byte)(id == 0 ? 255 : id);
                }
            }

            byte result = this.mCraftingMgr.getCraftingResult(ingredients);
            if (result == 255) { return; }
            GUI.SetBlockAt(49, VanillaChunk.staticBlock[VanillaChunk.byteToString[result]].getItemTexture(), result == 13 ? 5 : 1);
        }

        public void Add(byte item, int amount = 1)
        {
            Slot s = new Slot(amount, item);
            foreach (int y in this.mYValues)
            {
                for (int x = 0; x < this.X; x++)
                {
                    if (this.mInventory[y, x] != null && this.mInventory[y, x].item == item)
                    {
                        this.addAt(s, x, y);
                        return;
                    }
                }
            }

            this.addAt(s, this.getFreeSlot());
        }

        public void OnClose(Inventory builder = null)
        {
            foreach (int y in this.mYValues)
            {
                for (int x = 0; x < this.X; x++)
                {
                    int index = x + y * this.X;
                    Slot s = null;
                    int amount = GUI.GetAmountAt(index);
                    if (amount >= 0)
                    {
                        byte id = this.GetIdFromIndex(index);
                        s = new Slot(amount, id);
                    }
                    this.mInventory[y, x] = s;
                }
            }

            Inventory special = this;
            int iMax = 48;
            if (builder != null)
            {
                special = builder;
                iMax = 55;
            }

            /* get crafting table */
            for (int i = 40; i <= iMax; i++)
            {
                int amount = GUI.GetAmountAt(i);
                if (amount >= 0) { special.Add(this.GetIdFromIndex(i), amount); }
            }
        }

        private byte GetIdFromIndex(int index)
        {
            string imgName = GUI.GetImageAt(index);
            if (imgName == "blank.png") { return 0; }
            byte id = MagicCubes.Keys.FirstOrDefault(b => MagicCubes[b] == imgName);
            return id != default(byte) ? id : VanillaChunk.textureToBlock[imgName].getId();
        }

        public Slot addAt(Slot s, Vector2 pos) { return this.addAt(s, (int)pos.x, (int)pos.y); }
        public Slot addAt(Slot s, int x, int y) {
            if (this.mInventory[y, x] == null || s.item != this.mInventory[y, x].item)
            {
                Slot tmp = this.mInventory[y, x];
                this.mInventory[y, x] = s;

                // Update selectBar
                if (y == 3 && this.mMainInventory) {
                    Selector.SetBlockAt(x, MagicCubes.ContainsKey(s.item) ? MagicCubes[s.item] :
                        VanillaChunk.staticBlock[VanillaChunk.byteToString[s.item]].getItemTexture(), s.amount);
                }

                return tmp;
            }

            if(this.mInventory[y, x].amount + s.amount > 64) {
                s.amount = this.mInventory[y, x].amount + s.amount - 64;
                this.mInventory[y, x].amount = 64;
                return s;
            } else { this.mInventory[y, x].amount += s.amount; return null; }
        }

        public void removeAt(int x, int y, int amount)
        {
            if (this.mInventory[y, x] == null)    { return; }
            if (this.mInventory[y, x].amount - amount == 0)
            {
                this.mInventory[y, x] = null;
                if (y == 3 && this.mMainInventory) { Selector.SetBlockAt(x, "blank.png"); }
            }
            else
            {
                this.mInventory[y, x].amount -= amount;
                Selector.SetBlockAt(x, "", this.mInventory[y, x].amount);
            }
        }

        public Vector2 getFreeSlot() {
            foreach (int y in this.mYValues) { for (int x = 0; x < this.X; x++) { if (this.mInventory[y, x] == null) { return new Vector2(x, y); } } }
            return new Vector2(-1, -1);
        }

        public Slot getSlot(int x, int y) {
            return this.mInventory[y, x];
        }

        public byte getItemTypeAt(int x, int y) {
            return this.mInventory[y, x].item;
        }

        public int getAmountAt(int x, int y) {
            return this.mInventory[y, x].amount;
        }
    }
}
