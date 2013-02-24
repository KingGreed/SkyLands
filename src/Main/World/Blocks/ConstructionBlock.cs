using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.World.Blocks
{
    class ConstructionBlock : VanillaBlock
    {
        public ConstructionBlock()
        {
            this.mName = "Construction";
            this.mMaterial = "osef";
            this.mId = 7;
        }

        public override string[] getComposingFaces()
        { //Must be placed in the blockFace order
            return new string[6] { "Construction front", "Construction back", "Construction top", "Construction bottom", "Construction left", "Construction right" };
        }
    }
}
