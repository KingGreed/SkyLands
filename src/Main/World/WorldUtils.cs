using System;
using System.Collections.Generic;
using Mogre;

using Game.Terrain;
using Game.Material;

namespace Game
{
    public partial class World {

        public static Vector3 getBlockRelativeCoord(Vector3 blockPos) {
            blockPos.x = (int)blockPos.x % CHUNK_SIDE;
            blockPos.y = (int)blockPos.y % CHUNK_SIDE;
            blockPos.z = (int)blockPos.z % CHUNK_SIDE;

            if(blockPos.x < 0) { blockPos.x += 16; }
            if(blockPos.z < 0) { blockPos.z += 16; }

            return blockPos;
        }

        public static Vector3 getChunkRelativeCoord(Vector3 blockPos) {

            Vector3 chunkPos = Vector3.ZERO;
            
            chunkPos.x = (int)blockPos.x / CHUNK_SIDE;
            chunkPos.y = (int)blockPos.y / CHUNK_SIDE;
            chunkPos.z = (int)blockPos.z / CHUNK_SIDE;

            blockPos.x %= CHUNK_SIDE;
            blockPos.y %= CHUNK_SIDE;
            blockPos.z %= CHUNK_SIDE;

            if(blockPos.x < 0) { chunkPos.x--; }
            if(blockPos.z < 0) { chunkPos.z--; }

            return chunkPos;
        }

         public static Block getBlock(Vector3 chunkPos, Vector3 blockPos) {
            
             Vector3 blockCoord = (chunkPos * CHUNK_SIDE) + blockPos;

            chunkPos = getChunkRelativeCoord(blockCoord);
            blockPos = getBlockRelativeCoord(blockCoord);

            if(!hasChunk(chunkPos)) { return null; }

            return chunkArray[chunkPos].BlockArray[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z];
        }

        public static Chunk getChunk(Vector3 chunkPos, Vector3 blockPos){

            Vector3 blockCoord = chunkPos * CHUNK_SIDE + blockPos;

            chunkPos = getChunkRelativeCoord(blockCoord);

            if(!hasChunk(chunkPos)) { return null; }
            return chunkArray[chunkPos];

        }

        public static bool hasChunk(Vector3 chunkPos){ return chunkArray.ContainsKey(chunkPos); }

        public static void getBlockAndChunkPosFromAbsolute(Vector3 blockAbs, out Vector3 chunkPos, out Vector3 blockPos) {
            blockAbs /= World.CUBE_SIDE;
            chunkPos = World.getChunkRelativeCoord(blockAbs);
            blockPos = World.getBlockRelativeCoord(blockAbs);
        }


        //HitBox for collision is the distance between the player's center of gravity and the actual side you want to test
        public static bool hasCollision(Vector3 blockPos, CubeFace collisionSide)
        {
            Vector3 chunkPos;

            if      (collisionSide == CubeFace.leftFace)    { blockPos.x--; }
            else if (collisionSide == CubeFace.rightFace)   { blockPos.x++; }
            else if (collisionSide == CubeFace.frontFace)   { blockPos.z++; }
            else if (collisionSide == CubeFace.backFace)    { blockPos.z--; }
            else if (collisionSide == CubeFace.underFace)   { blockPos.y--; }
            else  /*(collisionSide == CubeFace.upperFace)*/ { blockPos.y++; }

            World.getBlockAndChunkPosFromAbsolute(blockPos, out chunkPos, out blockPos);

            Block block = World.getBlock(chunkPos, blockPos);
            if (block != null && !block.IsAir()) { return true; }

            return false;
        }


        public static Vector3 getSafeSpawn() {
            return Vector3.ZERO;
        }

    }
}
