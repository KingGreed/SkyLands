using System;
using Mogre;

using Game.CharacSystem;
using Game.States;

namespace Game
{
    public class Program
    {
        private static OgreManager mEngine;
        private static StateManager mStateMgr;

        private const int CUBE_SIDE = 50;

        [STAThread]
        static void Main()
        {
            mEngine = new OgreManager();
            mStateMgr = new StateManager(mEngine);
            Program prg = new Program();

            NameValuePairList param = new NameValuePairList();
            param["vsync"] = "true";
            WindowInfo windowInfo = new WindowInfo("Islands", 1024, 768, false, param);

            if (mEngine.Startup(windowInfo) && mStateMgr.Startup(new GameState()))
            {
                prg.CreateScene();

                while (!mEngine.Window.IsClosed)    // Render loop
                {
                    prg.UpdateScene();
                    mEngine.Update();

                    if (mStateMgr.ShuttingDown)
                    {
                        mEngine.Window.Destroy();
                    }
                }

                prg.RemoveScene();
            }

            mStateMgr.Shutdown();

            mEngine.Shutdown();
        }

        public Program()
        {
        }

        public void CreateScene()
        {
        }

        public void UpdateScene()
        {
            mStateMgr.Update(0);
        }

        public void RemoveScene()
        {
        }

        private void generateFace()
        {
            string defaultMaterial = "Cube";
            ManualObject block = new ManualObject("cubeObject");

            block.Begin(defaultMaterial, RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            block.Position(new Vector3(0, 0, 0)); block.TextureCoord(1, 1);
            block.Position(new Vector3(0, CUBE_SIDE, 0)); block.TextureCoord(1, 0);
            block.Position(new Vector3(CUBE_SIDE, CUBE_SIDE, 0)); block.TextureCoord(0, 0);
            block.Position(new Vector3(CUBE_SIDE, 0, 0)); block.TextureCoord(0, 1);

            block.Quad(3, 2, 1, 0);
            block.End();
        }
    }
}