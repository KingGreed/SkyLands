﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Mogre;
using MOIS;

using Awesomium.Core;
using API.Generic;

using Game.Display;
using Game.Input;
using Game.GUIs;

namespace Game.BaseApp {
    public abstract partial class OgreForm : Form {
        private const string  PLUGINS_CFG = "plugins.cfg";
        private const string  RESOURCES_CFG = "resources.cfg";
        private readonly Vector2 WND_SIZE = new Vector2(800, 600), INIT_SIZE = new Vector2(1600, 900);

        protected Root         mRoot;
        protected SceneManager mSceneMgr;
        protected RenderWindow mWindow;
        protected Controller   mController;
        protected Camera       mCam;
        protected Viewport     mViewport;
        protected bool         mIsShutDownRequested = false;
        private Size           mPreviousSize;
        private int            mRenderMode = 0;

        public Vector2 Ratio          { get; set; }

        protected OgreForm() {
            //Awesomium
            if(!WebCore.IsRunning) { WebCore.Initialize(WebConfig.Default); }

            this.InitializeComponent();

            this.CenterToScreen();
            Cursor.Position = new Point(this.Location.X + this.Size.Width / 2,
                                        this.Location.Y + this.Size.Height / 2);

            webView.DocumentReady += onDocumentReady;
            SelectBar.DocumentReady += onSelectBarLoaded;
            SelectBar.Source = new Uri("file://" + Directory.GetCurrentDirectory() + "/media/web/Selector.html");

            SelectBar.Size = new Size((int)Selector.WANTED_SIZE.x, (int)Selector.WANTED_SIZE.y);
            SelectBar.Location = new Point(this.Size.Width / 2 - (int)Selector.WANTED_SIZE.x / 2,
                                           this.Size.Height - (int)Selector.WANTED_SIZE.y);
            Console.WriteLine("SelectBar height : " + SelectBar.Location.Y);

            this.MinimumSize = new Size(800, 600);
            this.mPreviousSize = this.Size;
            Ratio = Vector2.UNIT_SCALE;
        }

        private void onDocumentReady(object sender, UrlEventArgs e) {
            if(webView == null || !webView.IsLive) { return; }
            if(webView.ParentView != null || !webView.IsJavascriptEnabled) { return; }

            JSObject jsobject = webView.CreateGlobalJavascriptObject("jsobject");

            jsobject.Bind("LogMsg", false, this.JSLogger);
        }

        private void onSelectBarLoaded(object sender, UrlEventArgs e)
        {
            if (SelectBar == null || !SelectBar.IsLive) { return; }
            if (SelectBar.ParentView != null || !SelectBar.IsJavascriptEnabled) { return; }

            string ratio = Convert.ToString(Cst.GUI_RATIO).Replace(',', '.');
            SelectBar.ExecuteJavascript("resize(" + ratio + ", " + ratio + ")");
        }

        private void JSLogger(object sender, JavascriptMethodEventArgs args) {
            if(args.Arguments.Length != 1) {
                LogManager.Singleton.DefaultLog.LogMessage("JS error : expected 1 argument of type string got : "
                                                            + args.Arguments.Length + " arguments"); }
            else {
                LogManager.Singleton.DefaultLog.LogMessage(args.Arguments[0]);
            }
        }

        public void OgreForm_Resize(object sender, EventArgs e) {
            this.mWindow.WindowMovedOrResized();
            string s = ("resize(" + this.Size.Width / INIT_SIZE.x + "," + this.Size.Height / INIT_SIZE.y + ")");
            webView.ExecuteJavascript(s.Replace(',', '.'));
            Ratio = new Vector2(this.Size.Width / WND_SIZE.x, this.Size.Height / WND_SIZE.y);
            foreach (GUI gui in GUI.GUIs) { gui.resize((float)this.Size.Width / this.mPreviousSize.Width,
                (float)this.Size.Height / this.mPreviousSize.Height,Ratio.x, Ratio.y); }
            this.mPreviousSize = this.Size;
        }

        public void AddFrameLstn(RootLstn listener) { listener.AddListener(this.mRoot); }
        public void RemoveFrameLstn(RootLstn listener) { listener.RemoveListener(this.mRoot); }

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

            int windowHnd;
            this.mWindow.GetCustomAttribute("WINDOW", out windowHnd);
            this.mController = new Controller(this, windowHnd);

            MaterialManager.Singleton.SetDefaultTextureFiltering(TextureFilterOptions.TFO_NONE);

            this.AddFrameLstn(new RootLstn(TypeLstn.FrameRendering, this.OnFrameRendering));

            GraphicBlock.generateFace();
            LogManager.Singleton.DefaultLog.LogMessage("***********************Program\'s Log***********************");
            //LogManager.Singleton.DefaultLog.LogDetail = LoggingLevel.LL_LOW;
            this.mSceneMgr.ShadowTechnique = ShadowTechnique.SHADOWDETAILTYPE_INTEGRATED;
            this.mSceneMgr.ShadowFarDistance = 400;
            Game.World.Display.ColoredMaterials.Init();

            this.Resize += this.OgreForm_Resize;

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
            renderSys.SetConfigOption("Video Mode", WND_SIZE.x + " x " + WND_SIZE.y);
            renderSys.SetConfigOption("sRGB Gamma Conversion", "No");

            this.mRoot.RenderSystem = renderSys;
            this.mRoot.Initialise(false, "SkyLands");
            NameValuePairList misc = new NameValuePairList();
            misc["externalWindowHandle"] = Handle.ToString();
            this.mWindow = this.mRoot.CreateRenderWindow("Main RenderWindow", (uint)WND_SIZE.x, (uint)WND_SIZE.y, false, misc);

            return true;
        }

        private void ChooseSceneManager() { this.mSceneMgr = this.mRoot.CreateSceneManager(SceneType.ST_GENERIC); }
        private void CreateCamera() {
            this.mCam = this.mSceneMgr.CreateCamera("Camera");
            this.mCam.NearClipDistance = 5;
        }

        private void CreateViewports() {
            this.mViewport = this.mWindow.AddViewport(this.mCam);
            this.mViewport.BackgroundColour = ColourValue.Black;
            this.mCam.AspectRatio = (this.mViewport.ActualWidth / this.mViewport.ActualHeight);
        }

        protected abstract void Update(float frameTime);

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

        private void ProcessInput(float frameTime) {
            this.mController.Update(frameTime);

            if(this.mController.WasKeyPressed(KeyCode.KC_F12)) { this.CyclePolygonMode(); }
            if (this.mController.WasKeyPressed(KeyCode.KC_F5)) { this.ReloadAllTextures(); }
            if (this.mController.WasKeyPressed(KeyCode.KC_SYSRQ)) { this.TakeScreenshot(); }
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
                float frameTime = evt.timeSinceLastFrame;
                this.ProcessInput(frameTime);
                this.Update(frameTime);
                this.mController.Clear();
                return true;
            } catch(ShutdownException) {
                this.mIsShutDownRequested = true;
                return false;
            }
        }

        protected virtual void Shutdown(object sender, EventArgs e) {
            this.mRoot.Dispose();
            this.mRoot = null;
        }
    }

    class ShutdownException : Exception { }
}