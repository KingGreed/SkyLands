using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.World.Blocks
{
    class GrassBlock : VanillaBlock
    {
        public GrassBlock() {
            this.mName = "Grass";
            this.mMaterial = "";
            this.mItemTexture = "dirt.jpg";
            this.mId = 1;
        }

        public override string getFace(int i) {
            switch (i) {
                case 0: return "cube/grass/side";
                case 1: return "cube/grass/side";
                case 2: return "cube/grass/top";
                case 3: return "cube/grass/bottom";
                case 4: return "cube/grass/side";
                case 5: return "cube/grass/side";
            }
            return "";
        }
    }
}
