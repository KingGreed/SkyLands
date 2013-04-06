using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generator;
using API.Geo.Cuboid;

using Game.World.Blocks;
using Game.World.Generator.Decorators.DarkTowerPopulator;

using Mogre;

namespace Game.World.Generator.Decorators {
    class DarkTower : Decorator {

        public override void populate(Island curr, Random random) {
            Vector3 pos = this.findRandomPoint(curr, random);
            MainTower t = new MainTower(1);
            t.makeTower(curr, random, pos);
        }
    }
}
