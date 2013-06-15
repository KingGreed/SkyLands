using System;
using System.Collections.Generic;
using API.Generic;

using Mogre;

namespace API.Geo.Cuboid
{
    public abstract class MultiBlock {

        protected List<Vector3> mList;
        protected List<int> mIndexInVertexBuffer;

        protected string mMaterial;
        protected string mName;
        protected Island mIsland;
        protected World  mWorld;

        protected ManualObject block;

        public static int CUBE_SIDE = Cst.CUBE_SIDE;

        protected RenderOperation moData;
        protected VertexElement posEl;
        protected static HardwareVertexBufferSharedPtr vBuff;
        private int faceNumber;
        private Vector3 displayCoord;
        
        
        public MultiBlock(string mat, Island current, World mainWorld) {
            this.mList                = new List<Vector3>();
            this.mIndexInVertexBuffer = new List<int>    ();
            this.mMaterial = mat;
            this.mName = this.mMaterial + Guid.NewGuid().ToString() + "0";

            this.moData = new RenderOperation();
            this.mIsland = current;
            this.mWorld = mainWorld;

            block = new ManualObject("MultiBlock-" + this.mName);
            block.Begin(this.mMaterial, RenderOperation.OperationTypes.OT_TRIANGLE_LIST);
        }

        /*
         * @param the block's location
         */
        public void addBlock(Vector3 loc, BlockFace face) {
            if(this.mList.Count == 0 || (this.mList[this.mList.Count - 1] != loc)) { this.mList.Add(loc); this.mIndexInVertexBuffer.Add(this.faceNumber); }

            displayCoord = this.mWorld.getDisplayCoords(this.mIsland.getPosition(), loc);

            for(int i = 0; i < 4; i++) {
                block.Position(displayCoord + getBlockPointsCoord((int)face * 4 + i)); block.TextureCoord(getTextureCoord((int)face * 4 + i));
                block.Normal(this.getNormals((int)face));
                this.faceNumber++;
            }
            block.Quad((uint)this.faceNumber - 4, (uint)this.faceNumber - 3, (uint)this.faceNumber - 2, (uint)this.faceNumber - 1);
        }


        public abstract Vector3 getBlockPointsCoord(int face);
        public abstract Vector2 getTextureCoord(int face);
        public abstract Vector3 getNormals(int face);

        /*
         * Warning does not remove item from List, only from Scene
         * */
        public void display(Island currentIsland, API.Geo.World currentWorld) {
            if(mList.Count == 0) { return; }
            block.End();
        }
        public void addMultiToScene() {
            if(mList.Count == 0) { return; }
            this.mIsland.Node.CreateChildSceneNode("MultiBlockNode-" + this.mName).AttachObject(block);
        }


        private int find(Vector3 item) {
            int i = 0;
            while(i < this.mList.Count) {
                if(this.mList[i] == item) { return i; }
                i++;
            }
            return -1;
        }

        public unsafe void removeFromScene(Vector3 item, int numFaces) {
            if(vBuff == null && this.block != null) {
                this.block.GetSection(0).GetRenderOperation(this.moData);
                this.posEl = this.moData.vertexData.vertexDeclaration.FindElementBySemantic(VertexElementSemantic.VES_POSITION);
            }

            int elemPosition = this.find(item);

            if(elemPosition == -1) { return; }

            vBuff = this.moData.vertexData.vertexBufferBinding.GetBuffer(posEl.Source);
            byte* pVertex = (byte*)vBuff.Lock(HardwareBuffer.LockOptions.HBL_NORMAL) + vBuff.VertexSize * this.mIndexInVertexBuffer[elemPosition];
            float* pReal;


            for(int i = 0; i < numFaces * 4; i++) {
                posEl.BaseVertexPointerToElement(pVertex, &pReal);

                pReal[0] = 0; pReal[1] = 0; pReal[2] = 0;
                pVertex += vBuff.VertexSize;
            }

            vBuff.Unlock();
        }

        public string getMaterial() { return this.mMaterial; }

        public string getName()          { return this.mName; }
        public void setName(string name) { this.mName = name; }
    }
}
