using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.World.Blocks {
    class LevitatorBlock : VanillaBlock {
        public LevitatorBlock() {
            this.mName = "Levitator";
            this.mMaterial = "";
            this.mItemTexture = "LevitatorTop.png";
            this.mId = 16;
        }

        public override string getFace(int i) {
            switch (i) {
                case 0: return "cube/Levitator/side";
                case 1: return "cube/Levitator/side";
                case 2: return "cube/Levitator/top";
                case 3: return "cube/Levitator/bottom";
                case 4: return "cube/Levitator/side";
                case 5: return "cube/Levitator/side";
            }
            return "";
        }
    }
}
