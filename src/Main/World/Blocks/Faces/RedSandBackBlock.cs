using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generic;

using Mogre;

namespace Game.World.Blocks
{
    class RedSandBackBlock : VanillaBlock
    {
        public RedSandBackBlock() {
            this.mName = "RedSand back";
            this.mMaterial = "cube/RedSand";
        }

        public override BlockFace[] getFaces() { 
            return new BlockFace[] {
                BlockFace.backFace,
            };
        }
    }
}
