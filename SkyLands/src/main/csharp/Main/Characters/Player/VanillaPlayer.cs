using System;
using System.Windows.Forms;
using Mogre;

using Game.World;
using Game.Animation;
using Game.Shoot;
using Game.GUIs;
using Game.Buildings;

using API.Geo.Cuboid;
using API.Generic;

namespace Game.CharacSystem
{
    public class VanillaPlayer : VanillaCharacter
    {
        private struct Emote
        {
            private readonly Sinbad.AnimName mAnim;
            private readonly Controller.UserAction mAction;

            public Sinbad.AnimName Anim { get { return this.mAnim; } }
            public Controller.UserAction Action { get { return this.mAction; } }

            public Emote(Controller.UserAction action, Sinbad.AnimName anim) { this.mAction = action; this.mAnim = anim; }
        }

        public const float  DEFAULT_PLAYER_LIFE = 340;
        private const float YAW_SENSIVITY = 0.4f;
        private const float PITCH_SENSIVITY = 0.15f;

        private readonly Controller        mController;
        private readonly Emote[]           mEmotes;
        private readonly Sinbad.AnimName[] mEmotesNames;
        private readonly bool              mIsFirstView;
        private readonly ShootCube         mShootCube;
        private User                       mUser;

        public Controller       Input         { get { return this.mController; } }
        public bool             IsFirstView   { get { return this.mIsFirstView; } }
        public MainPlayerCamera MainPlayerCam { get; private set; }
        public Degree           Pitch         { get { return this.MainPlayerCam.Pitch; } }
        public bool             IsMainPlayer  { get { return this.MainPlayerCam != null; } }

        public VanillaPlayer(CharacMgr characMgr, string meshName, CharacterInfo info, Controller input) : base(characMgr, info)
        {
            this.mController = input;
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

            this.mEmotes = new Emote[]
            {
                new Emote(Controller.UserAction.Dance, Sinbad.AnimName.Dance)
            };
            this.mEmotesNames = new Sinbad.AnimName[this.mEmotes.Length];
            for (int i = 0; i < this.mEmotes.Length; i++)
                this.mEmotesNames[i] = this.mEmotes[i].Anim;
        }

        public void MakeHimMainPlayer(User user, MainPlayerCamera cam)
        {
            this.mUser = user;
            this.MainPlayerCam = cam;
            /*this.HUD = hud;
            this.HUD.UpdateLife(this.mCharInfo.Life, DEFAULT_PLAYER_LIFE);*/
        }

        public void SwitchFreeCamMode()
        {
            this.mNode.SetVisible(this.mUser.IsFreeCamMode, true);
            if (this.mUser.IsFreeCamMode)
                this.mMesh.ToFreeCamMode();
        }

        public new void Update(float frameTime)
        {
            if(this.mUser.IsFreeCamMode) {this.MovementInfo.IsAllowedToMove = this.mController.IsKeyDown(Keys.ControlKey);}

            if (this.MovementInfo.IsAllowedToMove)
            {
                float yawValue = -this.mController.Yaw * YAW_SENSIVITY;
                float pitchValue = -this.mController.Pitch * PITCH_SENSIVITY;

                if (this.mIsFirstView) { this.FirstPersonUpdate(yawValue, pitchValue); }
                else { this.ThirdPersonUpdate(yawValue, pitchValue); }

                this.MovementInfo.Sprint = this.mController.IsActionOccuring(Controller.UserAction.Sprint);
                if(this.mController.HasActionOccured(Controller.UserAction.Levitate))
                    this.setIsPushedByArcaneLevitator(!this.MovementInfo.IsPushedByArcaneLevitator);

                /* Update emotes animations */
                if (!this.mMesh.AnimMgr.AreAnimationsPlaying(MeshAnim.GetString(Sinbad.AnimName.JumpStart, Sinbad.AnimName.JumpLoop, Sinbad.AnimName.JumpEnd, Sinbad.AnimName.RunBase, Sinbad.AnimName.RunTop)))
                {
                    foreach (Emote emote in this.mEmotes)
                    {
                        if (this.mController.HasActionOccured(emote.Action))
                        {
                            if (!this.mMesh.AnimMgr.AreAnimationsPlaying(MeshAnim.GetString(emote.Anim))) { this.mMesh.AnimMgr.SetAnims(MeshAnim.GetString(emote.Anim)); }
                            else { this.mMesh.AnimMgr.DeleteAnims(MeshAnim.GetString(emote.Anim)); }
                        }
                    }
                }
            }

            if (this.mUser.IsAllowedToMoveCam && this.mUser.Selector.IsBullet)
            {
                if (this.mController.HasActionEnded(Controller.UserAction.MainAction)) { this.mShootCube.Burst(); }
                if (this.mController.IsActionOccuring(Controller.UserAction.MainAction))
                {
                    this.mShootCube.Material = this.mUser.Selector.Material;
                    this.mShootCube.Grow(frameTime, this.mController.HasActionOccured(Controller.UserAction.MainAction));
                }
            }
        }

        private void FirstPersonUpdate(float yawValue, float pitchValue)
        {
            this.MovementInfo.MoveDirection = new Vector3(this.mController.MovementFactor.x,
                                                          this.mController.HasActionOccured(Controller.UserAction.Jump) ? 1 : 0,
                                                          this.mController.MovementFactor.z);
            this.MovementInfo.YawValue = yawValue;
            this.MovementInfo.PitchValue = pitchValue;
        }

        private void ThirdPersonUpdate(float yawValue, float pitchValue) { }
    }
}
