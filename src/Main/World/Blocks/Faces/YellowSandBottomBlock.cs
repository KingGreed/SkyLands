using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generic;

using Mogre;

namespace Game.World.Blocks
{
    class YellowSandBottomBlock : VanillaBlock
    {
        public YellowSandBottomBlock() {
            this.mName = "YellowSand bottom";
            this.mMaterial = "cube/YellowSand";
        }

        public override BlockFace[] getFaces() { 
            return new BlockFace[] {
                BlockFace.underFace,
            };
        }
    }
}
