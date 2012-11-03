using System;
using System.Collections.Generic;
using Mogre;

namespace Game.CharacSystem
{
<<<<<<< HEAD
    class Breed //sure it's a good name ?
    {
        public enum Action { idle, run, jump, dance }

        public Entity mEnt { get; protected set; }//planing to inherit the class ?
=======
    class Breed
    {
        public enum Action { idle, run, jump, dance }

        public Entity mEnt { get; protected set; }
>>>>>>> 5c3cc93d3dff01cb6a4c43c92d95002796543de9
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
