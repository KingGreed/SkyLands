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
            this.mMaterial = "";
            this.mId = 9;
        }

        public override string[] getComposingFaces() { //Must be placed in the blockFace order
          return new string[6] { "YellowSand front", "YellowSand front", "YellowSand front", "YellowSand front", "YellowSand front", "YellowSand front" };
        }
    }
}
