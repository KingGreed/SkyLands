using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generic;

using Mogre;

namespace Game.World.Blocks
{
    class RedSandRightBlock : VanillaBlock
    {
        public RedSandRightBlock() {
            this.mName = "RedSand right";
            this.mMaterial = "cube/RedSand";
        }

        public override BlockFace[] getFaces() { 
            return new BlockFace[] {
                BlockFace.rightFace,
            };
        }
    }
}
