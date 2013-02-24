using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generic;

using Mogre;

namespace Game.World.Blocks
{
    class ConstructionRightBlock : VanillaBlock
    {
        public ConstructionRightBlock() {
            this.mName = "Construction right";
            this.mMaterial = "cube/construction/side1";
        }

        public override BlockFace[] getFaces() { 
            return new BlockFace[] {
                BlockFace.rightFace
            };
        }
    }
}
