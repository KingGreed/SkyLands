using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generic;

using Mogre;

namespace Game.World.Blocks
{
    class SandLeftBlock : VanillaBlock
    {
        public SandLeftBlock() {
            this.mName = "Sand left";
            this.mMaterial = "cube/sand";
        }

        public override BlockFace[] getFaces() { 
            return new BlockFace[] {
                BlockFace.leftFace,
            };
        }
    }
}
