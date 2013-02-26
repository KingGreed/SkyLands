using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generic;

using Mogre;

namespace Game.World.Blocks
{
    class RedSandBottomBlock : VanillaBlock
    {
        public RedSandBottomBlock() {
            this.mName = "RedSand bottom";
            this.mMaterial = "cube/RedSand";
        }

        public override BlockFace[] getFaces() { 
            return new BlockFace[] {
                BlockFace.underFace,
            };
        }
    }
}
