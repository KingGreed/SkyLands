using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Game.World.Blocks
{
    class DarkWoodBlock : VanillaBlock
    {
        public DarkWoodBlock() {
            this.mName = "DarkWood";
            this.mMaterial = "darkTower/darkWood";
            this.mId = 11;
        }
    }
}
