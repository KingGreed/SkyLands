using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generic;

using Mogre;

namespace Game.World.Blocks
{
    class YellowSandLeftBlock : VanillaBlock
    {
        public YellowSandLeftBlock() {
            this.mName = "YellowSand left";
            this.mMaterial = "cube/YellowSand";
        }

        public override BlockFace[] getFaces() { 
            return new BlockFace[] {
                BlockFace.leftFace,
            };
        }
    }
}
