using System;
using System.Collections.Generic;
using Mogre;

namespace Game.Terrain
{
    public enum TypeBlock : byte { AIR, METAL }
    
    public class Block
    {
        private TypeBlock mType;// { get { return mType; } private set { mType = value; UpdateMaterial(); } }
        private SceneNode mNode   { get; set; }
        //public Vector3 mBlockPos { get; set; }
        private Vector3 mBlockPos;


        public Vector3 blockPos
        {
            get { return this.mBlockPos; }
        }
        public Vector3 setBlockPos
        {
            set { this.mBlockPos = value; }
        }


        public Block(Vector3 blockPos, TypeBlock blockType = TypeBlock.METAL)
        {
            this.mType = blockType;
            this.mBlockPos = blockPos;
        }

        public bool hasSceneNode() {
            return (this.mNode != null);
        }

        public bool IsAir() { return (mType == TypeBlock.AIR); }
        


        /* Return whether the block needs to update the visibility of its neighbor or not */
        internal bool SetType(TypeBlock type)
        {
            bool result = (this.mType == TypeBlock.AIR && type != TypeBlock.AIR) || (this.mType != TypeBlock.AIR && type == TypeBlock.AIR);
            mType = type;
            return result;
        }

        static public void generateFace()
        {
            string defaultMaterial = "Cube";
            ManualObject block = new ManualObject("cubeObject");

            block.Begin(defaultMaterial, RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            block.Position(new Vector3(0, 0, 0)); block.TextureCoord(1, 1);
            block.Position(new Vector3(0, World.CUBE_SIDE, 0)); block.TextureCoord(1, 0);
            block.Position(new Vector3(World.CUBE_SIDE, World.CUBE_SIDE, 0)); block.TextureCoord(0, 0);
            block.Position(new Vector3(World.CUBE_SIDE, 0, 0)); block.TextureCoord(0, 1);

            block.Quad(3, 2, 1, 0);
            block.End();
        }
    }
}