using System;
using System.Collections.Generic;
using Mogre;

namespace Game.CharacSystem
{
    class Race
    {
        public enum AnimType { idle, run, jump, fall, land, dance }
        private SceneNode mNode;
        private AnimationStateSet mAnimSet;
        private string[] mAnimNames;

        public SceneNode Node { get { return mNode; } }

        public Race(SceneManager sceneMgr, string meshName)
        {
            Entity sinbad = sceneMgr.CreateEntity("MainPlayer", meshName);

            this.mNode = sceneMgr.RootSceneNode.CreateChildSceneNode("PlayerNd");
            this.mNode.AttachObject(sinbad);
            this.mNode.Scale(17, 17, 17);

            sinbad.Skeleton.BlendMode = SkeletonAnimationBlendMode.ANIMBLEND_CUMULATIVE;
            this.mAnimSet = sinbad.AllAnimationStates;
            this.mAnimNames = new string[] { "IdleBase", "IdleTop", "RunBase", "RunTop", "JumpStart", "JumpLoop", "JumpEnd", "Dance" };

            for(int i = 0; i < mAnimNames.Length; i++)
            {
                bool isLooped = true;
                if (i == 4 || i == 6)   // JumpStart and JumpEnd aren't looped
                    isLooped = false;

                this.mAnimSet.GetAnimationState(this.mAnimNames[i]).Loop = isLooped;
            }

            this.ChangeAnimation(AnimType.dance);
        }

        public void ChangeAnimation(AnimType anim)
        {
            foreach (string name in mAnimNames) // Turn all animations off
                this.mAnimSet.GetAnimationState(name).Enabled = false;

            List<int> listAnimToEnable = new List<int>();

            if (anim == AnimType.run)        { listAnimToEnable.Add(2); listAnimToEnable.Add(3); }
            else if (anim == AnimType.jump)  { listAnimToEnable.Add(4); }
            else if (anim == AnimType.fall)  { listAnimToEnable.Add(5); }
            else if (anim == AnimType.land)  { listAnimToEnable.Add(6); }
            else if (anim == AnimType.dance) { listAnimToEnable.Add(7); }
            else                             { listAnimToEnable.Add(0); listAnimToEnable.Add(1); }

            this.EnableAnimation(listAnimToEnable);
        }

        private void EnableAnimation(List<int> listIndex)
        {
            foreach (int i in listIndex)
                this.mAnimSet.GetAnimationState(this.mAnimNames[i]).Enabled = true;
        }

        public void UpdateAnimation(float frameTime)
        {
            foreach (string animName in this.mAnimNames)
            {
                if (this.mAnimSet.GetAnimationState(animName).Enabled)
                    this.mAnimSet.GetAnimationState(animName).AddTime(frameTime);
            }
        }
    }
}
