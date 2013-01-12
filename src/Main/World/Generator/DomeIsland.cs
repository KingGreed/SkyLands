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

using Mogre;

namespace Game.World.Generator
{
    class DomeIsland : Island
    {
        public DomeIsland(Vector3 islandCoord, Vector2 size) : base(islandCoord, size) {

            if(size.x != size.y) { throw new ArgumentException("Dome Islands can't be non square Islands"); }

            this.generate();
        }

        public void generate() {
            Vector3 chunkTempPosition = new Vector3(0, 0, 0),
                    blockTempPosition = new Vector3(0, 0, 0);

            for (int xx = 0; xx < this.mIslandSize.x * MainWorld.CHUNK_SIDE; xx++) {
                for (int zz = 0; zz < this.mIslandSize.y * MainWorld.CHUNK_SIDE; zz++) {
                    for (int yy = 0; yy < MainWorld.MaxHeight; yy++) {
                        if(yy <= 100) {                            
                            if(this.isInSphere(xx, yy, zz)) {
                                this.getBlock(xx, yy, zz).setMaterial(API.Generic.Material.GRASS);
                            }
                        }
				    }
                }
            }
        }

        public bool isInSphere(int x, int y, int z) {
            int x0 = (int)this.mIslandSize.x * MainWorld.CHUNK_SIDE / 2,
                y0 = 100, 
                z0 = (int)this.mIslandSize.y * MainWorld.CHUNK_SIDE / 2;

            return (((x-x0) * (x-x0) + (y-y0) * (y-y0) + (z-z0) * (z-z0)) > x0);
        }



    }
}
