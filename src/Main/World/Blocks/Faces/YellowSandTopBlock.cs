using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generic;

using Mogre;

namespace Game.World.Blocks
{
    class YellowSandTopBlock : VanillaBlock
    {
        public YellowSandTopBlock() {
            this.mName = "YellowSand top";
            this.mMaterial = "cube/YellowSand";
        }

        public override BlockFace[] getFaces() { 
            return new BlockFace[] {
                BlockFace.upperFace,
            };
        }
    }
}
