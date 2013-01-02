﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Game.Terrain;
using Game.Material;
using Mogre;
using Game.Display;

namespace Game
{
    public partial class World {



        public static Vector3 getBlockRelativeCoord(Vector3 blockPos) {
            blockPos.x %= CHUNK_SIDE; blockPos.y %= CHUNK_SIDE; blockPos.z %= CHUNK_SIDE;
            return blockPos;
        }

        public static Vector3 getChunkRelativeCoord(Vector3 blockPos) {
            blockPos.x = (int)blockPos.x / CHUNK_SIDE; blockPos.y = (int)blockPos.y / CHUNK_SIDE; blockPos.z = (int)blockPos.z / CHUNK_SIDE;
            return blockPos;
        }

         public static Block getBlock(Vector3 chunkPos, Vector3 blockPos) {
            
             Vector3 blockCoord = (chunkPos * CHUNK_SIDE) + blockPos;

            chunkPos = getChunkRelativeCoord(blockCoord);
            blockPos = getBlockRelativeCoord(blockCoord);

            if(!hasChunk(chunkPos)) { return null; }

            if (blockPos.x < 0 || blockPos.y < 0 || blockPos.z < 0 || blockPos.x > World.CHUNK_SIDE || blockPos.y > World.CHUNK_SIDE || blockPos.z > World.CHUNK_SIDE)
                return null;
            else
                return chunkArray[chunkPos].BlockArray[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z];
        }

        public static Chunk getChunk(Vector3 chunkPos, Vector3 blockPos){

            Vector3 blockCoord = chunkPos * CHUNK_SIDE + blockPos;

            chunkPos = getChunkRelativeCoord(blockCoord);

            if(!hasChunk(chunkPos)) { return null; }
            return chunkArray[chunkPos];

        }

        public static bool hasChunk(Vector3 chunkPos){ return chunkArray.ContainsKey(chunkPos); }

        public static void getBlockAndChunkPosFromAbsolute(Vector3 blockAbs, ref Vector3 chunkPos, ref Vector3 blockPos) {
            chunkPos = World.getChunkRelativeCoord(blockAbs);
            blockPos = World.getBlockRelativeCoord(blockAbs);
        }


        //HitBox for collision is the distance between the player's center of gravity and the actual side you wanna test

        public static bool hasCollision(Vector3 playerPos, float hitBoxForCollision, GraphicBlock.blockFace collisionSide) {
            
            Vector3 chunkPos = Vector3.ZERO, blockPos = playerPos;

            if(collisionSide == GraphicBlock.blockFace.leftFace)  { blockPos.x -= hitBoxForCollision; }
            if(collisionSide == GraphicBlock.blockFace.rightFace) { blockPos.x += hitBoxForCollision; }
            if(collisionSide == GraphicBlock.blockFace.frontFace) { blockPos.z += hitBoxForCollision; }
            if(collisionSide == GraphicBlock.blockFace.backFace)  { blockPos.z -= hitBoxForCollision; }
            if(collisionSide == GraphicBlock.blockFace.underFace) { blockPos.y -= hitBoxForCollision; }
            if(collisionSide == GraphicBlock.blockFace.upperFace) { blockPos.y += hitBoxForCollision; }

            World.getBlockAndChunkPosFromAbsolute(blockPos, ref chunkPos, ref blockPos);

            Block block = World.getBlock(chunkPos, blockPos);
            if (block != null && block.IsAir()) { return true; }

            return false;
        }


    }
}
