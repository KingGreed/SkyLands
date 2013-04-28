using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Geo.Cuboid;

using Mogre;

namespace Game.World.Blocks {
    class LevitatorAirBlock : VanillaBlock, Air {

        public LevitatorAirBlock() {
            this.mName = "Levitator air";
            this.mMaterial = null;
            this.mId = 17;
        }

        public override void onBlockEnter(API.Ent.Entity e, Vector3 position) {
                e.setIsPushedByArcaneLevitator(true);
        }
        public override void onBlockLeave(API.Ent.Entity e, Vector3 position) {
            if(!(e.getIsland().getBlock(position + Vector3.UNIT_Y, false) is LevitatorAirBlock)) {
                e.setIsPushedByArcaneLevitator(false);
            }
        }
    }
}
