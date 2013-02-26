using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generic;

using Mogre;

namespace Game.World.Blocks
{
    class SandFrontBlock : VanillaBlock
    {
        public SandFrontBlock() {
            this.mName = "Sand front";
            this.mMaterial = "cube/sand";
        }

        public override BlockFace[] getFaces() { 
            return new BlockFace[] {
                BlockFace.frontFace,
            };
        }
    }
}
