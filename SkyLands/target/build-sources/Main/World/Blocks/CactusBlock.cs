using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Game.World.Blocks
{
    class CactusBlock : VanillaBlock
    {
        public CactusBlock() {
            this.mName = "Cactus";
            this.mMaterial = "cube/cactus";
            this.mId = 8;
        }
    }
}
