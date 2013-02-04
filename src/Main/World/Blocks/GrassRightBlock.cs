using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generic;

using Mogre;

namespace Game.World.Blocks
{
    class GrassRightBlock : VanillaBlock
    {
        public GrassRightBlock() {
            this.mName = "Grass right";
            this.mMaterial = "cube/grass/side";
        }

        public override BlockFace[] getFaces() { 
            return new BlockFace[] {
                BlockFace.rightFace
            };
        }
    }
}
