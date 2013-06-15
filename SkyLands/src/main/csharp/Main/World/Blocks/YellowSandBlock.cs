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
            this.mItemTexture = "sand2.png";
            this.mId = 9;
        }

    }
}
