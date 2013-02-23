using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Geo.Cuboid;
using API.Generator;
using API.Generic;

using Mogre;

using Game.Display;
using Game.World.Blocks;
using Material = API.Generic.Material;

namespace Game.World.Generator
{
    public class VanillaMultiBlock : MultiBlock
    {
        private List<Vector3> mList;
        private List<int>     mIndexInVertexBuffer;

        private string        mMaterial;
        private string        mName;
        private Island        mIsland;

        ManualObject block;

        public static int CUBE_SIDE = MainWorld.CUBE_SIDE;

        private RenderOperation moData;
        private VertexElement posEl;
        private HardwareVertexBufferSharedPtr vBuff;

        private uint size = VertexElement.GetTypeSize(VertexElementType.VET_FLOAT3);


        //see API.generic.BlockFace
        public static Vector3[] blockPointCoords = 
            new Vector3[] {
                new Vector3(0, CUBE_SIDE, 0),         new Vector3(0, 0, 0),                          new Vector3(CUBE_SIDE, 0, 0),                  new Vector3(CUBE_SIDE, CUBE_SIDE, 0),
                new Vector3(0, 0, -CUBE_SIDE),        new Vector3(0, CUBE_SIDE, -CUBE_SIDE),         new Vector3(CUBE_SIDE, CUBE_SIDE, -CUBE_SIDE), new Vector3(CUBE_SIDE, 0, -CUBE_SIDE),
                new Vector3(CUBE_SIDE, CUBE_SIDE, 0), new Vector3(CUBE_SIDE, CUBE_SIDE, -CUBE_SIDE), new Vector3(0, CUBE_SIDE, -CUBE_SIDE),         new Vector3(0, CUBE_SIDE, 0),
                new Vector3(0, 0, 0),                 new Vector3(0, 0, -CUBE_SIDE),                 new Vector3(CUBE_SIDE, 0, -CUBE_SIDE),         new Vector3(CUBE_SIDE, 0, 0),
                new Vector3(0, 0, 0),                 new Vector3(0, CUBE_SIDE, 0),                  new Vector3(0, CUBE_SIDE, -CUBE_SIDE),         new Vector3(0, 0, -CUBE_SIDE),
                new Vector3(CUBE_SIDE, 0, 0),         new Vector3(CUBE_SIDE, 0, -CUBE_SIDE),         new Vector3(CUBE_SIDE, CUBE_SIDE, -CUBE_SIDE), new Vector3(CUBE_SIDE, CUBE_SIDE, 0),
            };

        public VanillaMultiBlock(string mat) {
            this.mList                = new List<Vector3>();
            this.mIndexInVertexBuffer = new List<int>    ();
            this.mMaterial = mat;
            this.mName = this.mMaterial + Guid.NewGuid().ToString() + "0";

            this.moData = new RenderOperation();
        }

        public void regen() {
            this.mIndexInVertexBuffer = new List<int>    ();

        }

        public void addBlock(Vector3 loc)  { this.mList.Add(loc); }
        public string getName()            { return this.mName;   }
        public void   setName(string name) { this.mName = name;   }

        private int find(Vector3 item) {
            int i = 0;
            while(i < this.mList.Count) {
                if(this.mList[i] == item) { 
                    return i;
                }
                i++;
            }
            throw new ArgumentException("Could not find item");
        }

        public unsafe void removeFromScene(Vector3 item, Island currIsland) {
            if(this.vBuff == null) {
                this.block.GetSection(0).GetRenderOperation(this.moData);
                this.posEl = this.moData.vertexData.vertexDeclaration.FindElementBySemantic(VertexElementSemantic.VES_POSITION);
            }
            Block curr = currIsland.getBlock(item, false);

            int elemPosition = this.find(item);
            int i = 0;

            if(curr.getComposingFaces().Length > 1) {
                if(!currIsland.isinBlocksAdded(item, VanillaChunk.staticBlock[this.mMaterial].getFaces()[0])) {
                    this.removeFace(this.mIndexInVertexBuffer[elemPosition]);
                } else {
                    string cubeNodeName = "Node-" + item.x * MainWorld.CUBE_SIDE + "-" + item.y * MainWorld.CUBE_SIDE + "-" + item.z * MainWorld.CUBE_SIDE ;

                    currIsland.Node.GetChild(0).RemoveChild(cubeNodeName);
                }
            } else {
                foreach(BlockFace face in Enum.GetValues(typeof(BlockFace))) {
                    if(currIsland.hasVisiblefaceAt((int)item.x, (int)item.y, (int)item.z, face)) {
                        if(!currIsland.isinBlocksAdded(item, VanillaChunk.staticBlock[this.mMaterial].getFaces()[0])) {
                            this.removeFace(this.mIndexInVertexBuffer[elemPosition] + i*4);
                        }  else {
                            string cubeNodeName = "Node-" + item.x * MainWorld.CUBE_SIDE + "-" + item.y * MainWorld.CUBE_SIDE + "-" + item.z * MainWorld.CUBE_SIDE ;
                            currIsland.Node.GetChild(0).RemoveChild(cubeNodeName);
                        }
                        i++;
                    }
                }
            }
            

        }
        private unsafe void removeFace(int pos) {

            this.vBuff = this.moData.vertexData.vertexBufferBinding.GetBuffer(posEl.Source);
            byte*  pVertex = (byte*)vBuff.Lock(HardwareBuffer.LockOptions.HBL_NORMAL) + vBuff.VertexSize * pos;
            float* pReal;

            for(int i = 0; i < 4; i++) {
                posEl.BaseVertexPointerToElement(pVertex, &pReal);
                    
                pReal[0] = 0; pReal[1] = 0; pReal[2] = 0;
                pVertex += vBuff.VertexSize;
            }

            vBuff.Unlock();
            this.vBuff.Dispose();
        }

        public string getMaterial() { return this.mMaterial; }

        public void display(Island currentIsland, API.Geo.World currentWorld) {
            if(mList.Count == 0) { return; }
            this.mIsland = currentIsland;

            string material = currentIsland.getMaterialFromName(this.mMaterial);
            int faceNumber = 0;
            Block curr = VanillaChunk.staticBlock[this.mMaterial];

            Vector2[] textureCoord = 
                new Vector2[] {
                    new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0),
                    new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 1),
                    new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0),
                    new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0),
                    new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 1),
                    new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0)
                };
            Vector3 displayCoord;

            block = new ManualObject("MultiBlock-" + this.mName);
            block.Begin(material, RenderOperation.OperationTypes.OT_TRIANGLE_LIST);
                foreach(Vector3 loc in this.mList) {
                    displayCoord = currentWorld.getDisplayCoords(currentIsland.getPosition(), loc);
                    this.mIndexInVertexBuffer.Add(faceNumber);

                    foreach(BlockFace face in curr.getFaces()) {
                        if(currentIsland.hasVisiblefaceAt((int) loc.x, (int) loc.y, (int) loc.z, face)) {
                            for(int i = 0; i < 4; i++) {
                                block.Position(displayCoord + blockPointCoords[(int)face * 4 + i]); block.TextureCoord(textureCoord[(int)face * 4 + i]);
                                faceNumber++;
                            }
                            block.Quad((uint)faceNumber-4, (uint)faceNumber-3, (uint)faceNumber-2, (uint)faceNumber-1);
                        }
                    }
                }

            block.End();
        }
        public void addMultiToScene() {
            if(mList.Count == 0) { return; }
            this.mIsland.Node.CreateChildSceneNode("MultiBlockNode-" + this.mName).AttachObject(block);
        }
    }
}
