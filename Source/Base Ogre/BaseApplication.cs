using Mogre;
using System;



namespace Game.BaseApp
{
    class ShutdownException : Exception { }
    
    public abstract partial class BaseApplication
    {
        protected Root         mRoot;
        protected SceneManager mSceneMgr;
        protected Camera       mCamera;
        protected CameraMan    mCameraMan;
        protected RenderWindow mWindow;
        protected string       mPluginsCfg   = "plugins.cfg";
        protected string       mResourcesCfg = "resources.cfg";
        protected bool         mShutDown     = false;
        protected int          mTextureMode  = 0;
        protected int          mRenderMode   = 0;
        protected Overlay      mDebugOverlay;



        public void Go()
        {
            try
            {
                if (!this.Setup()) { return; }
                this.mRoot.StartRendering();
                this.DestroyScene();
            }
            catch (System.Runtime.InteropServices.SEHException e)
            {
                Console.WriteLine(e);

                System.Windows.Forms.MessageBox.Show(
                    "An Ogre error has occurred. Check the Ogre.log file for details", "Exception",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                System.Windows.Forms.MessageBox.Show(
                    e.Message, "Error",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        protected virtual bool Setup()
        {
            this.mRoot = new Root(mPluginsCfg);

            if (!this.Configure())
                return false;

            this.ChooseSceneManager();
            this.CreateCamera();
            this.CreateViewports();

            TextureManager.Singleton.DefaultNumMipmaps = 5;

            this.CreateResourceListener();
            this.LoadResources();

            this.CreateScene();

            this.CreateFrameListeners();
            this.InitializeInput();

            this.mDebugOverlay = new Overlay(mWindow);
            this.mDebugOverlay.AdditionalInfo = "Bilinear";

            return true;
        }

        protected virtual bool Configure()
        {
            if (this.mRoot.ShowConfigDialog()) { this.mWindow = this.mRoot.Initialise(true, "TutorialApplication Render Window"); return true; }
            else { return false; }
        }

        protected virtual void ChooseSceneManager() { this.mSceneMgr = this.mRoot.CreateSceneManager(SceneType.ST_GENERIC); }

        protected virtual void CreateCamera()
        {
            this.mCamera = this.mSceneMgr.CreateCamera("PlayerCam");

            this.mCamera.Position = new Vector3(0, 100, 250);

            this.mCamera.LookAt(new Vector3(0, 50, 0));
            this.mCamera.NearClipDistance = 5;

            this.mCameraMan = new CameraMan(mCamera);
        }

        protected virtual void CreateViewports()
        {
            // Create one viewport, entire window
            var vp = mWindow.AddViewport(mCamera);
            vp.BackgroundColour = ColourValue.Black;

            // Alter the camera aspect ratio to match the viewport
            this.mCamera.AspectRatio = (vp.ActualWidth / vp.ActualHeight);
        }

        protected virtual void CreateResourceListener() {}

        protected virtual void CreateScene(){}

        protected virtual void LoadResources()
        {
            // Load resource paths from config file
            var cf = new ConfigFile();
            cf.Load(mResourcesCfg, "\t:=", true);

            // Go through all sections & settings in the file
            var seci = cf.GetSectionIterator();
            while (seci.MoveNext())
            {
                foreach (var pair in seci.Current)
                {
                    ResourceGroupManager.Singleton.AddResourceLocation(
                        pair.Value, pair.Key, seci.CurrentKey);
                }
            }

            ResourceGroupManager.Singleton.InitialiseAllResourceGroups();
        }

        protected void ReloadAllTextures() { TextureManager.Singleton.ReloadAll(); }

        protected virtual void UpdateScene(FrameEvent evt)
        {
            this.mSceneMgr.GetEntity("MainPlayer").GetAnimationState("IdleBase").AddTime(evt.timeSinceLastEvent);
            this.mSceneMgr.GetEntity("MainPlayer").GetAnimationState("IdleTop").AddTime(evt.timeSinceLastEvent);
        }

        protected void CycleTextureFilteringMode()
        {
            this.mTextureMode = (this.mTextureMode + 1) % 4;
            switch (this.mTextureMode)
            {
                case 0:
                    MaterialManager.Singleton.SetDefaultTextureFiltering(TextureFilterOptions.TFO_BILINEAR);
                    this.mDebugOverlay.AdditionalInfo = "BiLinear";
                    break;

                case 1:
                    MaterialManager.Singleton.SetDefaultTextureFiltering(TextureFilterOptions.TFO_TRILINEAR);
                    this.mDebugOverlay.AdditionalInfo = "TriLinear";
                    break;

                case 2:
                    MaterialManager.Singleton.SetDefaultTextureFiltering(TextureFilterOptions.TFO_ANISOTROPIC);
                    MaterialManager.Singleton.DefaultAnisotropy = 8;
                    this.mDebugOverlay.AdditionalInfo = "Anisotropic";
                    break;

                case 3:
                    MaterialManager.Singleton.SetDefaultTextureFiltering(TextureFilterOptions.TFO_NONE);
                    MaterialManager.Singleton.DefaultAnisotropy = 1;
                    this.mDebugOverlay.AdditionalInfo = "None";
                    break;
            }
        }

        protected void CyclePolygonMode()
        {
            this.mRenderMode = (this.mRenderMode + 1) % 3;
            switch (mRenderMode)
            {
                case 0: mCamera.PolygonMode = PolygonMode.PM_SOLID;     break;
                case 1: mCamera.PolygonMode = PolygonMode.PM_WIREFRAME; break;
                case 2: mCamera.PolygonMode = PolygonMode.PM_POINTS;    break;
            }
        }

        protected void TakeScreenshot() { mWindow.WriteContentsToTimestampedFile("screenshot", ".png"); }

        protected virtual void CreateFrameListeners() { this.mRoot.FrameRenderingQueued += new FrameListener.FrameRenderingQueuedHandler(OnFrameRenderingQueued); }

        protected virtual bool OnFrameRenderingQueued(FrameEvent evt)
        {
            if (this.mWindow.IsClosed) { return false; }
            if (this.mShutDown)        { return false; }

            try
            {
                this.ProcessInput();
                this.UpdateScene(evt);

                this.mCameraMan.UpdateCamera(evt.timeSinceLastFrame);
                this.mDebugOverlay.Update(evt.timeSinceLastFrame);
                return true;
            }
            catch (ShutdownException) { this.mShutDown = true; return false; }
        }

        protected void Shutdown() { throw new ShutdownException(); }

        protected virtual void DestroyScene() {}
    }
}