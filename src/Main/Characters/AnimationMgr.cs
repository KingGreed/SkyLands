using System;
using System.Collections.Generic;
using Mogre;

namespace Game.CharacSystem
{
    public class Anim
    {
        private AnimName mName;
        private float mTimeFactor;

        public AnimName Name { get { return this.mName; } }

        public Anim(AnimName name, float timeFactor = 1)
        {
            this.mName = name;
            this.mTimeFactor = timeFactor;
        }
    }

    public enum AnimName : byte { IdleBase, IdleTop, RunBase, RunTop, JumpStart, JumpLoop, JumpEnd, Dance }

    public class AnimationMgr
    {
        private const float FADE_SPEED = 7.5f;

        private AnimationState[] mAnimStates;   // All the AnimationState available
        private List<Anim> mWantedAnims;        // Animations asked by the input or AI
        private List<AnimName> mNonLoopedAnimations;

        public List<Anim> CurrentAnims { get { return this.mWantedAnims; } }
        public AnimName[] AllAnims { get { return (AnimName[])Enum.GetValues(typeof(AnimName)); } }

        public AnimationMgr(AnimationStateSet animSet)
        {
            this.mAnimStates = new AnimationState[Enum.GetNames(typeof(AnimName)).Length];
            this.mNonLoopedAnimations = new List<AnimName>() { AnimName.JumpStart, AnimName.JumpEnd };
            for (int i = 0; i < this.mAnimStates.Length; i++)
            {
                this.mAnimStates[i] = animSet.GetAnimationState(Enum.GetName(typeof(AnimName), (AnimName)i));   // Stores all the AnimationState

                // All the animation excepted JumpStart and JumpLoop are looped
                this.mAnimStates[i].Loop = !this.mNonLoopedAnimations.Contains((AnimName)i);
            }

            this.mWantedAnims = new List<Anim>();
        }

        public void SetAnims(params AnimName[] names)                   { this.SetAnims<AnimName[]>(names, 1); }
        public void SetAnims(float timeFactor, params AnimName[] names) { this.SetAnims<AnimName[]>(names, timeFactor); }
        public void SetAnims<T>(T names, float timeFactor) where T : IEnumerable<AnimName>
        {
            this.DeleteAllAnims();
            this.AddAnims(names, timeFactor);
        }

        public void AddAnims(params AnimName[] names)                    { this.AddAnims<AnimName[]>(names, 1); }
        public void AddAnims(float timeFactor, params AnimName[] names)  { this.AddAnims<AnimName[]>(names, timeFactor); }
        public void AddAnims<T>(T names, float timeFactor) where T : IEnumerable<AnimName>
        {
            foreach (AnimName name in names)
            {
                if (!this.AreAnimationsPlaying(name))
                {
                    this.mWantedAnims.Add(new Anim(name, timeFactor));
                    this.mAnimStates[(int)name].Enabled = true;
                    if (this.mNonLoopedAnimations.Contains(name))
                        this.mAnimStates[(int)name].TimePosition = 0;
                }
            }
        }

        public void DeleteAnims(params AnimName[] names) { this.DeleteAnims<AnimName[]>(names); }
        public void DeleteAnims<T>(T names) where T : IEnumerable<AnimName>
        {
            this.mWantedAnims.RemoveAll(delegate(Anim anim)
            {
                foreach (AnimName name in names)
                    if (anim.Name == name)
                        return true;
                return false;
            });
        }

        public void DeleteAllExcept(params AnimName[] names) { this.DeleteAllExcept<AnimName[]>(names); }
        public void DeleteAllExcept<T>(params T[] names) where T : IEnumerable<AnimName>
        {
            List<AnimName> delete = new List<AnimName>(this.AllAnims);
            foreach(T namesList in names)
                foreach (AnimName name in namesList)
                    delete.Remove(name);

            this.DeleteAnims(delete);
        }

        public void DeleteAllAnims() { this.mWantedAnims.Clear(); }

        public bool AreAnimationsPlaying(params AnimName[] names) { return this.AreAnimationsPlaying<AnimName[]>(names); }
        public bool AreAnimationsPlaying<T>(T names) where T : IEnumerable<AnimName>
        {
            foreach(AnimName name in names)
                foreach (Anim anim in this.mWantedAnims)
                    if (anim.Name == name)
                        return true;

            return false;
        }

        public void Update(float frameTime)
        {
            this.FadeAnimations(frameTime);

            foreach (Anim anim in this.mWantedAnims)
                this.mAnimStates[(int)anim.Name].AddTime(frameTime);
        }

        private void FadeAnimations(float frameTime)
        {
            for (int i = 0; i < this.mAnimStates.Length; i++)
            {
                if (this.AreAnimationsPlaying((AnimName)i))
                {
                    if (this.mNonLoopedAnimations.Contains((AnimName)i) && this.mAnimStates[i].HasEnded)
                        this.DeleteAnims((AnimName)i);

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
