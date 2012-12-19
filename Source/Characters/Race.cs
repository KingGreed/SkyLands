using System;
using System.Collections.Generic;
using Mogre;

using Game.LibNoise;


namespace Game.CharacSystem
{
    class Race
    {
        private const float WALK_SPEED = 200.0f;
        private const float FADE_SPEED = 7.5f;        
        
        public enum AnimType { none, idle, run, jump, fall, land, dance }
        private SceneNode mNode;
        private AnimationStateSet mAnimSet;
        private string[] mAnimNames;
        private List<string> mFadingIn;
        private List<string> mFadingOut;
        private AnimType mCurrentAnim;
        private float mHeight;  // Height of the mesh use to place the camera at head level
        private Vector3 mFeetPosition;  // Notice that Node.Position is at hip level

        public AnimType CurrentAnim { get { return this.mCurrentAnim; } }
        public SceneNode Node       { get { return this.mNode; } }
        public float Height         { get { return this.mHeight; } }
        public Vector3 FeetPosition
        {
            get { return this.mFeetPosition; }
            set
            {
                this.mFeetPosition = value;
                this.mNode.SetPosition(this.mFeetPosition.x, this.mFeetPosition.y + this.mHeight / 2 + 5, this.mFeetPosition.z);
            }
        }

        public Race(SceneManager sceneMgr, string meshName)
        {
            Entity playerEnt = sceneMgr.CreateEntity("MainPlayer", meshName);

            this.mNode = sceneMgr.RootSceneNode.CreateChildSceneNode("PlayerNd");
            this.mNode.AttachObject(playerEnt);
            this.mNode.Scale(9, 10, 9);
            this.mHeight = playerEnt.BoundingBox.Size.y * this.mNode.GetScale().y;

            playerEnt.Skeleton.BlendMode = SkeletonAnimationBlendMode.ANIMBLEND_CUMULATIVE;
            this.mAnimSet = playerEnt.AllAnimationStates;
            this.mAnimNames = new string[] { "IdleBase", "IdleTop", "RunBase", "RunTop", "JumpStart", "JumpLoop", "JumpEnd", "Dance" };

            for(int i = 0; i < mAnimNames.Length; i++)
            {
                bool isLooped = true;
                if (i == 4 || i == 6)   // JumpStart and JumpEnd aren't looped
                    isLooped = false;

                this.mAnimSet.GetAnimationState(this.mAnimNames[i]).Loop = isLooped;
            }

            this.mFadingIn = new List<string>();
            this.mFadingOut = new List<string>();

            this.mCurrentAnim = AnimType.none;
            this.ChangeAnimation(AnimType.idle);
        }

        public void ChangeAnimation(AnimType anim)
        {
            foreach (string animName in TypeToNames(anim))
                this.FadeInAnim(animName);

            foreach (string animName in TypeToNames(this.mCurrentAnim))
                this.mFadingOut.Add(animName);

            this.mCurrentAnim = anim;
        }

        private List<string> TypeToNames(AnimType anim)
        {
            List<string> names = new List<string>();

            if (anim == AnimType.idle)       { names.Add(this.mAnimNames[0]); names.Add(this.mAnimNames[1]); }
            else if (anim == AnimType.run)   { names.Add(this.mAnimNames[2]); names.Add(this.mAnimNames[3]); }
            else if (anim == AnimType.jump)  { names.Add(this.mAnimNames[4]); }
            else if (anim == AnimType.fall)  { names.Add(this.mAnimNames[5]); }
            else if (anim == AnimType.land)  { names.Add(this.mAnimNames[6]); }
            else if (anim == AnimType.dance) { names.Add(this.mAnimNames[7]); }

            return names;   // Return an empty list if anim == AnimType.none
        }

        public void Update(float frameTime, MovementInfo info)
        {
            this.mNode.Translate(WALK_SPEED * frameTime * info.MoveDirection, Mogre.Node.TransformSpace.TS_LOCAL);

            this.mNode.Yaw(info.YawValue);
            // TO DO : add animation for that
            
            this.UpdateAnimation(frameTime, info);
        }

        private void UpdateAnimation(float frameTime, MovementInfo info)
        {
            if (info.MoveDirection.z != 0 && this.mCurrentAnim != Race.AnimType.run)
                this.ChangeAnimation(Race.AnimType.run);
            if (info.MoveDirection == Vector3.ZERO && this.CurrentAnim != Race.AnimType.idle)
                this.ChangeAnimation(Race.AnimType.idle);
            
            this.FadeAnimations(frameTime);
            
            foreach (string animName in this.mAnimNames)
            {
                if (this.mAnimSet.GetAnimationState(animName).Enabled)
                    this.mAnimSet.GetAnimationState(animName).AddTime(frameTime);
            }
        }

        private void FadeInAnim(string name)
        {
            this.mAnimSet.GetAnimationState(name).Enabled = true;
            this.mAnimSet.GetAnimationState(name).Weight = 0;
            this.mFadingIn.Add(name);
        }

        private void FadeAnimations(float frameTime)
        {
            List<string> namesToDelete = new List<string>();
            
            foreach (string name in mFadingIn)    // Slowly fade an animation until it has full weight
            {
                float newWeight = this.mAnimSet.GetAnimationState(name).Weight + frameTime * FADE_SPEED;
                this.mAnimSet.GetAnimationState(name).Weight = MathHelper.clamp<float>(newWeight, 0, 1);   // Be sure the new weight is between 0 and 1
                if (newWeight >= 1) { namesToDelete.Add(name); }  // Don't neet to fade an anmiation with weight == 1 anymore
            }

            foreach (string name in namesToDelete)  // Delete animations which don't need to be fade
                this.mFadingIn.Remove(name);

            namesToDelete.Clear();

            foreach (string name in mFadingOut)  // Slowly fade an animation out until it has no weight, and then disable it
            {
                float newWeight = this.mAnimSet.GetAnimationState(name).Weight - frameTime * FADE_SPEED;
                this.mAnimSet.GetAnimationState(name).Weight = MathHelper.clamp<float>(newWeight, 0, 1);   // Be sure the weight is between 0 and 1
                if (newWeight <= 0)
                {
                    namesToDelete.Add(name);  // Don't neet to fade an anmiation out with weight == 0 anymore
                    this.mAnimSet.GetAnimationState(name).Enabled = false;
                }
            }

            foreach (string name in namesToDelete)
                this.mFadingOut.Remove(name);
        }
    }
}
