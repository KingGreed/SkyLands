using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Game.World.Display
{
    class StaticRectangle
    {
        public static void DisplayRectangle(Vector3 origin, int height, int width, int depth, SceneNode node) {
            ManualObject block = new ManualObject(Guid.NewGuid().ToString());
            block.Begin("cube/Sand", RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            Vector2[] textureCoord = 
                new Vector2[] {
                    new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0),
                    new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 1),
                    new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0),
                    new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0),
                    new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 1),
                    new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0)
                };

            Vector3[] blockPointCoords = 
            new Vector3[] {
                new Vector3(0, height, 0),            new Vector3(0, 0, 0),                       new Vector3(width, 0, 0),                new Vector3(width, height, 0),
                new Vector3(0, 0, -depth),            new Vector3(0, height, -depth),             new Vector3(width, height, -depth),      new Vector3(width, 0, -depth),
                new Vector3(width, height, 0),        new Vector3(width, height, -depth),         new Vector3(0, height, -depth),          new Vector3(0, height, 0),
                new Vector3(0, 0, 0),                 new Vector3(0, 0, -depth),                  new Vector3(width, 0, -depth),           new Vector3(width, 0, 0),
                new Vector3(0, 0, 0),                 new Vector3(0, height, 0),                  new Vector3(0, height, -depth),          new Vector3(0, 0, -depth),
                new Vector3(width, 0, 0),             new Vector3(width, 0, -depth),              new Vector3(width, height, -depth),      new Vector3(width, height, 0),
            };
            int a = 0;
            for(int i = 0; i < 6; i++) {
                for(int j = 0; j < 4; j++) {
                    block.Position(origin + blockPointCoords[j * 4 + i]); block.TextureCoord(textureCoord[j * 4 + i]);
                    a++;
                }
                block.Quad((uint)a-4, (uint)a-3, (uint)a-2, (uint)a-1);
            }

            block.End();
            node.AttachObject(block);
        }
    }
}
