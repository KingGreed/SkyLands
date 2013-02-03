using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Geo.Cuboid;
using API.Generator;
using API.Generic;

using Mogre;

using Game.Display;
using Material = API.Generic.Material;

namespace Game.World.Generator
{
    public class VanillaMultiBlock : MultiBlock
    {
        private List<Vector3> mList;
        private Material      mMaterial;

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

        public VanillaMultiBlock(Material mat) {
            this.mList     = new List<Vector3>();
            this.mMaterial = mat;
        }

        public void addBlock(Vector3 loc) { this.mList.Add(loc); }
        public bool Contains(Vector3 loc) { return this.mList.Contains(loc); }

        public List<Vector3> getBlockList() { return this.mList; }

        public Material getMaterial() { return this.mMaterial; }

        public void display(SceneManager sceneMgr, Island currentIsland, API.Geo.World currentWorld) {
            if(mList.Count == 0) { return; }
            
            Materials mtr = new Materials();
            string material = mtr.getMaterial(((int)currentIsland.getBlock(this.mList[0], false).getMaterial()).ToString());
            LogManager.Singleton.DefaultLog.LogMessage("Material : " + material);
            int faceNumber = 0;
            Block curr;
            var values = Enum.GetValues(typeof(BlockFace));
            Vector2[] textureCoord = 
                new Vector2[] {
                    new Vector2(1, 1),
                    new Vector2(1, 0),
                    new Vector2(0, 0),
                    new Vector2(0, 1)

                };
            Vector3 displayCoord;

            ManualObject block = new ManualObject("MultiBlock-" + mList[0].x + "-" + mList[0].y + "-" + mList[0].z);
            block.Begin(material, RenderOperation.OperationTypes.OT_TRIANGLE_LIST);
                foreach(Vector3 loc in this.mList) {
                    curr = currentIsland.getBlock(loc, false);
                    displayCoord = currentWorld.getDisplayCoords(currentIsland.getPosition(), loc);

                    foreach(BlockFace face in values) {
                        if(curr.hasVisibleFaceAt(face)) {
                            for(int i = 0; i < 4; i++) {
                                block.Position(displayCoord + blockPointCoords[(int)face * 4 + i]); block.TextureCoord(textureCoord[i]);
                                faceNumber++;
                            }
                            block.Quad((uint)faceNumber-4, (uint)faceNumber-3, (uint)faceNumber-2, (uint)faceNumber-1);
                        }
                    }
                }

            block.End();
            block.ConvertToMesh("MultiBlock-" + mList[0].x + "-" + mList[0].y + "-" + mList[0].z);

            SceneNode node = sceneMgr.RootSceneNode.CreateChildSceneNode("MultiBlockNode-" + mList[0].x + "-" + mList[0].y + "-" + mList[0].z);
            node.AttachObject(block);
        }
    }
}
