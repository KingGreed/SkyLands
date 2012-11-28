using Mogre;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Display
{
    static class GraphicBlock
    {
        public enum blockFace { 
            frontFace,
            backFace,
            upperFace,
            underFace,
            leftFace,
            rightFace
        };
        public static string getFaceName(blockFace name) {
            if(name == blockFace.frontFace)      { return "frontFace"; }
            else if(name == blockFace.backFace)  { return "backFace" ; }
            else if(name == blockFace.rightFace) { return "rightFace"; } 
            else if(name == blockFace.leftFace)  { return "leftFace" ; }
            else if(name == blockFace.underFace) { return "underFace"; }
            else /*name == blockFace.upperFace*/ { return "upperFace"; }
        }

        public static void generateFace()
        {
            string defaultMaterial = "Cube";

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

            block.Position(new Vector3(World.CUBE_SIDE, 0, 0)); block.TextureCoord(1, 1);
            block.Position(new Vector3(World.CUBE_SIDE, 0, -World.CUBE_SIDE)); block.TextureCoord(1, 0);
            block.Position(new Vector3(World.CUBE_SIDE, World.CUBE_SIDE, -World.CUBE_SIDE)); block.TextureCoord(0, 0);
            block.Position(new Vector3(World.CUBE_SIDE, World.CUBE_SIDE, 0)); block.TextureCoord(0, 1);

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
