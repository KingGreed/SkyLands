using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Game.World.Blocks
{
    class WoodBlock : VanillaBlock
    {
        public WoodBlock() {
            this.mName = "Wood";
            this.mMaterial = "cube/wood";
            this.mId = 4;
        }
    }
}
