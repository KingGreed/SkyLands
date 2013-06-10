using System;
using System.Drawing;
using System.Windows.Forms;
using Awesomium.Core;
using Game.Display;
using Mogre;

namespace Game.BaseApp {

    public abstract partial class OgreForm : Form {
        private const string PLUGINS_CFG = "plugins.cfg";
        private const string RESOURCES_CFG = "resources.cfg";

        protected Root mRoot;
        protected SceneManager mSceneMgr;
        protected RenderWindow mWindow;
        protected Controller mController;
        protected Camera mCam;
        protected Viewport mViewport;
        protected bool mIsShutDownRequested = false;
        private int mRenderMode = 0;

        public static Size WindowSize;
        public static Point WindowPosition;

        protected OgreForm() {
            //Awesomium

            if(!WebCore.IsRunning) { WebCore.Initialize(WebConfig.Default); }

            //this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.InitializeComponent();

            this.CenterToScreen();
            Cursor.Position = new Point(this.Location.X + this.Size.Width / 2,
                                        this.Location.Y + this.Size.Height / 2);

            this.Resize += this.OgreForm_Resize;
            this.Move += (sender, args) => WindowPosition = this.Location;

            this.webView.DocumentReady += onDocumentReady;
            this.webView.Source = new Uri("file://C:/Users/kingGreed/Lib/Web/SkyLands/index.html");
            this.webView.Hide();

            this.MinimumSize = new Size(800, 600);
            WindowSize = this.Size;
            WindowPosition = this.Location;
        }

        private void onDocumentReady(object sender, UrlEventArgs e) {
            if((this.webView == null) || !this.webView.IsLive) { return; }
            this.webView.DocumentReady -= onDocumentReady;

            if(this.webView.ParentView != null || !this.webView.IsJavascriptEnabled) { return; }


            JSObject jsobject = this.webView.CreateGlobalJavascriptObject("jsobject");
            jsobject["test"] = "truc";


            jsobject.Bind("LogMsg", false, JSLogger);

            this.webView.ExecuteJavascript("logger()");
        }

        private void JSLogger(object sender, JavascriptMethodEventArgs args) {
            if(args.Arguments.Length != 1) { LogManager.Singleton.DefaultLog.LogMessage("JS error : expected 1 argument of type string got : " + args.Arguments.Length + " arguments"); }
            else {
                LogManager.Singleton.DefaultLog.LogMessage(args.Arguments[0]);
            }
        }

        public void OgreForm_Resize(object sender, EventArgs e)   { this.mWindow.WindowMovedOrResized(); WindowSize = this.Size; }
        public void AddFrameLstn(RootLstn listener)               { listener.AddListener(this.mRoot);        }
        public void RemoveFrameLstn(RootLstn listener)            { listener.RemoveListener(this.mRoot);     }

        public void Go() {
            this.Show();
            while(mRoot != null && mRoot.RenderOneFrame()) { Application.DoEvents(); }
        }

        public bool Setup() {
            this.mRoot = new Root(PLUGINS_CFG);

            if(!this.Configure()) { return false; }

            this.ChooseSceneManager();
            this.CreateCamera();
            this.CreateViewports();

            TextureManager.Singleton.DefaultNumMipmaps = 5;

            this.LoadResources();

            this.mController = new Controller(this);

            MaterialManager.Singleton.SetDefaultTextureFiltering(TextureFilterOptions.TFO_NONE);

            this.AddFrameLstn(new RootLstn(RootLstn.TypeLstn.FrameRendering, this.OnFrameRendering));

            GraphicBlock.generateFace();
            LogManager.Singleton.DefaultLog.LogMessage("***********************Program\'s Log***********************");
            //LogManager.Singleton.DefaultLog.LogDetail = LoggingLevel.LL_LOW;
            this.mSceneMgr.ShadowTechnique = ShadowTechnique.SHADOWDETAILTYPE_INTEGRATED;
            this.mSceneMgr.ShadowFarDistance = 400;

            return true;
        }

        private bool Configure() {
            RenderSystem renderSys = mRoot.GetRenderSystemByName("OpenGL Rendering Subsystem");
            renderSys.SetConfigOption("Colour Depth", "16");
            renderSys.SetConfigOption("Display Frequency", "40");
            renderSys.SetConfigOption("FSAA", "0");
            renderSys.SetConfigOption("Full Screen", "No");
            renderSys.SetConfigOption("RTT Preferred Mode", "FBO");
            renderSys.SetConfigOption("VSync", "Yes");
            renderSys.SetConfigOption("VSync Interval", "1");
            renderSys.SetConfigOption("Video Mode", "1024 x 768");
            renderSys.SetConfigOption("sRGB Gamma Conversion", "No");

            this.mRoot.RenderSystem = renderSys;
            this.mRoot.Initialise(false, "SkyLands");
            NameValuePairList misc = new NameValuePairList();
            misc["externalWindowHandle"] = Handle.ToString();
            this.mWindow = this.mRoot.CreateRenderWindow("Main RenderWindow", 800, 600, false, misc);

            return true;
        }

        private void ChooseSceneManager() { this.mSceneMgr = this.mRoot.CreateSceneManager(SceneType.ST_GENERIC); }
        private void CreateCamera() {
            this.mCam = this.mSceneMgr.CreateCamera("MainCamera");
            this.mCam.NearClipDistance = 5;
        }

        private void CreateViewports() {
            this.mViewport = this.mWindow.AddViewport(this.mCam);
            this.mViewport.BackgroundColour = ColourValue.Black;
            this.mCam.AspectRatio = (this.mViewport.ActualWidth / this.mViewport.ActualHeight);
        }

        protected abstract void Update(FrameEvent evt);

        private void LoadResources() {
            // Load resource paths from config file
            var cf = new ConfigFile();
            cf.Load(RESOURCES_CFG, "\t:=", true);

            // Go through all sections & settings in the file
            var seci = cf.GetSectionIterator();
            while(seci.MoveNext()) {
                foreach(var pair in seci.Current) {
                    ResourceGroupManager.Singleton.AddResourceLocation(
                        pair.Value, pair.Key, seci.CurrentKey);
                }
            }

            ResourceGroupManager.Singleton.InitialiseAllResourceGroups();
        }

        private void ReloadAllTextures() { TextureManager.Singleton.ReloadAll(); }

        private void ProcessInput() {
            this.mController.Update();

            if (this.mController.WasKeyPressed(Keys.F12)) { this.CyclePolygonMode(); }
            if (this.mController.WasKeyPressed(Keys.F5)) { this.ReloadAllTextures(); }
            if (this.mController.WasKeyPressed(Keys.PrintScreen)) { this.TakeScreenshot(); }
        }

        private void CyclePolygonMode() {
            this.mRenderMode = (this.mRenderMode + 1) % 3;
            switch(this.mRenderMode) {
                case 0: this.mCam.PolygonMode = PolygonMode.PM_SOLID; break;
                case 1: this.mCam.PolygonMode = PolygonMode.PM_WIREFRAME; break;
                case 2: this.mCam.PolygonMode = PolygonMode.PM_POINTS; break;
            }
        }

        private void TakeScreenshot() { this.mWindow.WriteContentsToTimestampedFile("screenshot", ".png"); }

        private bool OnFrameRendering(FrameEvent evt) {
            if(this.mWindow.IsClosed || this.mIsShutDownRequested) { return false; }
            try {
                this.ProcessInput();
                this.Update(evt);
                this.mController.Clear();
                return true;
            } catch(ShutdownException) {
                this.mIsShutDownRequested = true;
                return false;
            }
        }

        protected virtual void Shutdown(object sender, EventArgs e)
        {
            this.mRoot.Dispose();
            this.mRoot = null;
        }
    }

    class ShutdownException : Exception { }
}
