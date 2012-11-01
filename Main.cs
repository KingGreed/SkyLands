using Mogre;
using Mogre.TutorialFramework;
using System;



namespace Game
{
    class App : BaseApplication
    {
        public static void Main()
        {
            new App().Go();
        }

        protected override void CreateScene()
        {
            this.generateBlock();
            Entity ent = mSceneMgr.CreateEntity("Head", "blockMesh");
            ent.GetSubEntity(0).SetMaterialName("Cube");
            ent.GetSubEntity(1).SetMaterialName("Cube2");

            SceneNode node = mSceneMgr.RootSceneNode.CreateChildSceneNode("HeadNode");
            node.AttachObject(ent);
        }

        private void generateBlock()
        {
            ManualObject block = new ManualObject("front");

            block.Begin("", RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            block.Position(new Vector3(0, 0, 0)); block.TextureCoord(1, 1);
            block.Position(new Vector3(0, 100, 0)); block.TextureCoord(1, 0);
            block.Position(new Vector3(100, 100, 0)); block.TextureCoord(0, 0);
            block.Position(new Vector3(100, 0, 0)); block.TextureCoord(0, 1);

            block.Quad(3, 2, 1, 0);
            block.End();

            block.Begin("", RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            block.Position(new Vector3(0, 0, -100)); block.TextureCoord(1, 1);
            block.Position(new Vector3(0, 100, -100)); block.TextureCoord(1, 0);
            block.Position(new Vector3(100, 100, -100)); block.TextureCoord(0, 0);
            block.Position(new Vector3(100, 0, -100)); block.TextureCoord(0, 1);

            block.Quad(0, 1, 2, 3);
            block.End();

            block.Begin("Cube", RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            block.Position(new Vector3(0, 0, 0)); block.TextureCoord(1, 1);
            block.Position(new Vector3(0, 100, 0)); block.TextureCoord(1, 0);
            block.Position(new Vector3(0, 100, -100)); block.TextureCoord(0, 0);
            block.Position(new Vector3(0, 0, -100)); block.TextureCoord(0, 1);

            block.Quad(0, 1, 2, 3);
            block.End();

            block.Begin("Cube", RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            block.Position(new Vector3(100, 0, 0)); block.TextureCoord(1, 1);
            block.Position(new Vector3(100, 100, 0)); block.TextureCoord(1, 0);
            block.Position(new Vector3(100, 100, -100)); block.TextureCoord(0, 0);
            block.Position(new Vector3(100, 0, -100)); block.TextureCoord(0, 1);

            block.Quad(3, 2, 1, 0);
            block.End();

            block.Begin("Cube", RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            block.Position(new Vector3(100, 100, 0)); block.TextureCoord(1, 1);
            block.Position(new Vector3(100, 100, -100)); block.TextureCoord(1, 0);
            block.Position(new Vector3(0, 100, -100)); block.TextureCoord(0, 0);
            block.Position(new Vector3(0, 100, 0)); block.TextureCoord(0, 1);

            block.Quad(0, 1, 2, 3);
            block.End();


            block.ConvertToMesh("blockMesh");
        }
    }
}