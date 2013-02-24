using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generic;

using Mogre;

namespace Game.World.Blocks
{
    class ConstructionLeftBlock : VanillaBlock
    {
        public ConstructionLeftBlock() {
            this.mName = "Construction left";
            this.mMaterial = "cube/construction/side1";
        }

        public override BlockFace[] getFaces() { 
            return new BlockFace[] {
                BlockFace.leftFace
            };
        }
    }
}
