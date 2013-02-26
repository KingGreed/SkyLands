using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generic;

using Mogre;

namespace Game.World.Blocks
{
    class SandRightBlock : VanillaBlock
    {
        public SandRightBlock() {
            this.mName = "Sand right";
            this.mMaterial = "cube/sand";
        }

        public override BlockFace[] getFaces() { 
            return new BlockFace[] {
                BlockFace.rightFace,
            };
        }
    }
}
