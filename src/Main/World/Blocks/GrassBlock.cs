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
            this.mMaterial = "cube/wood";
            this.mId = 1;
        }

        public override string[] getComposingFaces() {
            return new string[6] { "Grass left", "Grass right", "Grass back", "Grass front", "Grass bottom", "Grass top" };
        }
    }
}
