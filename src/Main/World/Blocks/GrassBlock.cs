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
            this.mMaterial = "wdc";
            this.mId = 1;
        }

        public override string[] getComposingFaces() { //Must be placed in the blockFace order
            return new string[6] { "Grass front", "Grass back", "Grass top", "Grass bottom" , "Grass left", "Grass right"};
        }
    }
}
