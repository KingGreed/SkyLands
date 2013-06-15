using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Game.World.Blocks
{
    public class PlanksBlock : VanillaBlock {
        public PlanksBlock()
        {
            this.mName = "Planks";
            this.mMaterial = "cube/planks";
            this.mItemTexture = "planks.png";
            this.mId = 13;
        }
    }
}
