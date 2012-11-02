using System;
using System.Collections.Generic;
using Mogre;

namespace Game.CharacSystem
{
    class Breed
    {
        public enum Action { idle, run, jump, dance }

        public Entity mEnt { get; protected set; }
        public SceneNode mNode { get; protected set; }
        //public AnimationState mAnimState;

        public Breed(SceneManager sceneMgr, string meshName)
        {
            mEnt = sceneMgr.CreateEntity(meshName);
            mEnt.Skeleton.BlendMode = SkeletonAnimationBlendMode.ANIMBLEND_CUMULATIVE;
            //mListAnim = mEnt.AllAnimationStates;

            mNode = sceneMgr.RootSceneNode.CreateChildSceneNode();
            mNode.AttachObject(mEnt);
        }

        /* Play the animation(s) corresponding to an action */
        public void makeAction(Action act)
        {

        }
    }
}
