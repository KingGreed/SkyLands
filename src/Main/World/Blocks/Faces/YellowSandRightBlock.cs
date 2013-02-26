using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generic;

using Mogre;

namespace Game.World.Blocks
{
    class YellowSandRightBlock : VanillaBlock
    {
        public YellowSandRightBlock() {
            this.mName = "YellowSand right";
            this.mMaterial = "cube/YellowSand";
        }

        public override BlockFace[] getFaces() { 
            return new BlockFace[] {
                BlockFace.rightFace,
            };
        }
    }
}
