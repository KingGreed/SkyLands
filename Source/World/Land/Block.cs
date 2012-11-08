using System;
using System.Collections.Generic;
using Mogre;

namespace Game.Land
{
    public enum BlockFace { TOP, DOWN, FRONT, BEHIND, RIGHT, LEFT } // If you change this order, you have to modify GenerateBlock as well
    public enum TypeBlock : byte { AIR, METAL } // Metal is just here for the test
    
    class Block
    {
        private TypeBlock mType;// { get { return mType; } private set { mType = value; UpdateMaterial(); } }
        private SceneNode mNode { get; public set; }

        public bool hasSceneNode() {
            return (this.mNode != null);
        }

        public Block(TypeBlock blockType = TypeBlock.AIR)
        {
            this.mType = blockType;
        }

        public bool IsAir() { return (mType == 0); }
        


        /* Return whether the block needs to update the visibility of its neighbor or not         *
         * That is to say if it was air and become something else or if it becomes air (and wasn't already air) */
        internal bool SetType(TypeBlock type)
        {
            bool result = (mType == TypeBlock.AIR && type != TypeBlock.AIR) || (mType != TypeBlock.AIR && type == TypeBlock.AIR);
            mType = type;
            return result;
        }
    }
}
