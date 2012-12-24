using System;
using System.Collections.Generic;
using Mogre;

namespace Game.CharacSystem
{
    /* Mother class of Player and NonPlayer */
    public abstract class Character
    {
        private const float WALK_SPEED = 200.0f;

        protected SceneNode mNode;
        protected AnimationMgr mAnimMgr;
        protected float mHeight;  // Height of the mesh use to place the camera at head level
        protected Vector3 mFeetPosition;  // Notice that Node.Position is at hip level
        protected CharacterInfo mCharInfo;
        protected MovementInfo mMovementInfo;
        protected Anim[] mIdleAnims;
        protected Anim[] mRunAnims;

        public SceneNode Node { get { return this.mNode; } }
        public float Height   { get { return this.mHeight; } }
        public bool IsMoving  { get { return this.mMovementInfo.IsMoving; } }
        public Vector3 FeetPosition
        {
            get { return this.mFeetPosition; }
            set
            {
                this.mFeetPosition = value;
                this.mNode.SetPosition(this.mFeetPosition.x, this.mFeetPosition.y + this.mHeight / 2 + 5, this.mFeetPosition.z);
            }
        }

        protected Character(SceneManager sceneMgr, string meshName, CharacterInfo charInfo)
        {
            Entity playerEnt = sceneMgr.CreateEntity("CharacterEnt_" + charInfo.Id, meshName);
            playerEnt.Skeleton.BlendMode = SkeletonAnimationBlendMode.ANIMBLEND_CUMULATIVE;

            this.mNode = sceneMgr.RootSceneNode.CreateChildSceneNode("CharacterNode_" + charInfo.Id);
            this.mNode.AttachObject(playerEnt);
            this.mNode.Scale(9, 10, 9);
            this.mHeight = playerEnt.BoundingBox.Size.y * this.mNode.GetScale().y;

            this.mIdleAnims = new Anim[] { Anim.IdleBase, Anim.IdleTop };
            this.mRunAnims = new Anim[] { Anim.RunBase, Anim.RunTop };
            this.mAnimMgr = new AnimationMgr(playerEnt.AllAnimationStates);
            this.mAnimMgr.SetAnims(this.mIdleAnims);

            this.mCharInfo = charInfo;
            this.mMovementInfo = new MovementInfo();

            this.FeetPosition = this.mCharInfo.SpawnPoint;
        }

        public virtual void Update(float frameTime)
        {
            if (this.mMovementInfo.IsMoving)
            {
                this.mNode.Translate(WALK_SPEED * frameTime * this.mMovementInfo.MoveDirection, Mogre.Node.TransformSpace.TS_LOCAL);
                this.mNode.Yaw(this.mMovementInfo.YawValue);
            }

            if (this.mAnimMgr.CurrentAnims.Count == 0) { this.mAnimMgr.SetAnims(this.mIdleAnims); }

            this.mAnimMgr.Update(frameTime);
            this.mMovementInfo.ClearInfo();
        }
    }
}
