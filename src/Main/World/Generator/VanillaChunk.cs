using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using API.Geo.Cuboid;
using API.Generator;

using Game.World.Blocks;

using Mogre;

namespace Game.World.Generator
{
    class VanillaChunk : Chunk
    {
        public static Dictionary<string, Block> staticBlock;

        static VanillaChunk() {
            staticBlock = new Dictionary<string, Block>();
            Type[] blockList = GetTypesInNamespace(Assembly.GetExecutingAssembly(), "Game.World.Blocks");
            for (int i = 0; i < blockList.Length; i++) {

                if(typeof(Block).IsAssignableFrom(blockList[i]) && !blockList[i].IsAbstract) {
                    
                    ConstructorInfo ctor = blockList[i].GetConstructor(new Type[0]);
                    Block test = (Block) ctor.Invoke(new object[0]);

                    if(!staticBlock.ContainsKey(test.getName())) {
                        staticBlock.Add(test.getName(), test);
                    } else {
                        throw new Exception("Block : " + blockList[i].Name + " has a name (" + test.getName() + ") which already exists"); 
                    }
                    LogManager.Singleton.DefaultLog.LogMessage("New Block type : " + blockList[i].Name);
                    LogManager.Singleton.DefaultLog.LogMessage("    New Block has texture : " + test.getMaterial());
                }
            }
        }
        public static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace) {
            return assembly.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
        }

        public VanillaChunk(Vector3 chunkSize, Vector3 location, Island island) : base(chunkSize, location, island) {
            //LogManager.Singleton.DefaultLog.LogMessage("Space : " + GC.GetTotalMemory(false));

            this.mBlockList = new Block[16, 16, 16];
            for(int x = 0; x < mChunkSize.x; x++) {
                for(int y = 0; y < mChunkSize.y; y++) {
                    for(int z = 0; z < mChunkSize.z; z++) {
                        this.mBlockList[x, y, z] = new AirBlock();
                    }
                }
            }
        }

        public override void   setBlock(int x, int y, int z, string material) { this.mBlockList[x, y, z] = staticBlock[material]; }
        public override string getBlockMaterial(int x, int y, int z)          { return this.mBlockList[x, y, z].getMaterial();    }

    }
}
