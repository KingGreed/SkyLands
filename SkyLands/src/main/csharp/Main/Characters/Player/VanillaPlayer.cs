using System.Windows.Forms;
using Mogre;

using Game.Input;
using Game.Animation;
using Game.Shoot;

namespace Game.CharacSystem
{
    public class VanillaPlayer : VanillaCharacter
    {
        public const float  DEFAULT_PLAYER_LIFE = 340;
        public const float  DEFAULT_PLAYER_MANA = 120;
        private const float YAW_SENSIVITY = 0.4f;
        private const float PITCH_SENSIVITY = 0.15f;

        private readonly bool              mIsFirstView;
        private readonly ShootCube         mShootCube;
        private User                       mUser;

        public bool             IsFirstView   { get { return this.mIsFirstView; } }
        public MainPlayerCamera MainPlayerCam { get; private set; }
        public Degree           Pitch         { get { return this.MainPlayerCam.Pitch; } }
        public bool             IsMainPlayer  { get { return this.MainPlayerCam != null; } }

        public VanillaPlayer(CharacMgr characMgr, string meshName, CharacterInfo info) : base(characMgr, info)
        {
            this.mIsFirstView = true;

            SceneManager sceneMgr = characMgr.SceneMgr;
            Entity ent = sceneMgr.CreateEntity("CharacterEnt_" + this.mCharInfo.Id, meshName);
            ent.Skeleton.BlendMode = SkeletonAnimationBlendMode.ANIMBLEND_CUMULATIVE;
            Entity swordL = sceneMgr.CreateEntity("Sword.mesh");
            ent.AttachObjectToBone("Sheath.L", swordL);
            Entity swordR = sceneMgr.CreateEntity("Sword.mesh");
            ent.AttachObjectToBone("Sheath.R", swordR);
            this.mMesh = new Sinbad(ent);

            this.mNode.AttachObject(ent);
            this.mNode.Scale(this.mMesh.MeshSize / ent.BoundingBox.Size);
            this.mNode.Orientation = this.mMesh.InitialOrientation;

            this.mCollisionMgr = new CollisionMgr(characMgr.SceneMgr, this.mCharacMgr.World, this);
            this.FeetPosition = this.mCharInfo.SpawnPoint;
            this.mShootCube = new ShootCube(this.mCharacMgr.SceneMgr, this, this.mCharacMgr.BulletMgr);
        }

        public void MakeHimMainPlayer(User user, MainPlayerCamera cam)
        {
            this.mUser = user;
            this.MainPlayerCam = cam;
        }

        public void SwitchFreeCamMode()
        {
            this.mNode.SetVisible(this.mUser.IsFreeCamMode, true);
            if (this.mUser.IsFreeCamMode)
                this.mMesh.ToFreeCamMode();
        }

        public new void Update(float frameTime)
        {
            if(this.mUser.IsFreeCamMode) {this.MovementInfo.IsAllowedToMove = this.mCharacMgr.Controller.IsKeyDown(Keys.ControlKey);}

            if (this.MovementInfo.IsAllowedToMove)
            {
                float yawValue = -this.mCharacMgr.Controller.Yaw * YAW_SENSIVITY;
                float pitchValue = -this.mCharacMgr.Controller.Pitch * PITCH_SENSIVITY;

                if (this.mIsFirstView) { this.FirstPersonUpdate(yawValue, pitchValue); }
                else { this.ThirdPersonUpdate(yawValue, pitchValue); }

                this.MovementInfo.Sprint = this.mCharacMgr.Controller.IsActionOccuring(UserAction.Sprint);
                if(this.mCharacMgr.Controller.HasActionOccured(UserAction.Levitate))
                    this.setIsPushedByArcaneLevitator(!this.MovementInfo.IsPushedByArcaneLevitator);

                /* Update emotes animations */
                if (!this.mMesh.AnimMgr.AreAnimationsPlaying(MeshAnim.GetString(Sinbad.AnimName.JumpStart, Sinbad.AnimName.JumpLoop, Sinbad.AnimName.JumpEnd, Sinbad.AnimName.RunBase, Sinbad.AnimName.RunTop)))
                {
                    foreach (Sinbad.Emote emote in ((Sinbad)this.mMesh).Emotes)
                    {
                        if (this.mCharacMgr.Controller.HasActionOccured(emote.Action))
                        {
                            if (!this.mMesh.AnimMgr.AreAnimationsPlaying(MeshAnim.GetString(emote.Anim))) { this.mMesh.AnimMgr.SetAnims(MeshAnim.GetString(emote.Anim)); }
                            else { this.mMesh.AnimMgr.DeleteAnims(MeshAnim.GetString(emote.Anim)); }
                        }
                    }
                }
            }

            if (this.mUser.IsAllowedToMoveCam && this.mUser.Selector.IsBullet)
            {
                if (this.mCharacMgr.Controller.HasActionEnded(UserAction.MainAction)) { this.mShootCube.Burst(); }
                if (this.mCharacMgr.Controller.IsActionOccuring(UserAction.MainAction))
                {
                    this.mShootCube.Material = this.mUser.Selector.Material;
                    this.mShootCube.Grow(frameTime, this.mCharacMgr.Controller.HasActionOccured(UserAction.MainAction));
                }
            }
        }

        private void FirstPersonUpdate(float yawValue, float pitchValue)
        {
            this.MovementInfo.MoveDirection = new Vector3(this.mCharacMgr.Controller.MovementFactor.x,
                                                          this.mCharacMgr.Controller.HasActionOccured(UserAction.Jump) ? 1 : 0,
                                                          this.mCharacMgr.Controller.MovementFactor.z);
            this.MovementInfo.YawValue = yawValue;
            this.MovementInfo.PitchValue = pitchValue;
        }

        private void ThirdPersonUpdate(float yawValue, float pitchValue) { }
    }
}
