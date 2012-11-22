using Mogre;
using Mogre.TutorialFramework;
using System;

using Game.Land;
using Game.CharacSystem;

namespace Game
{
    class Program : BaseApplication
    {
        public static void Main()
        {
            new Program().Go();
        }

        protected override void CreateScene()
        {
            this.generateFace();
        }

        private void generateFace(){

            string defaultMaterial = "Cube";
            ManualObject block = new ManualObject("cubeObject");

            block.Begin(defaultMaterial, RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            block.Position(new Vector3(0, 0, 0));                              block.TextureCoord(1, 1);
            block.Position(new Vector3(0, World.CUBE_SIDE, 0));                block.TextureCoord(1, 0);
            block.Position(new Vector3(World.CUBE_SIDE, World.CUBE_SIDE, 0));  block.TextureCoord(0, 0);
            block.Position(new Vector3(World.CUBE_SIDE, 0, 0));                block.TextureCoord(0, 1);

            block.Quad(3, 2, 1, 0);
            block.End();
        }
    }
}