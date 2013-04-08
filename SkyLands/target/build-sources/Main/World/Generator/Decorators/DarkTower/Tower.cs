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
    interface Tower {
        void build(Island current, Random rd, Vector3 loc);
    }
}
