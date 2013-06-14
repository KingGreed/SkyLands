using Mogre;

using Game.CharacSystem;
using Game.World;
using Game.Input;
using Game.GUIs;

namespace Game.States
{
    public abstract class MainState : State
    {
        protected MainWorld mWorld;
        private SceneNode   mSelectedEntity;

        public User      User      { get; protected set; }
        public CharacMgr CharacMgr { get; protected set; }

        protected MainState(StateManager stateMgr, string name) : base(stateMgr, name) { this.mSelectedEntity = null; }

        protected override void Startup()
        {
            this.mWorld = new MainWorld(this.mStateMgr);
            this.mWorld.setSafeSpawnPoint();

            this.User = new User(this.mStateMgr, this.mWorld);

            this.AfterWorldCreation();
            if(!this.mStateMgr.GameInfo.Load) { this.mWorld.populate(); }
            this.mWorld.display();

            LogManager.Singleton.DefaultLog.LogMessage(" => Game loop begin");
        }

        protected virtual void AfterWorldCreation() {}

        public override void Show()
        {
            this.mStateMgr.Controller.CursorVisibility = false;
            this.mStateMgr.Controller.BlockMouse = true;

            this.User.IsAllowedToMoveCam = true;
        }

        public override void Hide()
        {
            this.mStateMgr.Controller.CursorVisibility = true;
            this.mStateMgr.Controller.BlockMouse = false;
        }

        public override void Update(float frameTime)
        {
            this.mWorld.Update(frameTime);

            if (this.mStateMgr.Controller.HasActionOccured(UserAction.Start))
                this.mStateMgr.RequestStatePop();

            this.User.Update(frameTime);
            this.CharacMgr.Update(frameTime);

            /* Entity selection */
            if (this.User.IsFreeCamMode && this.mStateMgr.Controller.HasActionOccured(UserAction.MainAction))
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
            }
        }

        protected override void Shutdown()
        {
            LogManager.Singleton.DefaultLog.LogMessage(" => Game loop end");
            this.mStateMgr.SceneMgr.DestroyAllParticleSystems();
            this.CharacMgr.Dispose();
            this.mWorld.Shutdown();
        }

        public abstract void Save();
    }
}
