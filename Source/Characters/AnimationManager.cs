using System;
using System.Collections.Generic;
using Mogre;

using Game.LibNoise;

namespace Game.CharacSystem
{
    public enum Anim : byte { IdleBase, IdleTop, RunBase, RunTop, JumpStart, JumpLoop, JumpEnd, Dance }
    
    class AnimationManager
    {
        private const float FADE_SPEED = 7.5f;

        private AnimationStateSet mAnimSet;
        private List<Anim> mCurrentAnims;
        private List<Anim> mFadingIn;
        private List<Anim> mFadingOut;
        private Anim[] mDefaultAnims;

        public List<Anim> CurrentAnims { get { return this.mCurrentAnims; } }

        public AnimationManager(AnimationStateSet animSet)
        {
            this.mAnimSet = animSet;

            string[] animNames = Enum.GetNames(typeof(Anim));
            foreach (string animName in animNames)
            {
                if (animName == this.AnimToName(Anim.JumpStart) || animName == this.AnimToName(Anim.JumpEnd))
                    continue;

                this.mAnimSet.GetAnimationState(animName).Loop = true;
            }

            this.mCurrentAnims = new List<Anim>();
            this.mFadingIn = new List<Anim>();
            this.mFadingOut = new List<Anim>();
            this.mDefaultAnims = new Anim[] { Anim.IdleBase, Anim.IdleTop };

            //this.FadeIn(Anim.IdleBase); this.FadeIn(Anim.IdleTop);
            this.AddAnim(mDefaultAnims);
        }

        public void AddAnim(params Anim[] anims)
        {
            foreach (Anim anim in anims)
            {
                if (this.mCurrentAnims.Contains(anim))
                    continue;

                //this.FadeIn(anim);
                AnimationState animState = this.mAnimSet.GetAnimationState(this.AnimToName(anim));
                animState.Enabled = true;
                animState.Weight = 0;
                this.mFadingIn.Add(anim);
                this.mFadingOut.Remove(anim);
                this.mCurrentAnims.Add(anim);

                if (!this.IsDefaultAnim(anim))  // && this.mCurrentAnims.TrueForAll(delegate(Anim currentAnim) { return true; })
                    this.DeleteAnim(mDefaultAnims);
            }
        }

        private bool IsDefaultAnim(Anim anim)
        {
            foreach (Anim defaultAnim in this.mDefaultAnims)
                if (anim == defaultAnim)
                    return true;

            return false;
        }

        /*private void FadeIn(Anim anim)
        {
            AnimationState animState = this.mAnimSet.GetAnimationState(this.AnimToName(anim));
            animState.Enabled = true;
            animState.Weight = 0;
            this.mFadingIn.Add(anim);
            this.mCurrentAnims.Add(anim);
        }*/

        public void DeleteAnim(params Anim[] anims)
        {
            foreach (Anim anim in anims)
            {
                if (this.mCurrentAnims.Contains(anim))
                {
                    this.mFadingOut.Add(anim);
                    this.mFadingIn.Remove(anim);
                    this.mCurrentAnims.Remove(anim);
                }
            }

            if (this.mCurrentAnims.Count == 0) { this.AddAnim(Anim.IdleBase, Anim.IdleTop); }
        }

        public void DeleteAllAnims()
        {
            this.DeleteAnim(this.mCurrentAnims.ToArray());
        }

        private string AnimToName(Anim anim)
        {
            return Enum.GetName(typeof(Anim), anim);
        }

        public void Update(float frameTime)
        {
            this.FadeAnimations(frameTime);

            foreach (Anim anim in this.mCurrentAnims)
                this.mAnimSet.GetAnimationState(this.AnimToName(anim)).AddTime(frameTime);
        }

        private void FadeAnimations(float frameTime)
        {
            List<Anim> namesToDelete = new List<Anim>();

            foreach (Anim anim in this.mFadingIn)    // Slowly fade an animation until it has full weight
            {
                AnimationState animState = this.mAnimSet.GetAnimationState(this.AnimToName(anim));
                float newWeight = animState.Weight + frameTime * FADE_SPEED;
                animState.Weight = MathHelper.clamp<float>(newWeight, 0, 1);   // Be sure the new weight is between 0 and 1
                if (newWeight >= 1) { namesToDelete.Add(anim); }  // Don't neet to fade an anmiation with weight == 1 anymore
            }

            foreach (Anim anim in namesToDelete)  // Delete animations which don't need to be fade
                this.mFadingIn.Remove(anim);

            namesToDelete.Clear();

            foreach (Anim anim in this.mFadingOut)  // Slowly fade an animation out until it has no weight, and then disable it
            {
                AnimationState animState = this.mAnimSet.GetAnimationState(this.AnimToName(anim));
                float newWeight = animState.Weight - frameTime * FADE_SPEED;
                animState.Weight = MathHelper.clamp<float>(newWeight, 0, 1);   // Be sure the weight is between 0 and 1
                if (newWeight <= 0)
                {
                    namesToDelete.Add(anim);  // Don't neet to fade an anmiation out with weight == 0 anymore
                    animState.Enabled = false;
                }
            }

            foreach (Anim anim in namesToDelete)
                this.mFadingOut.Remove(anim);
        }
    }
}
