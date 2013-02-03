using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generic;

using Mogre;

namespace Game.World.Blocks
{
    class GrassBottomBlock : VanillaBlock
    {
        public GrassBottomBlock() {
            this.mName = "Grass bottom";
            this.mMaterial = "cube/grass/bottom";
        }

        public override BlockFace[] getFaces() { return new BlockFace[] { BlockFace.underFace }; }

    }
}
