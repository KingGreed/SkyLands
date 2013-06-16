using System;
using System.Drawing;
using System.IO;
using Mogre;
using Awesomium.Core;

using API.Generic;

using Game.BaseApp;
using Game.CharacSystem;
using Game.World;
using Game.Input;
using Game.GUIs;

namespace Game.States
{
    public abstract class MainState : State
    {
        private bool mIsSelectBarReady;

        public User      User      { get; protected set; }
        public CharacMgr CharacMgr { get; protected set; }
        public MainWorld World     { get; protected set; }

        protected MainState(StateManager stateMgr, string name) : base(stateMgr, name) { }

        protected override void Startup()
        {
            this.World = new MainWorld(this.mStateMgr);
            this.World.setSafeSpawnPoint();

            this.User = new User(this.mStateMgr, this.World);

            if (!GUI.WebControls.Contains(OgreForm.SelectBar))
                GUI.WebControls.Add(OgreForm.SelectBar);
            OgreForm.SelectBar.DocumentReady += onSelectBarLoaded;
            OgreForm.SelectBar.Source =
                new Uri("file://" + Directory.GetCurrentDirectory() + "/media/web/Selector.html");
            OgreForm.SelectBar.Visible = false;

            this.AfterWorldCreation();
            if(!this.mStateMgr.GameInfo.Load) { this.World.populate(); }
            this.World.display();

            LogManager.Singleton.DefaultLog.LogMessage(" => Game loop begin");
        }

        private void onSelectBarLoaded(object sender, UrlEventArgs e)
        {
            if (OgreForm.SelectBar == null || !OgreForm.SelectBar.IsLive) { return; }
            if (OgreForm.SelectBar.ParentView != null || !OgreForm.SelectBar.IsJavascriptEnabled) { return; }

            Selector.Init(this.User.Inventory);
            Vector2 size = Selector.WANTED_SIZE * OgreForm.Ratio;
            Vector2 location = (OgreForm.InitSize / 2 - Selector.WANTED_SIZE / 2) * OgreForm.Ratio;
            OgreForm.SelectBar.Size = new Size((int)size.x, (int)size.y);
            OgreForm.SelectBar.Location = new Point((int)location.x, (int)(500 * OgreForm.Ratio.y));
            Vector2 ratio = new Vector2(size.x / Selector.IMAGE_SIZE.x, size.y / Selector.IMAGE_SIZE.y);
            GUI.ResizeJavascript(OgreForm.SelectBar, ratio);
            OgreForm.SelectBar.Visible = true;
            OgreForm.SelectBar.DocumentReady -= onSelectBarLoaded;
            this.mIsSelectBarReady = true;
        }

        protected virtual void AfterWorldCreation() {}

        public override void Show()
        {
            this.mStateMgr.Controller.CursorVisibility = false;
            this.mStateMgr.Controller.BlockMouse = true;

            this.User.IsAllowedToMoveCam = true;
            GUI.Visible = false;
            OgreForm.SelectBar.Visible = true;
            if (this.mIsSelectBarReady) { Selector.Init(this.User.Inventory); }
        }

        public override void Hide()
        {
            this.mStateMgr.Controller.CursorVisibility = true;
            this.mStateMgr.Controller.BlockMouse = false;
            OgreForm.SelectBar.Visible = false;
        }

        public override void Update(float frameTime)
        {
            this.World.Update(frameTime);

            if (this.mStateMgr.Controller.HasActionOccured(UserAction.Start))
                this.mStateMgr.RequestStatePop();

            this.User.Update(frameTime);
            this.CharacMgr.Update(frameTime);

            /* Entity selection */
            /*if (this.User.IsFreeCamMode && this.mStateMgr.Controller.HasActionOccured(UserAction.MainAction))
            {
                if (this.mSelectedEntity == null)
                {
                    RaySceneQuery raySceneQuery = this.CharacMgr.SceneMgr.CreateRayQuery(new Ray());
                    raySceneQuery.Ray = this.mStateMgr.Camera.GetCameraToViewportRay(0.5f, 0.5f);

                    RaySceneQueryResult.Enumerator itr = (RaySceneQueryResult.Enumerator) raySceneQuery.Execute().GetEnumerator();

                    while (itr.MoveNext())
                    {
                        if (itr.Current.movable.Name.Contains("CharacterEnt_"))
                        {
                            this.mSelectedEntity = ((SceneNode)itr.Current.movable.ParentNode);
                            this.mSelectedEntity.ShowBoundingBox = true;
                        }
                    }
                }
                else
                {
                    int characId = System.Convert.ToInt32(this.mSelectedEntity.Name.Substring(14));
                    this.mSelectedEntity.ShowBoundingBox = false;
                    this.CharacMgr.GetCharacterById(characId).MoveTo(this.CharacMgr.World.getSpawnPoint());
                    this.mSelectedEntity = null;
                }
            }*/
        }

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
