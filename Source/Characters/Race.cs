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

        public Race(OgreManager engine, string meshName)
        {
            mNode = engine.CreateSimpleObject("MainPlayer", meshName);
            mNode.Scale(17, 17, 17);
            engine.SceneMgr.GetEntity("MainPlayer").Skeleton.BlendMode = SkeletonAnimationBlendMode.ANIMBLEND_CUMULATIVE;
            //mListAnim = mEnt.AllAnimationStates;

            engine.AddObjectToScene(mNode);
        }

        /* Play the animation(s) corresponding to an action */
        public void makeAction(Action act)
        {

        }
    }
}
