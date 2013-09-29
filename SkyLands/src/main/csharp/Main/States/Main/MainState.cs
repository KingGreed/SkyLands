using System;
using System.Drawing;
using System.IO;
using API.Generic;
using Game.csharp.Main.RTS.Buildings;
using Mogre;
using Awesomium.Core;

using Game.BaseApp;
using Game.CharacSystem;
using Game.World;
using Game.Input;
using Game.GUIs;
using Game.RTS;

namespace Game.States
{
    public abstract class MainState : State
    {
        public User          User      { get; protected set; }
        public CharacMgr     CharacMgr { get; protected set; }
        public API.Geo.World World     { get; protected set; }
        public BuildingManager BuildingMgr { get; protected set; }
        public RTSManager RTSMgr           { get; protected set; }

        protected MainState(StateManager stateMgr, string name) : base(stateMgr, name) { }

        protected override void Startup()
        {
            this.User = new User(this.mStateMgr, this.World);

            if (!this.mStateMgr.IsOnWorldChangement)
            {
                if (!GUI.WebControls.Contains(OgreForm.SelectBar))
                    GUI.WebControls.Add(OgreForm.SelectBar);
                
                OgreForm.SelectBar.DocumentReady += onSelectBarLoaded;
                OgreForm.SelectBar.Crashed += OnCrashed;
                OgreForm.SelectBar.Source =
                    new Uri("file://" + Directory.GetCurrentDirectory() + "/media/web/Selector.html");
                OgreForm.SelectBar.Visible = false;

                
                if (!GUI.WebControls.Contains(OgreForm.Hud))
                    GUI.WebControls.Add(OgreForm.Hud);

                OgreForm.Hud.DocumentReady += onHUDLoaded;
                OgreForm.Hud.Crashed       += OnCrashed;
                OgreForm.Hud.Source = new Uri("file://" + Directory.GetCurrentDirectory() + "/media/web/HUD.html");
                OgreForm.Hud.Visible = false;
            }

            this.AfterWorldCreation();
            if(!this.mStateMgr.GameInfo.Load) { this.World.populate(); }
            this.World.display();

            LogManager.Singleton.DefaultLog.LogMessage(" => Game loop begin");
        }

        private void onHUDLoaded(object sender, UrlEventArgs e) {
            if (OgreForm.Hud == null || !OgreForm.Hud.IsLive) { return; }
            if (OgreForm.Hud.ParentView != null || !OgreForm.Hud.IsJavascriptEnabled) { return; }

            string[] buttons = new string[] { "Menu", "" };
            string[] inputs  = new string[] { "0", "0", "0" };

            Hud.Init(buttons, inputs);
            Vector2 size = new Vector2(this.mStateMgr.Width, (int)(this.mStateMgr.Height * 0.05f));

            OgreForm.Hud.Visible = true;
            OgreForm.Hud.Size = new Size((int)size.x, (int)size.y);
            OgreForm.Hud.Location = new Point(0, 0);

            Vector2 ratio = new Vector2(size.x / Hud.IMAGE_SIZE.x, size.y / Hud.IMAGE_SIZE.y);
            GUI.ResizeJavascript(OgreForm.Hud, ratio);
            
            OgreForm.Hud.DocumentReady -= onHUDLoaded;
        }

        private void OnCrashed(object sender, CrashedEventArgs e) {
            // Oops! The WebView crashed.
            Mogre.LogManager.Singleton.DefaultLog.LogMessage(e.Status.ToString());
        }

        private void onSelectBarLoaded(object sender, UrlEventArgs e)
        {
            if (OgreForm.SelectBar == null || !OgreForm.SelectBar.IsLive) { return; }
            if (OgreForm.SelectBar.ParentView != null || !OgreForm.SelectBar.IsJavascriptEnabled) { return; }

            Selector.Init(this.User.Inventory, this.mStateMgr.GameInfo.IsInEditorMode);
            Vector2 size = Selector.WANTED_SIZE * OgreForm.Ratio;
            Vector2 location = (OgreForm.InitSize / 2 - Selector.WANTED_SIZE / 2) * OgreForm.Ratio;
            OgreForm.SelectBar.Size = new Size((int)size.x, (int)size.y);
            OgreForm.SelectBar.Location = new Point((int)location.x, (int)(500f * this.mStateMgr.Height / 600 * OgreForm.Ratio.y));
            Vector2 ratio = new Vector2(size.x / Selector.IMAGE_SIZE.x, size.y / Selector.IMAGE_SIZE.y);
            GUI.ResizeJavascript(OgreForm.SelectBar, ratio);
            OgreForm.SelectBar.Visible = true;
            OgreForm.SelectBar.DocumentReady -= onSelectBarLoaded;
        }

        protected virtual void AfterWorldCreation() {}

        public override void Show()
        {
            this.mStateMgr.Controller.CursorVisibility = false;
            this.mStateMgr.Controller.BlockMouse = true;

            this.User.IsAllowedToMoveCam = true;
            GUI.Visible = false;
            OgreForm.SelectBar.Visible = true;
            OgreForm.Hud.Visible = true;
        }

        public override void Hide()
        {
            if (this.mStateMgr.IsOnWorldChangement) { return; }

            this.mStateMgr.Controller.CursorVisibility = true;
            this.mStateMgr.Controller.BlockMouse = false;
            OgreForm.SelectBar.Visible = false;
            OgreForm.Hud.Visible = false;
        }

        public override void Update(float frameTime)
        {
            this.World.Update(frameTime);

            this.User.Update(frameTime);
            this.CharacMgr.Update(frameTime);
        }

        public abstract void OpenMainGUI();

        public void Restart()
        {
            this.Shutdown();
            this.Startup();
        }

        protected override void Shutdown()
        {
            LogManager.Singleton.DefaultLog.LogMessage(" => Game loop end");
            this.mStateMgr.SceneMgr.DestroyAllParticleSystems();
            this.CharacMgr.Dispose();
            this.World.Shutdown();
        }

        public abstract void Save();
    }
}
