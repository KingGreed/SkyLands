using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generic;

using Mogre;

namespace Game.World.Blocks
{
    class ConstructionFrontBlock : VanillaBlock
    {
        public ConstructionFrontBlock() {
            this.mName = "Construction front";
            this.mMaterial = "cube/construction/side2";
        }

        public override BlockFace[] getFaces() { 
            return new BlockFace[] {
                BlockFace.frontFace,
            };
        }
    }
}
