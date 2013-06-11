using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Game.World.Blocks
{
    class SmoothStone : VanillaBlock
    {
        public SmoothStone() {
            this.mName = "SmoothStone";
            this.mMaterial = "cube/smoothStone";
            this.mItemTexture = "stonebricksmooth.png";
            this.mId = 20;
        }

    }
}
