using Mogre;

using Game.Input;
using Game.Animation;
using Game.Shoot;
using Game.GUIs;

namespace Game.CharacSystem
{
    public class VanillaPlayer : VanillaCharacter
    {
        public const float  DEFAULT_PLAYER_LIFE = 830;
        public const float  DEFAULT_PLAYER_MANA = 120;

        private readonly ShootCube mShootCube;
        private User               mUser;

        public bool IsMainPlayer  { get { return this.mUser != null; } }

        public VanillaPlayer(CharacMgr characMgr, string meshName, CharacterInfo info) : base(characMgr, info)
        {
            SceneManager sceneMgr = characMgr.SceneMgr;
            Entity ent = sceneMgr.CreateEntity("CharacterEnt_" + this.mCharInfo.Id, meshName);
            ent.Skeleton.BlendMode = SkeletonAnimationBlendMode.ANIMBLEND_CUMULATIVE;
            Entity swordL = sceneMgr.CreateEntity("Sword.mesh");
            ent.AttachObjectToBone("Sheath.L", swordL);
            Entity swordR = sceneMgr.CreateEntity("Sword.mesh");
            ent.AttachObjectToBone("Sheath.R", swordR);
            this.mMesh = new Sinbad(ent);

            this.mNode = characMgr.SceneMgr.RootSceneNode.CreateChildSceneNode("CharacterNode_" + this.mCharInfo.Id, Vector3.ZERO, this.mMesh.InitialOrientation);
            this.mNode.AttachObject(ent);
            this.mNode.Scale(this.mMesh.MeshSize / ent.BoundingBox.Size);
            this.mNode.Orientation = this.mMesh.InitialOrientation;

            this.mCollisionMgr = new CollisionMgr(characMgr.SceneMgr, this.mCharacMgr.World, this);
            this.FeetPosition = this.mCharInfo.SpawnPoint;

            this.mShootCube = new ShootCube(this.mCharacMgr.BulletMgr, this);
        }

        public void MakeHimMainPlayer(User user) { this.mUser = user; }

        public void SwitchFreeCamMode()
        {
            this.mNode.SetVisible(this.mUser.IsFreeCamMode, false);
            if (this.mUser.IsFreeCamMode)
                this.mMesh.ToFreeCamMode();
        }

        public new void Update(float frameTime)
        {
            if (this.MovementInfo.IsAllowedToMove && !this.MovementInfo.IsMovementForced)
            {
                this.MovementInfo.MoveDirection = new Vector3(this.mCharacMgr.Controller.MovementFactor.x,
                                                              this.mCharacMgr.Controller.HasActionOccured(UserAction.Jump) ? 1 : 0,
                                                              this.mCharacMgr.Controller.MovementFactor.z);
                
                this.MovementInfo.YawValue = this.mCharacMgr.Controller.Yaw;

                this.MovementInfo.Sprint = this.mCharacMgr.Controller.IsActionOccuring(UserAction.Sprint);
                if(this.mCharacMgr.Controller.HasActionOccured(UserAction.Levitate))
                    this.setIsPushedByArcaneLevitator(!this.MovementInfo.IsPushedByArcaneLevitator);

                ((Sinbad) this.mMesh).UpdateEmotes(this.mCharacMgr.Controller);
            }

            if (!this.mUser.IsFreeCamMode && this.mUser.IsAllowedToMoveCam && Selector.IsBullet)
            {
                if (this.mCharacMgr.Controller.IsActionOccuring(UserAction.MainAction))
                    this.mShootCube.Grow(frameTime, this.mCharacMgr.Controller.HasActionOccured(UserAction.MainAction));
                if (this.mCharacMgr.Controller.HasActionEnded(UserAction.MainAction))
                    this.mShootCube.Burst();
            }
        }

        public override bool Dispose(bool updateTargets = true)
        {
            bool switchToFreeCamMode = this.IsMainPlayer && !this.mUser.IsFreeCamMode;
            if (switchToFreeCamMode) { this.mUser.SwitchFreeCamMode(); }
            
            base.Dispose(updateTargets);

            return this.IsMainPlayer;
        }
    }
}
