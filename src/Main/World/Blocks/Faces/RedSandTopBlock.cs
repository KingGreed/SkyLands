using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generic;

using Mogre;

namespace Game.World.Blocks
{
    class RedSandTopBlock : VanillaBlock
    {
        public RedSandTopBlock() {
            this.mName = "RedSand top";
            this.mMaterial = "cube/RedSand";
        }

        public override BlockFace[] getFaces() { 
            return new BlockFace[] {
                BlockFace.upperFace,
            };
        }
    }
}
