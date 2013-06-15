using System;
using Mogre;

namespace Game.CharacSystem
{
    public class Inventory
    {
        private Slot[,] mInventory;
        private int[] mYValues;

        public Inventory()
        {
            this.mInventory = new Slot[10, 4];
            this.mYValues = new int[] { 3, 1, 2, 0 };    // Check the line representing the selectBar at first
        }

        public void removeAt(int x, int y, int amount) {
            if(x < 0 || y < 0 || x > 9 || y > 3) { throw new IndexOutOfRangeException(); }
            if(amount > 64 || this.mInventory[x, y].amount - amount < 0) { throw new ArgumentException("Invalid amound"); }
            else if(this.mInventory[x, y].amount - amount == 0)          { this.mInventory[x, y] = null; }
        }

        public void AddOne(byte item)
        {
            Slot s = new Slot(1, item);
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

        public Slot addAt(Slot s, Vector2 pos) { return this.addAt(s, (int)pos.x, (int)pos.y); }
        public Slot addAt(Slot s, int x, int y) {
            if(x < 0 || y < 0 || x > 9 || y > 3) { throw new IndexOutOfRangeException();          }
            if(s.amount < 0)                     { throw new ArgumentException("Invalid amound"); }
            if (this.mInventory[x, y] == null || s.item != this.mInventory[x, y].item)
            {
                Slot tmp = this.mInventory[x, y];
                this.mInventory[x, y] = s;
                return tmp;
            }

            if(this.mInventory[x, y].amount + s.amount > 64) {
                s.amount = this.mInventory[x, y].amount + s.amount - 64;
                this.mInventory[x, y].amount = 64;
                return s;
            } else { this.mInventory[x, y].amount += s.amount; return null; }
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
