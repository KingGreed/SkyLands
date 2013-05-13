using System;
using System.Collections.Generic;
using Mogre;

namespace Game.BaseApp
{
    class ShutdownException : Exception {}
    
    public abstract class BaseApplication : SdkTrayListener
    {
        private const string PLUGINS_CFG = "plugins.cfg";
        private const string RESOURCES_CFG = "resources.cfg";

        protected Root mRoot;
        protected SceneManager  mSceneMgr;
        protected RenderWindow  mWindow;
        protected MoisManager   mInput;
        protected Camera        mCam;
        protected Viewport      mViewport;
        protected bool          mIsShutDownRequested = false;
        private int             mRenderMode          = 0;
        private SdkTrayManager  mTrayMgr;
        private ParamsPanel     mPanel;

        //public bool OverlayVisibility        { get { return this.mDebugOverlay.Visibility; } set { this.mDebugOverlay.Visibility = value; } }

        public void Go()
        {
            //try
            {
                if (!this.Setup()) { return; }
                this.mRoot.StartRendering();
                this.Shutdown();
            }
            /*catch (System.Runtime.InteropServices.SEHException e)
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
            }*/
        }

        private bool Setup()
        {
            this.mRoot = new Root(PLUGINS_CFG);

            if (!this.Configure())
                return false;

            this.ChooseSceneManager();
            this.CreateCamera();
            this.CreateViewports();

            TextureManager.Singleton.DefaultNumMipmaps = 5;

            this.LoadResources();

            this.mInput = new MoisManager();
            int windowHnd;
            this.mWindow.GetCustomAttribute("WINDOW", out windowHnd);
            this.mInput.Startup(windowHnd, this.mWindow.Width, this.mWindow.Height);

            this.mTrayMgr = new SdkTrayManager("trayMgr", this.mWindow, this.mInput.Mouse, this);
            this.mTrayMgr.showFrameStats(TrayLocation.TL_BOTTOMLEFT);
            this.mTrayMgr.showLogo(TrayLocation.TL_BOTTOMRIGHT);
            this.mTrayMgr.hideCursor();

            this.mPanel = this.mTrayMgr.createParamsPanel(TrayLocation.TL_NONE, "DetailsPanel", 200, new string[] { "cam.pX", "cam.pY", "cam.pZ","cam.oW", "cam.oX", "cam.oY", "cam.oZ", "Filtering", "Poly Mode" });
            this.mPanel.setParamValue(7, "Bilinear");
            this.mPanel.setParamValue(8, "Solid");
            this.mPanel.hide();
            MaterialManager.Singleton.SetDefaultTextureFiltering(TextureFilterOptions.TFO_NONE);

            this.Create();
            this.AddFrameLstn(new RootLstn(RootLstn.TypeLstn.FrameRendering, this.OnFrameRendering));
            return true;
        }

        private bool Configure()
        {
            /*RenderSystem renderSys = mRoot.GetRenderSystemByName("OpenGL Rendering Subsystem");
            renderSys.SetConfigOption("Colour Depth", "16");
            renderSys.SetConfigOption("Display Frequency", "40");
            renderSys.SetConfigOption("FSAA", "0");
            renderSys.SetConfigOption("Full Screen", "No");
            renderSys.SetConfigOption("RTT Preferred Mode", "FBO");
            renderSys.SetConfigOption("VSync", "Yes");
            renderSys.SetConfigOption("VSync Interval", "1");
            renderSys.SetConfigOption("Video Mode", "1024 x 768");
            renderSys.SetConfigOption("sRGB Gamma Conversion", "No");

            mRoot.RenderSystem = renderSys;
            this.mWindow = this.mRoot.Initialise(true, "SkyLands");

            return true;*/
            if (!this.mRoot.ShowConfigDialog()) { return false; }
            this.mWindow = this.mRoot.Initialise(true, "SkyLands");
            return true;
        }

        private void ChooseSceneManager() { this.mSceneMgr = this.mRoot.CreateSceneManager(SceneType.ST_GENERIC); }

        private void CreateCamera()
        {
            this.mCam = this.mSceneMgr.CreateCamera("MainCamera");
            this.mCam.NearClipDistance = 5;
        }

        private void CreateViewports()
        {
            this.mViewport = this.mWindow.AddViewport(this.mCam);
            this.mViewport.BackgroundColour = ColourValue.Black;
            this.mCam.AspectRatio = (this.mViewport.ActualWidth / this.mViewport.ActualHeight);
        }

        protected abstract void Create();

        protected abstract void Update(FrameEvent evt);

        private void LoadResources()
        {
            // Load resource paths from config file
            var cf = new ConfigFile();
            cf.Load(RESOURCES_CFG, "\t:=", true);

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

        private void ReloadAllTextures() { TextureManager.Singleton.ReloadAll(); }

        private void ProcessInput()
        {
            this.mInput.Update();

            if (mInput.WasKeyPressed(MOIS.KeyCode.KC_F11))   { this.CycleTextureFilteringMode(); }
            if (mInput.WasKeyPressed(MOIS.KeyCode.KC_F12))   { this.CyclePolygonMode(); }
            if (mInput.WasKeyPressed(MOIS.KeyCode.KC_F5))    { this.ReloadAllTextures(); }
            if (mInput.WasKeyPressed(MOIS.KeyCode.KC_SYSRQ)) { this.TakeScreenshot(); }
        }

        private void CycleTextureFilteringMode()
        {
            //MaterialManager.Singleton.SetDefaultTextureFiltering(TextureFilterOptions.TFO_NONE);
            string newVal;
            TextureFilterOptions tfo;
            uint aniso;

            switch (this.mPanel.getParamValue(7).ToUpper()[0])
            {
                case 'B':
                    newVal = "Trilinear";
                    tfo = TextureFilterOptions.TFO_TRILINEAR;
                    aniso = 1;
                    break;
                case 'T':
                    newVal = "Anisotropic";
                    tfo = TextureFilterOptions.TFO_ANISOTROPIC;
                    aniso = 8;
                    break;
                case 'A':
                    newVal = "None";
                    tfo = TextureFilterOptions.TFO_NONE;
                    aniso = 1;
                    break;
                default:
                    newVal = "Bilinear";
                    tfo = TextureFilterOptions.TFO_BILINEAR;
                    aniso = 1;
                    break;
            }

            MaterialManager.Singleton.SetDefaultTextureFiltering(tfo);
            MaterialManager.Singleton.DefaultAnisotropy = aniso;
            this.mPanel.setParamValue(7, newVal);
        }

        private void CyclePolygonMode()
        {
            this.mRenderMode = (this.mRenderMode + 1) % 3;
            switch (mRenderMode)
            {
                case 0: mCam.PolygonMode = PolygonMode.PM_SOLID;     break;
                case 1: mCam.PolygonMode = PolygonMode.PM_WIREFRAME; break;
                case 2: mCam.PolygonMode = PolygonMode.PM_POINTS;    break;
            }
        }

        private void TakeScreenshot() { mWindow.WriteContentsToTimestampedFile("screenshot", ".png"); }

        public void AddFrameLstn(RootLstn listener)    { listener.AddListener(this.mRoot); }
        public void RemoveFrameLstn(RootLstn listener) { listener.RemoveListener(this.mRoot); }

        private bool OnFrameRendering(FrameEvent evt)
        {
            if (this.mWindow.IsClosed || this.mIsShutDownRequested) { return false; }
            try
            {
                this.ProcessInput();
                this.Update(evt);
                this.mTrayMgr.frameRenderingQueued(evt);

                //this.mDebugOverlay.Update(evt.timeSinceLastFrame);
                return true;
            }
            catch (ShutdownException)
            { 
                this.mIsShutDownRequested = true;
                return false;
            }
        }

        protected virtual void Shutdown()
        {
            this.mTrayMgr.closeDialog();
            this.mTrayMgr = null;
        }
    }
}