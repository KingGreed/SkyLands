using System;
using System.Collections.Generic;
using Mogre;

namespace Game.Animation
{
    public class AnimationMgr
    {
        private const float FADE_SPEED = 7.5f;

        private AnimationState[] mAnimStates;   // All the AnimationState available
        private List<Anim>       mWantedAnims;        // Animations asked by the input or AI
        private List<string>     mNonLoopedAnimations;
        private List<string>     mAllAnims;

        public List<Anim> CurrentAnims { get { return this.mWantedAnims; } }
        public List<string> AllAnims  { get { return this.mAllAnims; } }

        public AnimationMgr(AnimationStateSet animSet, string[] animsName, string[] nonLoopedAnims)
        {
            this.mAllAnims = new List<string>(animsName);
            this.mAnimStates = new AnimationState[this.mAllAnims.Count];
            this.mNonLoopedAnimations = new List<string>(nonLoopedAnims);
            for (int i = 0; i < this.mAnimStates.Length; i++)
            {
                this.mAnimStates[i] = animSet.GetAnimationState(this.mAllAnims[i]);   // Stores all the AnimationState

                // All the animation excepted JumpStart and JumpLoop are looped
                this.mAnimStates[i].Loop = !this.mNonLoopedAnimations.Contains(this.mAllAnims[i]);
            }

            this.mWantedAnims = new List<Anim>();
        }

        public void SetAnims(params string[] names)                   { this.SetAnims<string[]>(names, 1); }
        public void SetAnims(float timeFactor, params string[] names) { this.SetAnims<string[]>(names, timeFactor); }
        public void SetAnims<T>(T names, float timeFactor) where T : IEnumerable<string>
        {
            this.DeleteAllAnims();
            this.AddAnims(names, timeFactor);
        }
        public void SetAnims(params Anim[] anims)
        {
            this.DeleteAllAnims();
            this.AddAnims(anims);
        }

        public void AddAnims(params string[] names)                    { this.AddAnims<string[]>(names, 1); }
        public void AddAnims(float timeFactor, params string[] names)  { this.AddAnims<string[]>(names, timeFactor); }
        public void AddAnims<T>(T names, float timeFactor) where T : IEnumerable<string>
        {
            foreach (string name in names)
            {
                if (!this.AreAnimationsPlaying(name))
                {
                    this.mWantedAnims.Add(new Anim(name, timeFactor));
                    this.mAnimStates[this.GetIndex(name)].Enabled = true;
                    if (this.mNonLoopedAnimations.Contains(name))
                        this.mAnimStates[this.GetIndex(name)].TimePosition = 0;
                }
            }
        }
        public void AddAnims(params Anim[] anims)
        {
            foreach (Anim anim in anims)
            {
                if (!this.AreAnimationsPlaying(anim.Name))
                {
                    this.mWantedAnims.Add(anim);
                    this.mAnimStates[this.GetIndex(anim.Name)].Enabled = true;
                    if (this.mNonLoopedAnimations.Contains(anim.Name))
                        this.mAnimStates[this.GetIndex(anim.Name)].TimePosition = 0;
                }
            }
        }

        public void DeleteAnims(params string[] names) { this.DeleteAnims<string[]>(names); }
        public void DeleteAnims<T>(T names) where T : IEnumerable<string>
        {
            this.mWantedAnims.RemoveAll(delegate(Anim anim)
            {
                foreach (string name in names)
                    if (anim.Name == name)
                        return true;
                return false;
            });
        }

        public void DeleteAllExcept(params string[] names) { this.DeleteAllExcept<string[]>(names); }
        public void DeleteAllExcept<T>(params T[] names) where T : IEnumerable<string>
        {
            List<string> delete = new List<string>(this.AllAnims);
            foreach (T namesList in names)
                foreach (string name in namesList)
                    delete.Remove(name);

            this.DeleteAnims(delete);
        }

        public void DeleteAllAnims() { this.mWantedAnims.Clear(); }

        public bool AreAnimationsPlaying(params string[] names) { return this.AreAnimationsPlaying<string[]>(names); }
        public bool AreAnimationsPlaying<T>(T names) where T : IEnumerable<string>
        {
            foreach(string name in names)
                foreach (Anim anim in this.mWantedAnims)
                    if (anim.Name == name)
                        return true;

            return false;
        }

        public void Update(float frameTime)
        {
            this.FadeAnimations(frameTime);

            foreach (Anim anim in this.mWantedAnims)
                this.mAnimStates[this.GetIndex(anim.Name)].AddTime(frameTime * anim.TimeFactor);
        }

        private int GetIndex(string anim)
        {
            return this.mAllAnims.IndexOf(anim);
        }

        private void FadeAnimations(float frameTime)
        {
            for (int i = 0; i < this.mAnimStates.Length; i++)
            {
                if (this.AreAnimationsPlaying(this.mAllAnims[i]))
                {
                    if (this.mNonLoopedAnimations.Contains(this.mAllAnims[i]) && this.mAnimStates[i].HasEnded)
                    {
                        Anim anim = this.mWantedAnims.Find(a => a.Name == this.mAllAnims[i]);
                        if (!anim.KeepActive) { this.DeleteAnims(this.mAllAnims[i]); }
                    }

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
