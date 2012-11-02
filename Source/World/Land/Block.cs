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
        public Entity mEnt { get; private set; }
        public SceneNode mNode { get; private set; }

        public Block(ref SceneManager sceneMgr, SceneNode blockNode, string blockName, int cubeSide, TypeBlock blockType = TypeBlock.AIR)
        {
            mType = blockType;

            string meshName = "-Mesh" + blockName;
            GenerateBlock(meshName, cubeSide);
            mEnt = sceneMgr.CreateEntity(blockName, meshName);

            mNode = blockNode;
            mNode.AttachObject(mEnt);

            UpdateMaterial();
        }

        public bool IsAir() { return (mType == 0); }

        internal void SetFaceVisibility(BlockFace face, bool isVisible)
        {
            mEnt.GetSubEntity((uint)face).SetVisible(isVisible);
        }

        internal bool isFaceVisible(BlockFace face)
        {
            return mEnt.IsVisible() && mEnt.GetSubEntity((uint)face).IsVisible; // When mEnt.IsVisible() is false, its subEntities may be "visible" even if they aren't displayed
        }

        /* Return whether the block needs to update the visibility of its neighbor or not         *
         * That is to say if it was air and become something else or if it becomes air (and wasn't already air) */
        internal bool SetType(TypeBlock type)
        {
            bool result = (mType == TypeBlock.AIR && type != TypeBlock.AIR) || (mType != TypeBlock.AIR && type == TypeBlock.AIR);
            mType = type;
            UpdateMaterial();
            return result;
        }

        private void UpdateMaterial()
        {
            if (mType == TypeBlock.AIR)
                mEnt.Visible = false;
            else
            {
                mEnt.Visible = true;
                switch (mType)
                {
                    case TypeBlock.METAL:
                        mEnt.SetMaterialName("Metal");
                        break;

                    default:
                        break;
                }
            }
        }

        /* The faces of the mesh have to be generated in the same order than enum BlockFace */
        private void GenerateBlock(string name, int cubeSide)
        {
            string defaultMaterial = "Black";
            ManualObject block = new ManualObject("cubeObject");

            /* TOP */
            block.Begin(defaultMaterial, RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            block.Position(new Vector3(cubeSide, cubeSide, 0)); block.TextureCoord(1, 1);
            block.Position(new Vector3(cubeSide, cubeSide, -cubeSide)); block.TextureCoord(1, 0);
            block.Position(new Vector3(0, cubeSide, -cubeSide)); block.TextureCoord(0, 0);
            block.Position(new Vector3(0, cubeSide, 0)); block.TextureCoord(0, 1);

            block.Quad(0, 1, 2, 3);
            block.End();

            /* DOWN */
            block.Begin(defaultMaterial, RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            block.Position(new Vector3(cubeSide, 0, 0)); block.TextureCoord(1, 1);
            block.Position(new Vector3(cubeSide, 0, -cubeSide)); block.TextureCoord(1, 0);
            block.Position(new Vector3(0, 0, -cubeSide)); block.TextureCoord(0, 0);
            block.Position(new Vector3(0, 0, 0)); block.TextureCoord(0, 1);

            block.Quad(3, 2, 1, 0);
            block.End();

            /* FRONT */
            block.Begin(defaultMaterial, RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            block.Position(new Vector3(0, 0, 0)); block.TextureCoord(1, 1);
            block.Position(new Vector3(0, cubeSide, 0)); block.TextureCoord(1, 0);
            block.Position(new Vector3(cubeSide, cubeSide, 0)); block.TextureCoord(0, 0);
            block.Position(new Vector3(cubeSide, 0, 0)); block.TextureCoord(0, 1);

            block.Quad(3, 2, 1, 0);
            block.End();

            /* BEHIND */
            block.Begin(defaultMaterial, RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            block.Position(new Vector3(0, 0, -cubeSide)); block.TextureCoord(1, 1);
            block.Position(new Vector3(0, cubeSide, -cubeSide)); block.TextureCoord(1, 0);
            block.Position(new Vector3(cubeSide, cubeSide, -cubeSide)); block.TextureCoord(0, 0);
            block.Position(new Vector3(cubeSide, 0, -cubeSide)); block.TextureCoord(0, 1);

            block.Quad(0, 1, 2, 3);
            block.End();

            /* RIGHT */
            block.Begin(defaultMaterial, RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            block.Position(new Vector3(cubeSide, 0, 0)); block.TextureCoord(1, 1);
            block.Position(new Vector3(cubeSide, cubeSide, 0)); block.TextureCoord(1, 0);
            block.Position(new Vector3(cubeSide, cubeSide, -cubeSide)); block.TextureCoord(0, 0);
            block.Position(new Vector3(cubeSide, 0, -cubeSide)); block.TextureCoord(0, 1);

            block.Quad(3, 2, 1, 0);
            block.End();

            /* LEFT */
            block.Begin(defaultMaterial, RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            block.Position(new Vector3(0, 0, 0)); block.TextureCoord(1, 1);
            block.Position(new Vector3(0, cubeSide, 0)); block.TextureCoord(1, 0);
            block.Position(new Vector3(0, cubeSide, -cubeSide)); block.TextureCoord(0, 0);
            block.Position(new Vector3(0, 0, -cubeSide)); block.TextureCoord(0, 1);

            block.Quad(0, 1, 2, 3);
            block.End();

            block.ConvertToMesh(name);
        }

        public static BlockFace opposite(BlockFace face)
        {
            switch (face)
            {
                case BlockFace.LEFT:
                    return BlockFace.RIGHT;

                case BlockFace.RIGHT:
                    return BlockFace.LEFT;

                case BlockFace.TOP:
                    return BlockFace.DOWN;

                case BlockFace.DOWN:
                    return BlockFace.TOP;

                case BlockFace.FRONT:
                    return BlockFace.BEHIND;

                default:
                    return BlockFace.FRONT;
            }
        }
    }
}
