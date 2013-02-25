using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Game.World.Blocks
{
    class YellowSandBlock : VanillaBlock {
        public YellowSandBlock() {
            this.mName = "Yellow Sand";
            this.mMaterial = "cube/YellowSand";
            this.mId = 9;
        }
    }
}
