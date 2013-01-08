using Mogre;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Display
{
    public static class GraphicBlock
    {
        public static string getFaceName(CubeFace name) {
            if(name == CubeFace.frontFace)      { return "frontFace"; }
            else if(name == CubeFace.backFace)  { return "backFace" ; }
            else if(name == CubeFace.rightFace) { return "rightFace"; } 
            else if(name == CubeFace.leftFace)  { return "leftFace" ; }
            else if(name == CubeFace.underFace) { return "underFace"; }
            else /*name == CubeFace.upperFace*/ { return "upperFace"; }
        }

        public static void generateFace()
        {
            string defaultMaterial = "Default";

            ManualObject block = new ManualObject("frontFace");
            block.Begin(defaultMaterial, RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            block.Position(new Vector3(0, 0, 0)); block.TextureCoord(1, 1);
            block.Position(new Vector3(0, World.CUBE_SIDE, 0)); block.TextureCoord(1, 0);
            block.Position(new Vector3(World.CUBE_SIDE, World.CUBE_SIDE, 0)); block.TextureCoord(0, 0);
            block.Position(new Vector3(World.CUBE_SIDE, 0, 0)); block.TextureCoord(0, 1);

            block.Quad(3, 2, 1, 0);
            block.End();
            block.ConvertToMesh("frontFace");


            block = new ManualObject("underFace");
            block.Begin(defaultMaterial, RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            block.Position(new Vector3(0, 0, 0)); block.TextureCoord(1, 1);
            block.Position(new Vector3(0, 0, -World.CUBE_SIDE)); block.TextureCoord(1, 0);
            block.Position(new Vector3(World.CUBE_SIDE, 0, -World.CUBE_SIDE)); block.TextureCoord(0, 0);
            block.Position(new Vector3(World.CUBE_SIDE, 0, 0)); block.TextureCoord(0, 1);

            block.Quad(0, 1, 2, 3);
            block.End();
            block.ConvertToMesh("underFace");


            block = new ManualObject("rightFace");
            block.Begin(defaultMaterial, RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            block.Position(new Vector3(World.CUBE_SIDE, 0, 0)); block.TextureCoord(0, 1);
            block.Position(new Vector3(World.CUBE_SIDE, 0, -World.CUBE_SIDE)); block.TextureCoord(1, 1);
            block.Position(new Vector3(World.CUBE_SIDE, World.CUBE_SIDE, -World.CUBE_SIDE)); block.TextureCoord(1, 0);
            block.Position(new Vector3(World.CUBE_SIDE, World.CUBE_SIDE, 0)); block.TextureCoord(0, 0);

            block.Quad(0, 1, 2, 3);
            block.End();
            block.ConvertToMesh("rightFace");


            block = new ManualObject("upperFace");
            block.Begin(defaultMaterial, RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            block.Position(new Vector3(World.CUBE_SIDE, World.CUBE_SIDE, 0)); block.TextureCoord(1, 1);
            block.Position(new Vector3(World.CUBE_SIDE, World.CUBE_SIDE, -World.CUBE_SIDE)); block.TextureCoord(1, 0);
            block.Position(new Vector3(0, World.CUBE_SIDE, -World.CUBE_SIDE)); block.TextureCoord(0, 0);
            block.Position(new Vector3(0, World.CUBE_SIDE, 0)); block.TextureCoord(0, 1);

            block.Quad(0, 1, 2, 3);
            block.End();
            block.ConvertToMesh("upperFace");


            block = new ManualObject("leftFace");
            block.Begin(defaultMaterial, RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            block.Position(new Vector3(0, 0, 0)); block.TextureCoord(1, 1);
            block.Position(new Vector3(0, World.CUBE_SIDE, 0)); block.TextureCoord(1, 0);
            block.Position(new Vector3(0, World.CUBE_SIDE, -World.CUBE_SIDE)); block.TextureCoord(0, 0);
            block.Position(new Vector3(0, 0, -World.CUBE_SIDE)); block.TextureCoord(0, 1);

            block.Quad(0, 1, 2, 3);
            block.End();
            block.ConvertToMesh("leftFace");


            block = new ManualObject("backFace");
            block.Begin(defaultMaterial, RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            block.Position(new Vector3(0, 0, -World.CUBE_SIDE)); block.TextureCoord(1, 1);
            block.Position(new Vector3(0, World.CUBE_SIDE, -World.CUBE_SIDE)); block.TextureCoord(1, 0);
            block.Position(new Vector3(World.CUBE_SIDE, World.CUBE_SIDE, -World.CUBE_SIDE)); block.TextureCoord(0, 0);
            block.Position(new Vector3(World.CUBE_SIDE, 0, -World.CUBE_SIDE)); block.TextureCoord(0, 1);

            block.Quad(0, 1, 2, 3);
            block.End();
            block.ConvertToMesh("backFace");


        }
    }
}
