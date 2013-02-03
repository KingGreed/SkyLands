using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generic;

using Mogre;

namespace Game.World.Blocks
{
    class GrassSideBlock : VanillaBlock
    {
        public GrassSideBlock() {
            this.mName = "Grass side";
            this.mMaterial = "cube/grass/side";
        }

        public override BlockFace[] getFaces() { 
            return new BlockFace[] {
                BlockFace.frontFace, BlockFace.backFace,
                BlockFace.leftFace,  BlockFace.rightFace
            };
        }
    }
}
