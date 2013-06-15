using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;

using LibNoise;
using LibNoise.Modules;
using LibNoise.Modules.Source;
using LibNoise.Modules.Modifier;
using LibNoise.Modules.Combiner;

using API.Geo.Cuboid;
using API.Generator;
using API.Generic;

using Game.World.Blocks;

using Mogre;

namespace Game.World.Generator
{
    public class FlatIsland : VanillaIsland {

        public FlatIsland(SceneNode node, Vector2 size, API.Geo.World currentWorld) : base(node, size, currentWorld) {
            this.generate(42);
        }

        public override void generate(int seed) {
            Vector3 chunkTempPosition = new Vector3(0, 0, 0);

            for(int xx = 0; xx < this.mIslandSize.x * Cst.CHUNK_SIDE; xx++) {
                for(int zz = 0; zz < this.mIslandSize.z * Cst.CHUNK_SIDE; zz++) {
                    this.setBlockAt(xx, 2, zz, "Grass", true);

				}
            }
        }

    }
}
