using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Game.World.Blocks
{
    class CrystalBlock : VanillaBlock
    {
        public CrystalBlock() {
            this.mName = "Crystal";
            this.mMaterial = "cube/Crystal";
            this.mId = 14;
            this.mMeshType = 1;
        }

    }
}
