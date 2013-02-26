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
            this.mMaterial = "";
            this.mId = 6;
        }

        public override string[] getComposingFaces()
        { //Must be placed in the blockFace order
            return new string[6] { "Sand front", "Sand back", "Sand top", "Sand bottom", "Sand left", "Sand right" };
        }
    }
}
