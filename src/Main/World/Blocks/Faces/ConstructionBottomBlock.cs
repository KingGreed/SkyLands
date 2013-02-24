using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generic;

using Mogre;

namespace Game.World.Blocks
{
    class ConstructionBottomBlock : VanillaBlock
    {
        public ConstructionBottomBlock() {
            this.mName = "Construction bottom";
            this.mMaterial = "cube/construction/bottom";
        }

        public override BlockFace[] getFaces() { return new BlockFace[] { BlockFace.underFace }; }

    }
}
