using System;
using System.Collections.Generic;
using Mogre;

namespace Game.CharacSystem
{
    public class Player : Character
    {
        private struct Emote
        {
            private Anim mAnim;
            private MOIS.KeyCode mKey;

            public Anim Anim        { get { return this.mAnim; } }
            public MOIS.KeyCode Key { get { return this.mKey; } }

            public Emote(MOIS.KeyCode key, Anim anim) { this.mKey = key; this.mAnim = anim; }
        }
        
        private MoisManager mInput;
        private Emote[] mEmotes;
        private float mYawCamValue;
        private float mPitchCamValue;
        private bool mIsFirstView;
        private bool mIsDebugMode;

        public MoisManager Input   { get { return this.mInput; } }
        public float YawCamValue   { get { return this.mYawCamValue; } }
        public float PitchCamValue { get { return this.mPitchCamValue; } }
        public bool IsFirstView    { get { return this.mIsFirstView; } }
        public bool IsDebugMode
        { 
            get { return this.mIsDebugMode; }
            set
            {
                this.mIsDebugMode = value;
                this.mNode.SetVisible(this.mIsDebugMode);
            }
        }

        public Player(SceneManager sceneMgr, string meshName, CharacterInfo info, MoisManager input) : base(sceneMgr, meshName, info, true)
        {
            this.mInput = input;
            this.mIsFirstView = true;

            this.mEmotes = new Emote[]
            {
                new Emote(MOIS.KeyCode.KC_1, Anim.Dance)
            };
        }

        public new void Update(float frameTime)
        {
            bool isNowMoving = !this.mIsDebugMode || this.mInput.IsOneKeyEventTrue(this.mInput.IsKeyDown, MOIS.KeyCode.KC_LCONTROL, MOIS.KeyCode.KC_RCONTROL);
            if (this.mMovementInfo.IsMoving && !isNowMoving) { this.mAnimMgr.DeleteAllAnims(); }
            this.mMovementInfo.IsMoving = isNowMoving;

            if (this.mMovementInfo.IsMoving)
            {
                float yawValue = -this.mInput.MouseMoveX * CharacMgr.YAW_SENSIVITY;
                float pitchValue = -this.mInput.MouseMoveY * CharacMgr.PITCH_SENSIVITY;

                if (this.mIsFirstView) { this.FirstPersonUpdate(yawValue, pitchValue); }
                else { this.ThirdPersonUpdate(yawValue, pitchValue); }

                if (this.mInput.WasKeyPressed(MOIS.KeyCode.KC_SPACE))                                                 { this.mAnimMgr.SetAnims(Anim.JumpStart); }
                if (this.mInput.IsOneKeyEventTrue(this.mInput.WasKeyPressed, MOIS.KeyCode.KC_W, MOIS.KeyCode.KC_UP))  { this.mAnimMgr.SetAnims(this.mRunAnims); }
                if (this.mInput.IsOneKeyEventTrue(this.mInput.WasKeyReleased, MOIS.KeyCode.KC_W, MOIS.KeyCode.KC_UP)) { this.mAnimMgr.DeleteAnims(this.mRunAnims); }

                if (!this.mAnimMgr.AreAnimationsPlaying(Anim.JumpStart, Anim.JumpLoop, Anim.JumpEnd, Anim.RunBase, Anim.RunTop))
                {
                    foreach (Emote emote in this.mEmotes)
                    {
                        if (this.mInput.WasKeyPressed(emote.Key))
                        {
                            if (!this.mAnimMgr.AreAnimationsPlaying(emote.Anim)) { this.mAnimMgr.SetAnims(emote.Anim); }
                            else                                                 { this.mAnimMgr.DeleteAnims(emote.Anim); }
                        }
                    }
                }
            }
        }

        private void FirstPersonUpdate(float yawValue, float pitchValue)
        {
            Vector3 moveDirection = new Vector3();
            if (this.mInput.IsKeyDown(MOIS.KeyCode.KC_W) || this.mInput.IsKeyDown(MOIS.KeyCode.KC_UP))    { moveDirection.z = 1; }
            if (this.mInput.IsKeyDown(MOIS.KeyCode.KC_S) || this.mInput.IsKeyDown(MOIS.KeyCode.KC_DOWN))  { moveDirection.z = -1; }
            if (this.mInput.IsKeyDown(MOIS.KeyCode.KC_A) || this.mInput.IsKeyDown(MOIS.KeyCode.KC_LEFT))  { moveDirection.x = 1; }
            if (this.mInput.IsKeyDown(MOIS.KeyCode.KC_D) || this.mInput.IsKeyDown(MOIS.KeyCode.KC_RIGHT)) { moveDirection.x = -1; }
            this.mMovementInfo.MoveDirection = moveDirection;

            this.mMovementInfo.YawValue = yawValue;

            this.mYawCamValue = 0;
            this.mPitchCamValue = pitchValue;
        }

        private void ThirdPersonUpdate(float yawValue, float pitchValue) { }
    }
}
