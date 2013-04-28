using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Mogre;

namespace Game.World.Blocks {
    class Glass : VanillaBlock, TransparentBlock {
        public Glass() {

            this.mName = "Glass";
            this.mMaterial = "cube/glass";
            this.mId = 22;
        }

    }
}
