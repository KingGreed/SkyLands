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
            this.mMaterial = "";
            this.mId = 13;
            this.mMeshType = (byte)2;
        }

        public override string getFace(int i) { return "cube/planks"; }
    }
}
