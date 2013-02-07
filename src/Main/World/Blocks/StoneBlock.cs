using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Game.World.Blocks
{
    class StoneBlock : VanillaBlock
    {
        public StoneBlock() {
            this.mName = "Stone";
            this.mMaterial = "cube/stone";
            this.mId = 3;
        }
    }
}
