using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Game.World.Blocks
{
    class AirBlock : VanillaBlock
    {
        public AirBlock() {
            this.mName = "Air";
            this.mMaterial = null;
        }
    }
}
