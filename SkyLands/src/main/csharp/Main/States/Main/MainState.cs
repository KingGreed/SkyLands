using System;
using Mogre;

using Game.CharacSystem;
using Game.World;
using Game.GUICreator;

namespace Game.States
{
    public abstract class MainState : State
    {
        protected MainWorld mWorld;
        private SceneNode   mSelectedEntity;

        public User      User { get; protected set; }
        public CharacMgr CharacMgr { get; protected set; }
        public MainGUI   MainGUI { get; protected set; }

        protected MainState(StateManager stateMgr, string name) : base(stateMgr, name) { this.mSelectedEntity = null; }

        protected override void Startup()
        {
            this.mWorld = new MainWorld(this.mStateMgr);
            if (!this.mStateMgr.GameInfo.Load) { this.mWorld.populate(); }
            this.mWorld.setSafeSpawnPoint();
            this.mWorld.display();

            this.User = new User(this.mStateMgr, this.mWorld);

            this.AfterWorldCreation();

            this.Show();
            Mogre.LogManager.Singleton.DefaultLog.LogMessage(" => Game loop begin");
        }

        protected virtual void AfterWorldCreation() {}

        public override void Show()
        {
            this.mStateMgr.HideGUIs();
            this.mStateMgr.MiyagiMgr.CursorVisibility = false;
            if (!this.User.IsFreeCamMode) { this.MainGUI.Show(); }
            this.User.IsAllowedToMoveCam = true;
        }

        public override void Update(float frameTime)
        {
            this.mWorld.Update(frameTime);

            if (this.mStateMgr.Input.WasKeyPressed(MOIS.KeyCode.KC_ESCAPE))
                this.mStateMgr.RequestStatePush(typeof(IGMenuState));

            this.User.Update(frameTime);
            this.CharacMgr.Update(frameTime);

            /* Entity selection */
            if (this.User.IsFreeCamMode && this.mStateMgr.Input.WasMouseButtonPressed(MOIS.MouseButtonID.MB_Left))
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
                    int characId = Convert.ToInt32(this.mSelectedEntity.Name.Substring(14));
                    this.mSelectedEntity.ShowBoundingBox = false;
                    this.CharacMgr.GetCharacterById(characId).MoveTo(this.CharacMgr.World.getSpawnPoint() + Vector3.UNIT_Y);
                    this.mSelectedEntity = null;
                }
            }
        }

        public void EnableMovement(bool enable)
        {
            this.User.IsAllowedToMoveCam = enable;
            if(this.CharacMgr.MainPlayer != null) { this.CharacMgr.MainPlayer.MovementInfo.IsAllowedToMove = enable; }
        }

        protected override void Shutdown()
        {
            Mogre.LogManager.Singleton.DefaultLog.LogMessage(" => Game loop end");
            this.mStateMgr.SceneMgr.DestroyAllParticleSystems();
            this.MainGUI.Dispose();
            this.CharacMgr.Dispose();
            this.mWorld.Shutdown();
        }

        public abstract void Save();
    }
}
