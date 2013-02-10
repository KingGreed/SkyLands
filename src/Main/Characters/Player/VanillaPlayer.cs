using System;
using System.Collections.Generic;
using Mogre;

using Game.World;
using Game.Animation;

namespace Game.CharacSystem
{
    public class VanillaPlayer : VanillaCharacter
    {
        private struct Emote
        {
            private AnimName mAnim;
            private MOIS.KeyCode mKey;

            public AnimName Anim        { get { return this.mAnim; } }
            public MOIS.KeyCode Key { get { return this.mKey; } }

            public Emote(MOIS.KeyCode key, AnimName anim) { this.mKey = key; this.mAnim = anim; }
        }
        
        private MoisManager mInput;
        private Emote[]     mEmotes;
        private AnimName[]  mEmotesNames;
        private float       mYawCamValue;
        private float       mPitchCamValue;
        private bool        mIsFirstView;
        private bool        mIsDebugMode;

        public MoisManager Input         { get { return this.mInput; } }
        public float       YawCamValue   { get { return this.mYawCamValue; } }
        public float       PitchCamValue { get { return this.mPitchCamValue; } }
        public bool        IsFirstView   { get { return this.mIsFirstView; } }
        public bool        IsDebugMode
        { 
            get { return this.mIsDebugMode; }
            set
            {
                this.mIsDebugMode = value;
                this.mNode.SetVisible(this.mIsDebugMode);
            }
        }

        public VanillaPlayer(CharacMgr characMgr, string meshName, CharacterInfo info, MoisManager input) : base(characMgr, meshName, info)
        {
            this.mInput = input;
            this.mIsFirstView = true;

            this.mEmotes = new Emote[]
            {
                new Emote(MOIS.KeyCode.KC_1, AnimName.Dance)
            };
            this.mEmotesNames = new AnimName[this.mEmotes.Length];
            for (int i = 0; i < this.mEmotes.Length; i++)
                this.mEmotesNames[i] = this.mEmotes[i].Anim;
        }

        public new void Update(float frameTime)
        {
            bool isNowMoving = !this.mIsDebugMode || this.mInput.IsOneKeyEventTrue(this.mInput.IsKeyDown, MOIS.KeyCode.KC_LCONTROL, MOIS.KeyCode.KC_RCONTROL);
            if (this.mMovementInfo.IsMoving && !isNowMoving)
                this.mAnimMgr.DeleteAllExcept<AnimName[]>(this.mEmotesNames, this.mIdleAnims, this.mJumpAnims);
            this.mMovementInfo.IsMoving = isNowMoving;

            if (this.mMovementInfo.IsMoving)
            {
                float yawValue = -this.mInput.MouseMoveX * CharacMgr.YAW_SENSIVITY;
                float pitchValue = -this.mInput.MouseMoveY * CharacMgr.PITCH_SENSIVITY;

                if (this.mIsFirstView) { this.FirstPersonUpdate(yawValue, pitchValue); }
                else { this.ThirdPersonUpdate(yawValue, pitchValue); }

                /* Update emotes animations */
                if (!this.mAnimMgr.AreAnimationsPlaying(AnimName.JumpStart, AnimName.JumpLoop, AnimName.JumpEnd, AnimName.RunBase, AnimName.RunTop))
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
