using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Game
{
    class BlockFactory {
        public static int CUBE_SIDE = 80;
        public static Vector3[] blockPointCoords =
            new Vector3[] {
                new Vector3(0, CUBE_SIDE, 0),                  new Vector3(0, 0, 0),                  new Vector3(CUBE_SIDE, 0, 0),                  new Vector3(CUBE_SIDE, CUBE_SIDE, 0),  new Vector3(0, CUBE_SIDE, 0),
                new Vector3(0, 0, 0),                          new Vector3(0, 0, -CUBE_SIDE),         new Vector3(0, CUBE_SIDE, -CUBE_SIDE),         new Vector3(0, CUBE_SIDE, 0),          new Vector3(0, CUBE_SIDE, -CUBE_SIDE),
                new Vector3(CUBE_SIDE, CUBE_SIDE, -CUBE_SIDE), new Vector3(CUBE_SIDE, 0, -CUBE_SIDE), new Vector3(0, 0, -CUBE_SIDE),                 new Vector3(CUBE_SIDE, 0, -CUBE_SIDE), new Vector3(CUBE_SIDE, 0, 0),
                new Vector3(CUBE_SIDE, CUBE_SIDE, 0),          new Vector3(CUBE_SIDE, CUBE_SIDE, -CUBE_SIDE),
            };


        public static SceneNode block(Vector3 loc, SceneManager sceneMgr) {
            ManualObject block = new ManualObject("block-" + Guid.NewGuid().ToString());

            block.Begin("BaseWhiteNoLighting", RenderOperation.OperationTypes.OT_LINE_LIST);

            for (int i = 0; i < blockPointCoords.Length; i++) {
                block.Position(loc + blockPointCoords[i]);
                if(i != 0) {
                    block.Position(loc + blockPointCoords[i]);
                }
            }

            block.End();

            SceneNode returnNode = sceneMgr.RootSceneNode.CreateChildSceneNode("cubeBorders", new Vector3(0, 0, 0));

            returnNode.AttachObject(block);
            return returnNode;

        }
    }
}
