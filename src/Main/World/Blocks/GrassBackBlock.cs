using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generic;

using Mogre;

namespace Game.World.Blocks
{
    class GrassBackBlock : VanillaBlock
    {
        public GrassBackBlock() {
            this.mName = "Grass back";
            this.mMaterial = "cube/grass/side";
        }

        public override BlockFace[] getFaces() { 
            return new BlockFace[] {
                BlockFace.backFace
            };
        }
    }
}
