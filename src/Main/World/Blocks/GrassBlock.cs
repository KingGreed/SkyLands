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
        }

        public override string[] getComposingFaces() {
            return new string[3] { "Grass side", "Grass bottom", "Grass top" };
        }
    }
}
