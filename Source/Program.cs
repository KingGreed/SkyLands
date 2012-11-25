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

        [STAThread]
        static void Main()
        {
            mEngine = new OgreManager();
            mStateMgr = new StateManager(mEngine);

            NameValuePairList param = new NameValuePairList();
            param["vsync"] = "true";
            WindowInfo windowInfo = new WindowInfo("Islands", 1024, 768, false, param);

            if (mEngine.Startup(windowInfo) && mStateMgr.Startup(typeof(GameState)))
            {
                LogManager.Singleton.DefaultLog.LogMessage("Start rendering");
                while (!mEngine.Window.IsClosed)    // Render loop
                {
                    mStateMgr.Update(1 / mEngine.Window.LastFPS);    // Frame time in seconds
                    mEngine.Update();

                    if (mStateMgr.ShuttingDown)
                    {
                        mEngine.Window.Destroy();
                        LogManager.Singleton.DefaultLog.LogMessage("Stopped rendering");
                    }
                }
            }

            mStateMgr.Shutdown();

            mEngine.Shutdown();
        }
    }
}