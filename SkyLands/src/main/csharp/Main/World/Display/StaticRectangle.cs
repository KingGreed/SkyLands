using System;
using Mogre;

namespace Game.World.Display
{
    static class StaticRectangle
    {
        static void DisplayRectangle(Vector3 origin, int height, int width, int depth, SceneNode node) {
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

        public static ManualObject CreateLine(SceneManager sceneMgr, Vector3 p1, Vector3 p2, string material = ColoredMaterials.BLUE)
        {
            ManualObject manOb = sceneMgr.CreateManualObject();
            manOb.Begin(material, RenderOperation.OperationTypes.OT_LINE_LIST);
            manOb.Position(p1);
            manOb.Position(p2);
            manOb.End();

            return manOb;
        }

        public static ManualObject CreateRectangle(SceneManager sceneMgr, Vector3 size, string material = ColoredMaterials.BLUE) { return CreateRectangle(sceneMgr, Vector3.ZERO, size, material); }
        public static ManualObject CreateRectangle(SceneManager sceneMgr, Vector3 pos, Vector3 size, string material = ColoredMaterials.BLUE)
        {
            Vector3[,] points = new Vector3[2,4];
            for (int y = 0; y < 2; y++)
            {
                points[y, 0] = pos + y * Vector3.UNIT_Y * size.y;
                points[y, 1] = pos + new Vector3(1, y, 0) * size;
                points[y, 2] = pos + new Vector3(1, y, 1) * size;
                points[y, 3] = pos + new Vector3(0, y, 1) * size;
            }
            
            ManualObject manOb = sceneMgr.CreateManualObject();
            manOb.Begin(material, RenderOperation.OperationTypes.OT_LINE_LIST);
            for (int y = 0; y < 2; y++)
            {
                for (int i = 0; i < 4; i++)
                {
                    manOb.Position(points[y, i]);
                    manOb.Position(points[y, (i + 1)%4]);
                }
            }

            for (int i = 0; i < 4; i++)
            {
                manOb.Position(points[0, i]);
                manOb.Position(points[1, i]);
            }

            manOb.End();
            return manOb;
        }

        public static SceneNode DrawLine(SceneManager sceneMgr, Vector3 p1, Vector3 p2, string material = ColoredMaterials.BLUE)
        {
            SceneNode node = sceneMgr.RootSceneNode.CreateChildSceneNode();
            node.Position = p1;
            node.AttachObject(CreateLine(sceneMgr, Vector3.ZERO, p2 - p1, material));
            return node;
        }
    }
}
