using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.World.Blocks
{
    class ConstructionBlock : VanillaBlock
    {
        public ConstructionBlock()
        {
            this.mName = "Construction";
            this.mMaterial = "";
            this.mId = 7;
        }
    }
}
