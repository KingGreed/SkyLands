using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.World.Blocks
{
    class WoodenSlab : VanillaBlock
    {
        public WoodenSlab() {
            this.mName = "Wooden Slab";
            this.mMaterial = "cube/planks";
            this.mItemTexture = "towerWood.jpg";
            this.mId = 13;
            this.mMeshType = 1;
        }

    }
}
