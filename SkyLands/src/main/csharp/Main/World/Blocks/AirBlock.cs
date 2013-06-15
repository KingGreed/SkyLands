using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Game.World.Blocks
{
    public class AirBlock : VanillaBlock, Air
    {
        public AirBlock() {
            this.mName = "Air";
            this.mMaterial = null;
        }

        public override bool onLeftClick() { return false; }
        public override bool onRightClick() { return false; }
    }
}
