using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Game.World.Blocks
{
    class SandBlock : VanillaBlock {
        public SandBlock() {
            this.mName = "Sand";
            this.mMaterial = "cube/sand";
            this.mItemTexture = "sand.jpg";
            this.mId = 6;
        }

    }
}
