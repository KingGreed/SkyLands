using System;
using System.Collections.Generic;
using Mogre;

namespace Game.CharacSystem
{
    public enum Anim : byte { IdleBase, IdleTop, RunBase, RunTop, JumpStart, JumpLoop, JumpEnd, Dance }

    public class AnimationMgr
    {
        private const float FADE_SPEED = 7.5f;

        private AnimationState[] mAnimStates;   // All the AnimationState available
        private List<Anim> mWantedAnims;        // Animations asked by the input or AI 

        public List<Anim> CurrentAnims { get { return this.mWantedAnims; } }

        public AnimationMgr(AnimationStateSet animSet)
        {
            this.mAnimStates = new AnimationState[Enum.GetNames(typeof(Anim)).Length];
            for(int i = 0; i < this.mAnimStates.Length; i++)
            {
                this.mAnimStates[i] = animSet.GetAnimationState(Enum.GetName(typeof(Anim), (Anim)i));   // Stores all the AnimationState
                
                if ((Anim)i == Anim.JumpStart || (Anim)i == Anim.JumpEnd)
                    continue;

                this.mAnimStates[i].Loop = true;    // All the animation excepted JumpStart and JumpLoop are looped
            }

            this.mWantedAnims = new List<Anim>();
        }

        public void SetAnims(params Anim[] anims)
        {
            this.DeleteAllAnims();
            this.AddAnims(anims);
        }

        public void AddAnims(params Anim[] anims)
        {
            foreach (Anim anim in anims)
            {
                this.mWantedAnims.Add(anim);
                this.mAnimStates[(int)anim].Enabled = true;
            }
        }

        public void DeleteAnims(params Anim[] anims)
        {
            foreach (Anim anim in anims)
                this.mWantedAnims.Remove(anim);
        }

        public void DeleteAllAnims()
        {
            this.mWantedAnims.Clear();
        }

        public bool AreAnimationsPlaying(params Anim[] anims)
        {
            foreach(Anim anim in anims)
                if (!this.mWantedAnims.Contains(anim)) { return false; }

            return true;
        }

        public void Update(float frameTime)
        {
            this.FadeAnimations(frameTime);

            foreach (Anim anim in this.mWantedAnims)
                this.mAnimStates[(int) anim].AddTime(frameTime);
        }

        private void FadeAnimations(float frameTime)
        {
            for (int i = 0; i < this.mAnimStates.Length; i++)
            {
                if (this.mWantedAnims.Contains((Anim)i))
                {
                    if (this.mAnimStates[i].Weight >= 1) { continue; }

                    float newWeight = this.mAnimStates[i].Weight + FADE_SPEED * frameTime;
                    this.mAnimStates[i].Weight = MathHelper.clamp<float>(newWeight, 0, 1);
                }
                else
                {
                    if (this.mAnimStates[i].Weight <= 0) { continue; }

                    float newWeight = this.mAnimStates[i].Weight - FADE_SPEED * frameTime;
                    this.mAnimStates[i].Weight = MathHelper.clamp<float>(newWeight, 0, 1);
                    if (this.mAnimStates[i].Weight <= 0) { this.mAnimStates[i].Enabled = false; }
                }
            }
        }
    }
}
