using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generic;

using Mogre;

namespace Game.World.Blocks
{
    class ConstructionTopBlock : VanillaBlock
    {
        public ConstructionTopBlock() {
            this.mName = "Construction top";
            this.mMaterial = "cube/construction/top";
        }

        public override BlockFace[] getFaces() { return new BlockFace[] { BlockFace.upperFace }; }
    }
}
