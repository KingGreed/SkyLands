using Mogre;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generic;
using Game.World;

namespace Game.Display
{
    public static class GraphicBlock
    {
        public static string getFaceName(BlockFace name) {
            if(name == BlockFace.frontFace)      { return "frontFace"; }
            else if(name == BlockFace.backFace)  { return "backFace" ; }
            else if(name == BlockFace.rightFace) { return "rightFace"; } 
            else if(name == BlockFace.leftFace)  { return "leftFace" ; }
            else if(name == BlockFace.underFace) { return "underFace"; }
            else /*name == CubeFace.upperFace*/  { return "upperFace"; }
        }

        public static void generateFace()
        {
            string defaultMaterial = "Default";

            ManualObject block = new ManualObject("frontFace");
            block.Begin(defaultMaterial, RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            block.Position(new Vector3(0, 0, 0)); block.TextureCoord(1, 1);
            block.Position(new Vector3(0, MainWorld.CUBE_SIDE, 0)); block.TextureCoord(1, 0);
            block.Position(new Vector3(MainWorld.CUBE_SIDE, MainWorld.CUBE_SIDE, 0)); block.TextureCoord(0, 0);
            block.Position(new Vector3(MainWorld.CUBE_SIDE, 0, 0)); block.TextureCoord(0, 1);

            block.Quad(3, 2, 1, 0);
            block.End();
            block.ConvertToMesh("frontFace");


            block = new ManualObject("underFace");
            block.Begin(defaultMaterial, RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            block.Position(new Vector3(0, 0, 0)); block.TextureCoord(1, 1);
            block.Position(new Vector3(0, 0, -MainWorld.CUBE_SIDE)); block.TextureCoord(1, 0);
            block.Position(new Vector3(MainWorld.CUBE_SIDE, 0, -MainWorld.CUBE_SIDE)); block.TextureCoord(0, 0);
            block.Position(new Vector3(MainWorld.CUBE_SIDE, 0, 0)); block.TextureCoord(0, 1);

            block.Quad(0, 1, 2, 3);
            block.End();
            block.ConvertToMesh("underFace");


            block = new ManualObject("rightFace");
            block.Begin(defaultMaterial, RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            block.Position(new Vector3(MainWorld.CUBE_SIDE, 0, 0)); block.TextureCoord(0, 1);
            block.Position(new Vector3(MainWorld.CUBE_SIDE, 0, -MainWorld.CUBE_SIDE)); block.TextureCoord(1, 1);
            block.Position(new Vector3(MainWorld.CUBE_SIDE, MainWorld.CUBE_SIDE, -MainWorld.CUBE_SIDE)); block.TextureCoord(1, 0);
            block.Position(new Vector3(MainWorld.CUBE_SIDE, MainWorld.CUBE_SIDE, 0)); block.TextureCoord(0, 0);

            block.Quad(0, 1, 2, 3);
            block.End();
            block.ConvertToMesh("rightFace");


            block = new ManualObject("upperFace");
            block.Begin(defaultMaterial, RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            block.Position(new Vector3(MainWorld.CUBE_SIDE, MainWorld.CUBE_SIDE, 0)); block.TextureCoord(1, 1);
            block.Position(new Vector3(MainWorld.CUBE_SIDE, MainWorld.CUBE_SIDE, -MainWorld.CUBE_SIDE)); block.TextureCoord(1, 0);
            block.Position(new Vector3(0, MainWorld.CUBE_SIDE, -MainWorld.CUBE_SIDE)); block.TextureCoord(0, 0);
            block.Position(new Vector3(0, MainWorld.CUBE_SIDE, 0)); block.TextureCoord(0, 1);

            block.Quad(0, 1, 2, 3);
            block.End();
            block.ConvertToMesh("upperFace");


            block = new ManualObject("leftFace");
            block.Begin(defaultMaterial, RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            block.Position(new Vector3(0, 0, 0)); block.TextureCoord(1, 1);
            block.Position(new Vector3(0, MainWorld.CUBE_SIDE, 0)); block.TextureCoord(1, 0);
            block.Position(new Vector3(0, MainWorld.CUBE_SIDE, -MainWorld.CUBE_SIDE)); block.TextureCoord(0, 0);
            block.Position(new Vector3(0, 0, -MainWorld.CUBE_SIDE)); block.TextureCoord(0, 1);

            block.Quad(0, 1, 2, 3);
            block.End();
            block.ConvertToMesh("leftFace");


            block = new ManualObject("backFace");
            block.Begin(defaultMaterial, RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            block.Position(new Vector3(0, 0, -MainWorld.CUBE_SIDE)); block.TextureCoord(1, 1);
            block.Position(new Vector3(0, MainWorld.CUBE_SIDE, -MainWorld.CUBE_SIDE)); block.TextureCoord(1, 0);
            block.Position(new Vector3(MainWorld.CUBE_SIDE, MainWorld.CUBE_SIDE, -MainWorld.CUBE_SIDE)); block.TextureCoord(0, 0);
            block.Position(new Vector3(MainWorld.CUBE_SIDE, 0, -MainWorld.CUBE_SIDE)); block.TextureCoord(0, 1);

            block.Quad(0, 1, 2, 3);
            block.End();
            block.ConvertToMesh("backFace");


        }
    }
}
