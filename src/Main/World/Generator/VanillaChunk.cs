using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Geo.Cuboid;
using API.Generator;

using Mogre;

namespace Game.World.Generator
{
    class VanillaChunk : Chunk
    {

        public VanillaChunk(Vector3 chunkSize, Vector3 location, Island island) : base(chunkSize, location, island) {
            //LogManager.Singleton.DefaultLog.LogMessage("Space : " + GC.GetTotalMemory(false));

            this.mBlockList = new Block[16, 16, 16];
            for(int x = 0; x < mChunkSize.x; x++) {
                for(int y = 0; y < mChunkSize.y; y++) {
                    for(int z = 0; z < mChunkSize.z; z++) {
                        this.mBlockList[x, y, z] = new VanillaBlock(new Vector3(x, y, z), location);
                    }
                }
            }
        }
    }
}
