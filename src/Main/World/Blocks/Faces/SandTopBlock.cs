using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generic;

using Mogre;

namespace Game.World.Blocks
{
    class SandTopBlock : VanillaBlock
    {
        public SandTopBlock() {
            this.mName = "Sand top";
            this.mMaterial = "cube/sand";
        }

        public override BlockFace[] getFaces() { 
            return new BlockFace[] {
                BlockFace.upperFace,
            };
        }
    }
}
