using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Game.World.Blocks
{
    class TowerWoodBlock : VanillaBlock
    {
        public TowerWoodBlock() {
            this.mName = "TowerWoodBlock";
            this.mMaterial = "darkTower/towerWood";
            this.mItemTexture = "towerWood.jpg";
            this.mId = 12;
        }
    }
}
