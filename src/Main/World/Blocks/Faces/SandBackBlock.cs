using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generic;

using Mogre;

namespace Game.World.Blocks
{
    class SandBackBlock : VanillaBlock
    {
        public SandBackBlock() {
            this.mName = "Sand back";
            this.mMaterial = "cube/sand";
        }

        public override BlockFace[] getFaces() { 
            return new BlockFace[] {
                BlockFace.backFace,
            };
        }
    }
}
