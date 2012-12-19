using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Game.Terrain;
using Game.Material;
using Mogre;

namespace Game
{
    public partial class World {

        public static Vector3 getBlocCoord(Vector3 chunkPos, Vector3 blockPos){
            int[] chunkPosArray = new int[3] { (int)chunkPos.x, (int)chunkPos.y, (int)chunkPos.z };
            int[] blockPosArray = new int[3] { (int)blockPos.x, (int)blockPos.y, (int)blockPos.z };
            for (int i = 0; i < 3; i++) {
                chunkPosArray[i] += blockPosArray[i] / CHUNK_SIDE;  // Division entière
                blockPosArray[i] %= CHUNK_SIDE;

                while (blockPosArray[i] < 0) { chunkPosArray[i]--; blockPosArray[i] += CHUNK_SIDE; }
            }
            return new Vector3(blockPosArray[0], blockPosArray[1], blockPosArray[2]);
        }

        public static Vector3 getChunkCoord(Vector3 chunkPos, Vector3 blockPos){
            int[] chunkPosArray = new int[3] { (int)chunkPos.x, (int)chunkPos.y, (int)chunkPos.z };
            int[] blockPosArray = new int[3] { (int)blockPos.x, (int)blockPos.y, (int)blockPos.z };
            for (int i = 0; i < 3; i++) {
                chunkPosArray[i] += blockPosArray[i] / CHUNK_SIDE;  // Division entière
                blockPosArray[i] %= CHUNK_SIDE;

                while (blockPosArray[i] < 0) { chunkPosArray[i]--; blockPosArray[i] += CHUNK_SIDE; }
            }
            return new Vector3(chunkPosArray[0], chunkPosArray[1], chunkPosArray[2]);
        }


         public static Block getBlock(Vector3 chunkPos, Vector3 blockPos) {
            
            chunkPos = getChunkCoord(chunkPos, blockPos);
            blockPos = getBlocCoord (chunkPos, blockPos);

            //LogManager.Singleton.DefaultLog.LogMessage("chunkPos : " + chunkPos.ToString());
            //LogManager.Singleton.DefaultLog.LogMessage("blockPos : " + blockPos.ToString());

            if(!hasChunk(chunkPos)) { return new Block(new Vector3(0, 0, 0)); }
            
            return chunkArray[chunkPos].mBlockArray[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z];
        }

        public static Chunk getChunk(Vector3 chunkPos, Vector3 blockPos){
            chunkPos = getChunkCoord(chunkPos, blockPos);
            blockPos = getBlocCoord (chunkPos, blockPos);

            if(!hasChunk(chunkPos)) { return null; }
            return chunkArray[chunkPos];

        }

        public static Chunk getChunkAt(Vector3 chunkPos) {
            if(!chunkArray.ContainsKey(chunkPos)){ return null; }
            return chunkArray[chunkPos];
        }
        public static bool hasChunk(Vector3 chunkPos){ return chunkArray.ContainsKey(chunkPos); }
    }
}
