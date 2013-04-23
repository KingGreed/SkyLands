using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

using API.Generator;
using API.Geo.Cuboid;

using Game.World.Blocks;
using Game.World;

namespace Game.World.Generator.Decorators {
    class Portals : Decorator {
        public Portals() { }

        public override void populate(Island curr, Random random) {
            this.makePortal(1, curr, this.findRandomPoint(curr, random));
        }

        private void makePortal(int val, Island current, Vector3 position) {
            switch(val) {
                case 1:
                    for(int x = 0; x < 4; x++) {
                        for(int y = -1; y < 5; y++) {
                            if(x == 0 || x == 3 || y == -1 || y == 4) { current.setBlockAt((int)position.x + x, (int)position.y + y, (int)position.z, "Crystal", true); }
                            else { current.setBlockAt((int)position.x + x, (int)position.y + y, (int)position.z, "CrystalGate", true); }
                        }
                    }
                break;
                case 2:
                break;

            }
        }
    }
}
