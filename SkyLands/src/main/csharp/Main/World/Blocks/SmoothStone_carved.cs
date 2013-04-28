using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Game.World.Blocks
{
    class SmoothStone_Carved : VanillaBlock
    {
        public SmoothStone_Carved() {
            this.mName = "SmoothStone_Carved";
            this.mMaterial = "cube/smoothStone_carved";
            this.mId = 21;
        }

    }
}
