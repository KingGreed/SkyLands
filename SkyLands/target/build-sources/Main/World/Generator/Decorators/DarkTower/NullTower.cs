using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generator;
using API.Generic;

using API.Geo.Cuboid;

using Game.World.Blocks;

using Mogre;


namespace Game.World.Generator.Decorators.DarkTowerPopulator {
    class NullTower : Tower {
        public void build(Island current, Random rd, Vector3 loc) {
            //does not build anything
        }
    }
}
