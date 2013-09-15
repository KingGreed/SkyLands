using System;
using System.Collections.Generic;
using API.Generic;

using Mogre;

namespace API.Geo.Cuboid
{
    public abstract class MultiBlock {

        protected string mMaterial;
        protected string mName;
        protected Island mIsland;
        protected World  mWorld;

        protected ManualObject block;
        protected SceneNode node;

        public static int CUBE_SIDE = Cst.CUBE_SIDE;

        private int faceNumber;

        public int meshType;
        
        public MultiBlock(string mat, Island current, World mainWorld, int meshType) {

            this.mMaterial = mat;
            this.mName = this.mMaterial + Guid.NewGuid().ToString();

            this.mIsland = current;
            this.mWorld = mainWorld;

            block = new ManualObject("MultiBlock-" + this.mName);
            block.Begin(this.mMaterial, RenderOperation.OperationTypes.OT_TRIANGLE_LIST);
            this.meshType = meshType;
        }

        /*
         * @param the block's location
         */
        public void addBlock(Vector3 loc, BlockFace face) {
            Vector3 displayCoord = this.mWorld.getDisplayCoords(this.mIsland.getPosition(), loc);

            for(int i = 0; i < 4; i++) {
                block.Position(displayCoord + getBlockPointsCoord((int)face * 4 + i)); 
                block.TextureCoord(getTextureCoord((int)face * 4 + i));
                block.Normal(this.getNormals((int)face));
                this.faceNumber++;
            }
            block.Quad((uint)this.faceNumber - 4, (uint)this.faceNumber - 3, (uint)this.faceNumber - 2, (uint)this.faceNumber - 1);
        }


        public abstract Vector3 getBlockPointsCoord(int face);
        public abstract Vector2 getTextureCoord(int face);
        public abstract Vector3 getNormals(int face);

        public void display(Island currentIsland, API.Geo.World currentWorld) {
            if (this.faceNumber > 0) {
                block.End();
                this.node = this.mIsland.Node.CreateChildSceneNode("MultiBlockNode-" + this.mName);
                this.node.AttachObject(block);
            }
        }

        public void Remove() {
            if (node != null) {
                this.node.RemoveAndDestroyAllChildren();
                this.mIsland.mWorld.getSceneMgr().DestroySceneNode(node);
            }
        }

        public string getMaterial() { return this.mMaterial; }

        public string getName()          { return this.mName; }
        public void setName(string name) { this.mName = name; }
    }
}
