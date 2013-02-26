using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generic;

using Mogre;

namespace Game.World.Blocks
{
    class RedSandLeftBlock : VanillaBlock
    {
        public RedSandLeftBlock() {
            this.mName = "RedSand left";
            this.mMaterial = "cube/RedSand";
        }

        public override BlockFace[] getFaces() { 
            return new BlockFace[] {
                BlockFace.leftFace,
            };
        }
    }
}
