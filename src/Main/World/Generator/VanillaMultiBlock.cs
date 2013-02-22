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
        ManualObject block;

        public static int CUBE_SIDE = MainWorld.CUBE_SIDE;

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
        }

        public void addBlock(Vector3 loc) { this.mList.Add(loc); }

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

            Block curr = currIsland.getBlock(item, false);
            if(curr is AirBlock) { return; }

            RenderOperation moData = new RenderOperation();
            this.block.GetSection(0).GetRenderOperation(moData);

            int elemPosition = this.find(item);
            

            VertexElement posEl = moData.vertexData.vertexDeclaration.FindElementBySemantic(VertexElementSemantic.VES_POSITION);
            HardwareVertexBufferSharedPtr vBuff = moData.vertexData.vertexBufferBinding.GetBuffer(posEl.Source);
            
            uint size = VertexElement.GetTypeSize(VertexElementType.VET_FLOAT3);
 
            byte*  pVertex = (byte*)vBuff.Lock(HardwareBuffer.LockOptions.HBL_NORMAL) + vBuff.VertexSize * this.mIndexInVertexBuffer[elemPosition];
            float* pReal;

            if(curr.getComposingFaces().Length > 1) {
                for(int i = 0; i < 4; i++) {
                    posEl.BaseVertexPointerToElement(pVertex, &pReal);
                    
                    pReal[0] = 0; pReal[1] = 0; pReal[2] = 0;
                    pVertex += vBuff.VertexSize;
                }
            } else {
                foreach(BlockFace face in Enum.GetValues(typeof(BlockFace))) {
                    if(currIsland.hasVisiblefaceAt((int)item.x, (int)item.y, (int)item.z, face)) {
                        for(int i = 0; i < 4; i++) { 
                            posEl.BaseVertexPointerToElement(pVertex, &pReal);
                    
                            pReal[0] = 0; pReal[1] = 0; pReal[2] = 0;
                            pVertex += vBuff.VertexSize;
                        }
                    }
                }
            }
            
            vBuff.Unlock();
            vBuff.Dispose();
        }

        public string getMaterial() { return this.mMaterial; }

        public void display(Island currentIsland, API.Geo.World currentWorld) {
            if(mList.Count == 0) { return; }
            

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

            block = new ManualObject("MultiBlock-" + this.mMaterial);
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

            currentIsland.Node.CreateChildSceneNode("MultiBlockNode-" + this.mMaterial).AttachObject(block);
        }
    }
}
