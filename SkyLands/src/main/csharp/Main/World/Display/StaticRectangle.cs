using System;
using Mogre;

namespace Game.World.Display
{
    static class StaticRectangle
    {
        static StaticRectangle() {
            /* For drawing lines */
            String resourceGroupName = "debugger";
            if (ResourceGroupManager.Singleton.ResourceGroupExists(resourceGroupName) == false)
                ResourceGroupManager.Singleton.CreateResourceGroup(resourceGroupName);

            MaterialPtr moMaterial2 = MaterialManager.Singleton.Create("line_material_red", resourceGroupName);
            moMaterial2.ReceiveShadows = false;
            moMaterial2.GetTechnique(0).SetLightingEnabled(true);
            moMaterial2.GetTechnique(0).GetPass(0).SetDiffuse(1, 0, 0, 0);
            moMaterial2.GetTechnique(0).GetPass(0).SetAmbient(1, 0, 0);
            moMaterial2.GetTechnique(0).GetPass(0).SetSelfIllumination(1, 0, 0);
            moMaterial2.Dispose();  // dispose pointer, not the material

            MaterialPtr moMaterial3 = MaterialManager.Singleton.Create("line_material_green", resourceGroupName);
            moMaterial3.ReceiveShadows = false;
            moMaterial3.GetTechnique(0).SetLightingEnabled(true);
            moMaterial3.GetTechnique(0).GetPass(0).SetDiffuse(0, 1, 0, 0);
            moMaterial3.GetTechnique(0).GetPass(0).SetAmbient(0, 1, 0);
            moMaterial3.GetTechnique(0).GetPass(0).SetSelfIllumination(0, 1, 0);
            moMaterial3.Dispose();  // dispose pointer, not the material

            MaterialPtr moMaterial = MaterialManager.Singleton.Create("line_material_blue", resourceGroupName);
            moMaterial.ReceiveShadows = false;
            moMaterial.GetTechnique(0).SetLightingEnabled(true);
            moMaterial.GetTechnique(0).GetPass(0).SetDiffuse(0, 0, 1, 0);
            moMaterial.GetTechnique(0).GetPass(0).SetAmbient(0, 0, 1);
            moMaterial.GetTechnique(0).GetPass(0).SetSelfIllumination(0, 0, 1);
            moMaterial.Dispose();  // dispose pointer, not the material
        }
        
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

        public static ManualObject CreateLine(SceneManager sceneMgr, Vector3 p1, Vector3 p2, string material = "line_material_blue")
        {
            ManualObject manOb = sceneMgr.CreateManualObject();
            manOb.Begin(material, RenderOperation.OperationTypes.OT_LINE_LIST);
            manOb.Position(p1);
            manOb.Position(p2);
            manOb.End();

            return manOb;
        }

        public static SceneNode DrawLine(SceneManager sceneMgr, Vector3 p1, Vector3 p2, string material = "line_material_blue")
        {
            SceneNode node = sceneMgr.RootSceneNode.CreateChildSceneNode();
            node.Position = p1;
            node.AttachObject(CreateLine(sceneMgr, Vector3.ZERO, p2 - p1, material));
            return node;
        }
    }
}
