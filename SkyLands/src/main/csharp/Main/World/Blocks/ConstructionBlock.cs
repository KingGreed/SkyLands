using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Game.CharacSystem;

namespace Game.World.Blocks
{
    class ConstructionBlock : VanillaBlock
    {
        private Inventory mInventory;
        
        public ConstructionBlock()
        {
            this.mName = "Construction";
            this.mMaterial = "";
            this.mItemTexture = "constructionBlockSide2.png";
            this.mId = 7;
            this.mInventory = new Inventory(5, 3, new int[] {0, 1, 2}, false);
        }

        public override string getFace(int i) {
            switch (i) {
                case 0: return "cube/construction/side2";
                case 1: return "cube/construction/side2";
                case 2: return "cube/construction/top";
                case 3: return "cube/construction/bottom";
                case 4: return "cube/construction/side1";
                case 5: return "cube/construction/side1";
            }
            return "";
        }

        public override bool onLeftClick()
        {
            User.OpenBuilder = true;
            User.BuilderInventory = this.mInventory;
            return false;
        }
    }
}
