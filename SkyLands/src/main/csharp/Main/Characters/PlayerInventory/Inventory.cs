using System;
using Game.World.Generator;
using Mogre;

using Game.BaseApp;

namespace Game.CharacSystem
{
    public class Inventory
    {
        
        
        private Slot[,] mInventory = new Slot[10, 4];

        public void removeAt(int x, int y, int amount) {
            if(x < 0 || y < 0 || x > 9 || y > 3) { throw new IndexOutOfRangeException(); }
            if(amount > 64 || this.mInventory[x, y].amount - amount < 0) { throw new ArgumentException("Invalid amound"); }
            else if(this.mInventory[x, y].amount - amount == 0)          { this.mInventory[x, y] = null; }
        }

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
            for(int i = 0; i < 10; i++) { for(int j = 0; j < 4; j++) { if(this.mInventory[i, j] == null) { return new Vector2(i, j); } } }
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
