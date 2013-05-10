using System;
using Mogre;

using Game.World;
using Game.Animation;
using Game.Shoot;
using Game.GUICreator;

using API.Geo.Cuboid;
using API.Generic;

using Game.Buildings;

namespace Game.CharacSystem
{
    public class VanillaPlayer : VanillaCharacter
    {
        private struct Emote
        {
            private readonly Sinbad.AnimName mAnim;
            private readonly MOIS.KeyCode mKey;

            public Sinbad.AnimName Anim { get { return this.mAnim; } }
            public MOIS.KeyCode Key { get { return this.mKey; } }

            public Emote(MOIS.KeyCode key, Sinbad.AnimName anim) { this.mKey = key; this.mAnim = anim; }
        }

        public const float  DEFAULT_PLAYER_LIFE = 340;
        private const float YAW_SENSIVITY = 0.4f;
        private const float PITCH_SENSIVITY = 0.15f;

        private readonly MoisManager       mInput;
        private readonly Emote[]           mEmotes;
        private readonly Sinbad.AnimName[] mEmotesNames;
        private readonly bool              mIsFirstView;
        private readonly ShootCube         mShootCube;
        private CameraMgr                  mCameraMgr;

        public MoisManager      Input         { get { return this.mInput; } }
        public bool             IsFirstView   { get { return this.mIsFirstView; } }
        public MainPlayerCamera MainPlayerCam { get; private set; }
        public Degree           Pitch         { get { return this.MainPlayerCam.Pitch; } }
        public HUD              HUD           { get; private set; }
        public bool             IsMainPlayer  { get { return this.MainPlayerCam != null; } }

        public VanillaPlayer(CharacMgr characMgr, string meshName, CharacterInfo info, MoisManager input) : base(characMgr, info)
        {
            this.mInput = input;
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
                new Emote(MOIS.KeyCode.KC_1, Sinbad.AnimName.Dance)
            };
            this.mEmotesNames = new Sinbad.AnimName[this.mEmotes.Length];
            for (int i = 0; i < this.mEmotes.Length; i++)
                this.mEmotesNames[i] = this.mEmotes[i].Anim;
        }

        public void MakeHimMainPlayer(CameraMgr cameraMgr, MainPlayerCamera cam, HUD hud)
        {
            this.mCameraMgr = cameraMgr;
            this.MainPlayerCam = cam;
            this.HUD = hud;
            this.HUD.UpdateLife(this.mCharInfo.Life, DEFAULT_PLAYER_LIFE);
        }

        public void SwitchFreeCamMode()
        {
            this.mNode.SetVisible(this.mCameraMgr.IsFreeCamMode, true);
            if (this.mCameraMgr.IsFreeCamMode)
                this.mMesh.ToFreeCamMode();
        }

        public new void Update(float frameTime)
        {
            if(this.mCameraMgr.IsFreeCamMode) {this.MovementInfo.IsAllowedToMove = this.mInput.IsCtrltDown;}

            if (this.MovementInfo.IsAllowedToMove)
            {
                float yawValue = -this.mInput.MouseMoveX * YAW_SENSIVITY;
                float pitchValue = -this.mInput.MouseMoveY * PITCH_SENSIVITY;

                if (this.mIsFirstView) { this.FirstPersonUpdate(yawValue, pitchValue); }
                else { this.ThirdPersonUpdate(yawValue, pitchValue); }

                this.MovementInfo.Sprint = this.mInput.IsShiftDown;

                /* Update emotes animations */
                if (!this.mMesh.AnimMgr.AreAnimationsPlaying(MeshAnim.GetString(Sinbad.AnimName.JumpStart, Sinbad.AnimName.JumpLoop, Sinbad.AnimName.JumpEnd, Sinbad.AnimName.RunBase, Sinbad.AnimName.RunTop)))
                {
                    foreach (Emote emote in this.mEmotes)
                    {
                        if (this.mInput.WasKeyPressed(emote.Key))
                        {
                            if (!this.mMesh.AnimMgr.AreAnimationsPlaying(MeshAnim.GetString(emote.Anim))) { this.mMesh.AnimMgr.SetAnims(MeshAnim.GetString(emote.Anim)); }
                            else { this.mMesh.AnimMgr.DeleteAnims(MeshAnim.GetString(emote.Anim)); }
                        }
                    }
                }
            }

            if (this.mCameraMgr.IsAllowedToMoveCam)
            {
                if (this.mInput.WasMouseButtonReleased(MOIS.MouseButtonID.MB_Left)) { this.mShootCube.Burst(); }
                if (this.mInput.IsMouseButtonDown(MOIS.MouseButtonID.MB_Left))
                {
                    if ((int) this.HUD.Selector <= 2)
                    {
                        string material = "";
                        switch (this.HUD.Selector)
                        {
                            case HUD.Selection.FireCube:
                                material = "fireball";
                                break;
                            case HUD.Selection.WaterCube:
                                material = "waterball";
                                break;
                            case HUD.Selection.MagicCube:
                                material = "magicball";
                                break;

                        }
                        this.mShootCube.Material = material;
                        this.mShootCube.Grow(frameTime, this.mInput.WasMouseButtonPressed(MOIS.MouseButtonID.MB_Left));
                    }
                }
            }
        }

        private void FirstPersonUpdate(float yawValue, float pitchValue)
        {
            Vector3 moveDirection = new Vector3();
            if (this.mInput.IsKeyDown(MOIS.KeyCode.KC_W) || this.mInput.IsKeyDown(MOIS.KeyCode.KC_UP)) { moveDirection.z = 1; }
            if (this.mInput.IsKeyDown(MOIS.KeyCode.KC_S) || this.mInput.IsKeyDown(MOIS.KeyCode.KC_DOWN)) { moveDirection.z = -1; }
            if (this.mInput.IsKeyDown(MOIS.KeyCode.KC_A) || this.mInput.IsKeyDown(MOIS.KeyCode.KC_LEFT)) { moveDirection.x = 1; }
            if (this.mInput.IsKeyDown(MOIS.KeyCode.KC_D) || this.mInput.IsKeyDown(MOIS.KeyCode.KC_RIGHT)) { moveDirection.x = -1; }
            if (this.mInput.WasKeyPressed(MOIS.KeyCode.KC_SPACE)) { moveDirection.y = 1; }
            this.MovementInfo.MoveDirection = moveDirection;

            this.MovementInfo.YawValue = yawValue;
            this.MovementInfo.PitchValue = pitchValue;
        }

        private void ThirdPersonUpdate(float yawValue, float pitchValue) { }
    }
}
