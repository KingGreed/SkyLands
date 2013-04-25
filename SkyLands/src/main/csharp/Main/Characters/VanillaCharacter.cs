using System;
using System.Collections.Generic;
using System.Linq;
using Mogre;

using Game.World;
using Game.Animation;
using Game.Characters.IA;
using API.Ent;
using API.Geo.Cuboid;

namespace Game.CharacSystem
{
    public abstract class VanillaCharacter : Character
    {
        public static readonly Vector3 CHARAC_SIZE = new Vector3(65, 99, 65);
        private const float WALK_SPEED = 350.0f;

        protected CharacMgr     mCharacMgr;
        protected SceneNode     mNode;
        protected AnimationMgr  mAnimMgr;
        protected CharacterInfo mCharInfo;
        protected MovementInfo  mMovementInfo;
        protected AnimName[]    mRunAnims;
        protected AnimName[]    mJumpAnims;
        protected AnimName[]    mIdleAnims;
        private CollisionMgr    mCollisionMgr;
        private Vector3         mPreviousDirection;

        //MoveForward variable
        private Vector3         mDirection;
        private PathFinder      mPathFinder;
        private bool            mIsWalking;

        public SceneNode     Node            { get { return this.mNode; } }
        public bool          IsAllowedToMove { get { return this.mMovementInfo.IsAllowedToMove; } set { this.mMovementInfo.IsAllowedToMove = value; } }
        public float         Height          { get { return CHARAC_SIZE.y; } }
        public CharacterInfo Info            { get { return this.mCharInfo; } }
        public Vector3       FeetPosition
        {
            get         { return this.mNode.Position - new Vector3(0, this.Height / 2 + 8, 0); }
            private set { this.mNode.SetPosition(value.x, value.y + this.Height / 2 + 8, value.z); }
        }

        public Vector3 BlockPosition
        {
            get { return this.mCharacMgr.World.getRelativeFromAbsolute(this.FeetPosition); }
        }

        protected VanillaCharacter(CharacMgr characMgr, string meshName, CharacterInfo charInfo)
        {
            this.mCharacMgr = characMgr;
            this.mCharInfo = charInfo;
            this.mMovementInfo = new MovementInfo(OnFall, OnJump);
            this.mPreviousDirection = Vector3.ZERO;

            /* Create entity and node */
            SceneManager sceneMgr = characMgr.SceneMgr;
            Mogre.Entity ent = sceneMgr.CreateEntity("CharacterEnt_" + this.mCharInfo.Id, meshName);
            ent.Skeleton.BlendMode = SkeletonAnimationBlendMode.ANIMBLEND_CUMULATIVE;
            Mogre.Entity swordL = sceneMgr.CreateEntity("Sword.mesh");
            ent.AttachObjectToBone("Sheath.L", swordL);
            Mogre.Entity swordR = sceneMgr.CreateEntity("Sword.mesh");
            ent.AttachObjectToBone("Sheath.R", swordR);

            this.mNode = sceneMgr.RootSceneNode.CreateChildSceneNode("CharacterNode_" + this.mCharInfo.Id);
            this.mCollisionMgr = new CollisionMgr(sceneMgr, this.mCharacMgr.World, this);

            this.mDirection = Vector3.ZERO;
            //this.mDist          = 0;
            this.mIsWalking     = false;

            this.mNode.AttachObject(ent);
            this.FeetPosition = this.mCharInfo.SpawnPoint;


            /* Create Animations */
            this.mIdleAnims = new AnimName[] { AnimName.IdleBase, AnimName.IdleTop };
            this.mRunAnims  = new AnimName[] { AnimName.RunBase, AnimName.RunTop };
            this.mJumpAnims = new AnimName[] { AnimName.JumpStart, AnimName.JumpLoop, AnimName.JumpEnd };
            this.mAnimMgr   = new AnimationMgr(ent.AllAnimationStates);
            this.mAnimMgr.SetAnims(this.mIdleAnims);

            this.mNode.Scale(CHARAC_SIZE / ent.BoundingBox.Size);
        }

        private void OnFall(bool isFalling)
        {
            if (isFalling)
            {
                this.mAnimMgr.SetAnims(AnimName.JumpLoop);
                GravitySpeed.Reset();
            }
            else { this.mAnimMgr.SetAnims(AnimName.JumpEnd); }
        }

        private void OnJump(bool isJumping)
        {
            if (isJumping)
            {
                this.mAnimMgr.SetAnims(AnimName.JumpStart, AnimName.JumpLoop);
                JumpSpeed.Jump();
            }
            else { this.mMovementInfo.IsFalling = true; }
        }

        public Degree GetYaw()  // Return the yaw between -180 and 180
        {
            Degree deg = this.mNode.Orientation.Yaw;

            if (Mogre.Math.Abs(this.mNode.Orientation.w) < Mogre.Math.Abs(this.mNode.Orientation.y))    // isOnBottom
            {
                deg *= -1;
                deg -= Mogre.Math.Sign(deg.ValueAngleUnits) * new Degree(180); // +180 if on left -180 else
            }
            return deg;
        }

        public void Update(float frameTime)
        {
            /* Actualise mMovementInfo */
            if (this.mMovementInfo.IsAllowedToMove)
            {
                if (this.mCharInfo.IsPlayer) { (this as VanillaPlayer).Update(frameTime); }
                else { (this as VanillaNonPlayer).Update(frameTime); }
            }
            //MoveForward
            if (this.mIsWalking)
            {
                //LogManager.Singleton.DefaultLog.LogMessage((this.BlockPosition - this.mPathFinder.goal.Head.Data).SquaredLength.ToString());
                if(this.mPathFinder.goal.Head != null) {
                    if((this.BlockPosition - this.mPathFinder.goal.Head.Data).SquaredLength > 3) {
                        float move = frameTime * WALK_SPEED;
                        this.mNode.Translate(this.mDirection * move);

                    } else {
                        
                        this.mPathFinder.goal.RemoveFirst();
                        if(this.mPathFinder.goal.Head != null) {
                            this.mDirection = this.mPathFinder.goal.Head.Data - this.mCharacMgr.World.getRelativeFromAbsolute(this.mNode.Position);
                                
                        }
                    }
                } else {
                    this.mIsWalking = false;
                }
            }

            /* Compute translation and yaw */
            Vector3 translation = Vector3.ZERO;
            if (this.mMovementInfo.IsPushedByArcaneLevitator)
                translation.y = ArcaneLevitatorSpeed.GetSpeed();
            else if (this.mMovementInfo.IsJumping)
                translation.y = JumpSpeed.GetSpeed();
            else
                translation.y = GravitySpeed.GetSpeed();

            if (this.mMovementInfo.IsAllowedToMove)
            {
                translation += WALK_SPEED * this.mMovementInfo.MoveDirection * new Vector3(1, 0, 1);    // Ignores the y axis translation here
                this.mNode.Yaw(this.mMovementInfo.YawValue * frameTime);
            }

            this.mCollisionMgr.DrawPoints();

            translation = this.Translate(translation * frameTime);    // Apply the translation

            /* Update animations */
            if (!this.mMovementInfo.IsJumping && !this.mMovementInfo.IsFalling && !this.mMovementInfo.IsPushedByArcaneLevitator)
            {
                int zDirSign = (int)Mogre.Math.Sign(this.mMovementInfo.MoveDirection.z);
                int prevZDirSign = (int)Mogre.Math.Sign(this.mPreviousDirection.z);

                if (zDirSign != 0 && zDirSign != prevZDirSign)
                    this.mAnimMgr.SetAnims(this.mRunAnims, zDirSign);
                else if (zDirSign == 0 && prevZDirSign != 0)
                    this.mAnimMgr.DeleteAnims(this.mRunAnims);
            }
            this.mPreviousDirection = this.mMovementInfo.MoveDirection;
            if (this.mAnimMgr.CurrentAnims.Count == 0) // By default apply idle anim
            { 
                this.mAnimMgr.AddAnims(this.mIdleAnims);
                this.mPreviousDirection = Vector3.ZERO;
            }
            this.mAnimMgr.Update(frameTime);
            this.mMovementInfo.ClearInfo();
        }

        private Vector3 Translate(Vector3 relTranslation)  // relTranslation is the translation relative to the player. Return the actual relative translation
        {
            Vector3 actualTranslation = this.mCollisionMgr.ComputeCollision(relTranslation * this.mNode.LocalAxes.Transpose());

            /* Here translate has been modified to avoid collisions */
            this.mMovementInfo.IsFalling = actualTranslation.y < 0;
            this.mMovementInfo.IsJumping = actualTranslation.y > 0 && JumpSpeed.IsJumping;
            this.mNode.Translate(actualTranslation);

            return actualTranslation * this.mNode.LocalAxes;
        }

        public void moveTo(Vector3 destination)
        {

            destination = this.mCharacMgr.World.getRelativeFromAbsolute(destination);
            this.mPathFinder = new PathFinder(destination, this.mCharacMgr.World.getRelativeFromAbsolute(this.mNode.Position), this.mCharacMgr.World.getIslandAt(this.mCharInfo.IslandLoc));
            this.mIsWalking = true;

            this.mDirection = this.mPathFinder.goal.Head.Data - this.mCharacMgr.World.getRelativeFromAbsolute(this.mNode.Position);
            this.mNode.SetOrientation(0, 0, 180, 0);
        }

        /* Character */
        public String getName()                 { return this.mCharInfo.Name; }
        public String getDisplayName()          { return this.getName(); }
        public void setDisplayName(String name) { this.mCharInfo.Name = name; }
        public void teleport(Vector3 loc)       { this.FeetPosition = loc; }
        public bool save()                      { throw new NotImplementedException(); }
        public Vector3 getSpawnPoint()          { return this.mCharInfo.SpawnPoint; }
        public void setSpawnPoint(Vector3 loc)  { this.mCharInfo.SpawnPoint = loc; }

        /* Entity */
        public int getId()                        { return this.mCharInfo.Id; }
        public void remove()                      { this.mCharacMgr.RemoveCharac(this); }
        public bool isRemoved()                   { throw new NotImplementedException(); }
        public bool isSpawned()                   { throw new NotImplementedException(); }
        public void setSavable(bool savable)      { throw new NotImplementedException(); }
        public bool isSavable()                   { throw new NotImplementedException(); }
        public void setViewDistance(int distance) { throw new NotImplementedException(); }
        public int getViewDistance()              { throw new NotImplementedException(); }
        public Chunk getChunk()                   { throw new NotImplementedException(); }
        public Island getRegion()                 { throw new NotImplementedException(); }
        public void setIsPushedByArcaneLevitator(bool value)
        {
            if (value && !this.mMovementInfo.IsPushedByArcaneLevitator)
                ArcaneLevitatorSpeed.StartLevitation();
            this.mMovementInfo.IsPushedByArcaneLevitator = value;
            this.mAnimMgr.DeleteAllAnims();
        }
    }
}