using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Game.World.Blocks
{
    class DirtBlock : VanillaBlock
    {
        public DirtBlock() {
            this.mName = "Dirt";
            this.mMaterial = "cube/dirt";
        }
    }
}
