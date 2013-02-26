using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Game.World.Blocks
{
    class RedSandBlock : VanillaBlock {
        public RedSandBlock() {
            this.mName = "Red Sand";
            this.mMaterial = "cube/RedSand";
            this.mId = 10;
        }

        public override string[] getComposingFaces()
        { //Must be placed in the blockFace order
            return new string[6] { "RedSand front", "RedSand back", "RedSand top", "RedSand bottom", "RedSand left", "RedSand right" };
        }
    }
}
