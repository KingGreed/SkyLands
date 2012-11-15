using System;
using System.Collections.Generic;
using Mogre;

namespace Game.Land
{
    public enum BlockFace { TOP, DOWN, FRONT, BEHIND, RIGHT, LEFT } // If you change this order, you have to modify GenerateBlock as well
    public enum TypeBlock : byte { AIR, METAL } // Metal is just here for the test
    
    public class Block
    {
        private TypeBlock mType;// { get { return mType; } private set { mType = value; UpdateMaterial(); } }
        private SceneNode mNode   { get; public set; }
        private Vector3 mBlockPos { get; public set; }


        public Block(Vector3 blockPos, TypeBlock blockType = TypeBlock.AIR)
        {
            this.mType = blockType;
        }

        public bool hasSceneNode() {
            return (this.mNode != null);
        }

        public bool IsAir() { return (mType == 0); }
        


        /* Return whether the block needs to update the visibility of its neighbor or not */
        internal bool SetType(TypeBlock type)
        {
            bool result = (this.mType == TypeBlock.AIR && type != TypeBlock.AIR) || (this.mType != TypeBlock.AIR && type == TypeBlock.AIR);
            mType = type;
            return result;
        }
    }
}