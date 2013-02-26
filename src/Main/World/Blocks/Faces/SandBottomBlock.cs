using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generic;

using Mogre;

namespace Game.World.Blocks
{
    class SandBottomBlock : VanillaBlock
    {
        public SandBottomBlock() {
            this.mName = "Sand bottom";
            this.mMaterial = "cube/sand";
        }

        public override BlockFace[] getFaces() { 
            return new BlockFace[] {
                BlockFace.underFace,
            };
        }
    }
}
