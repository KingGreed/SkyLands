using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.CharacSystem.PlayerInventory
{
    class Slot {
        public int amount;
        public char item;

        public Slot(int amount, char item) {
            this.amount = amount;
            this.item = item;
        }

    }
}
