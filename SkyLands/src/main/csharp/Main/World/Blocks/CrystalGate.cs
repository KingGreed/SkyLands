using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Mogre;

namespace Game.World.Blocks {
    class CrystalGate : VanillaBlock, Air, TransparentBlock {
        public CrystalGate() {

            string[] s = new string[]{ "red", "green", "blue" };

            this.mName = "CrystalGate";
            this.mMaterial = "cube/CrystalGate/" + s[new Random().Next(0, 3)];
            this.mId = 19;
            this.mMeshType = 1;
        }
    }
}
