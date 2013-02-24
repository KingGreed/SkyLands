using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generic;

using Mogre;

namespace Game.World.Blocks
{
    class GrassTopBlock : VanillaBlock
    {
        public GrassTopBlock() {
            this.mName = "Grass top";
            this.mMaterial = "cube/grass/top";
        }

        public override BlockFace[] getFaces() { return new BlockFace[] { BlockFace.upperFace }; }
    }
}
