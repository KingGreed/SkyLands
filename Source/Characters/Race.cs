using System;
using System.Collections.Generic;
using Mogre;

namespace Game.CharacSystem
{
    class Race
    {
        public enum Action { idle, run, jump, dance }

        protected SceneNode mNode;
        //public AnimationState mAnimState;

        public SceneNode Node
        {
            get { return mNode; }
        }

        public Race(SceneManager sceneMgr, string meshName)
        {
            Entity sinbad = sceneMgr.CreateEntity("MainPlayer", meshName);

            mNode = sceneMgr.RootSceneNode.CreateChildSceneNode("PlayerNd");
            mNode.AttachObject(sinbad);

            mNode.Scale(17, 17, 17);
            sinbad.Skeleton.BlendMode = SkeletonAnimationBlendMode.ANIMBLEND_CUMULATIVE;

            sinbad.GetAnimationState("IdleBase").Enabled = true;
            sinbad.GetAnimationState("IdleBase").Loop    = true;

            sinbad.GetAnimationState("IdleTop").Enabled  = true;
            sinbad.GetAnimationState("IdleTop").Loop     = true;

            //mListAnim = mEnt.AllAnimationStates;

        }

        /* Play the animation(s) corresponding to an action */
        public void makeAction(Action act)
        {

        }
    }
}
