using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.World.Blocks {
    class LevitatorAirBlock : VanillaBlock, Air {

        public LevitatorAirBlock() {
            this.mName = "Levitator air";
            this.mMaterial = null;
            this.mId = 17;
        }

        public override void onBlockEnter(API.Ent.Entity e) { e.setIsPushedByArcaneLevitator(true);  }
        public override void onBlockLeave(API.Ent.Entity e) { e.setIsPushedByArcaneLevitator(false); }
    }
}
