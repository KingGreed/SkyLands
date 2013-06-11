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
            this.mId = 4;
            this.mItemTexture = "wood.png";
        }

        public override string getFace(int i) {
            switch(i) {
                case 0: return "cube/wood/side";
                case 1: return "cube/wood/side";
                case 2: return "cube/wood/top";
                case 3: return "cube/wood/top";
                case 4: return "cube/wood/side";
                case 5: return "cube/wood/side";
            }
            return "";
        }
    }
}
