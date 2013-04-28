using System;
using System.Collections.Generic;
using System.Linq;
using Mogre;

using Game.World;
using Game.Animation;
using Game.Characters.IA;
using API.Ent;
using API.Geo.Cuboid;
using Game.Shoot;

namespace Game.CharacSystem
{
    public abstract class VanillaCharacter : Character
    {
        private const float WALK_SPEED = 350.0f;

        protected CharacMgr     mCharacMgr;
        protected SceneNode     mNode;
        protected MeshAnim      mMesh;
        protected CharacterInfo mCharInfo;
        protected MovementInfo  mMovementInfo;
        protected CollisionMgr  mCollisionMgr;
        private Vector3         mPreviousDirection;
        private Vector3         mPreviousBlockPos;
        private float           mTimeSinceDead;   // Wait the end of the animation 

        //MoveForward variable
        private Vector3         mDirection;
        private PathFinder      mPathFinder;
        private bool            mIsWalking;

        public SceneNode     Node            { get { return this.mNode; } }
        public bool          IsAllowedToMove { get { return this.mMovementInfo.IsAllowedToMove; } set { this.mMovementInfo.IsAllowedToMove = value; } }
        public Vector3       Size            { get { return this.mMesh.MeshSize; } }
        public CharacterInfo Info            { get { return this.mCharInfo; } }
        public Vector3       FeetPosition
        {
            get         { return this.mNode.Position - new Vector3(0, this.Size.y / 2 + this.mMesh.FeetDiff, 0); }
            protected set { this.mNode.SetPosition(value.x, value.y + this.Size.y / 2 + this.mMesh.FeetDiff, value.z); }
        }

        public Vector3 BlockPosition
        {
            get { return MainWorld.getRelativeFromAbsolute(this.FeetPosition); }
        }

        protected VanillaCharacter(CharacMgr characMgr, string meshName, CharacterInfo charInfo)
        {
            this.mCharacMgr = characMgr;
            this.mCharInfo = charInfo;
            this.mMovementInfo = new MovementInfo(OnFall, OnJump);
            this.mPreviousDirection = Vector3.ZERO;
            this.mTimeSinceDead = 0;
            this.mPreviousBlockPos = -1 * Vector3.UNIT_SCALE;

            this.mNode = characMgr.SceneMgr.RootSceneNode.CreateChildSceneNode("CharacterNode_" + this.mCharInfo.Id);

            this.mDirection = Vector3.ZERO;
            //this.mDist          = 0;
        }

        private void OnFall(bool isFalling)
        {
            if (isFalling)
            {
                if (this.mCharInfo.IsPlayer) { (this.mMesh as Sinbad).JumpLoop(); }
                GravitySpeed.Reset();
            }
            else if (this.mCharInfo.IsPlayer)
                (this.mMesh as Sinbad).EndJump();
        }

        private void OnJump(bool isJumping)
        {
            if (isJumping)
            {
                this.mMesh.StartJump();
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
            bool isDead = this.mCharInfo.Life <= 0;

            if (!isDead)
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
                    if (this.mPathFinder.goal.Head != null)
                    {
                        if ((this.BlockPosition - this.mPathFinder.goal.Head.Data).SquaredLength > 3)
                        {
                            float move = frameTime * WALK_SPEED;
                            this.mNode.Translate(this.mDirection * move);

                        }
                        else
                        {

                            this.mPathFinder.goal.RemoveFirst();
                            if (this.mPathFinder.goal.Head != null)
                            {
                                this.mDirection = this.mPathFinder.goal.Head.Data - this.BlockPosition;
                            }
                        }
                    }
                    else
                    {
                        this.mIsWalking = false;
                    }
                }
            }
            else
            {
                this.mTimeSinceDead += frameTime;
                if (this.mTimeSinceDead >= 2) { this.mCharacMgr.RemoveCharac(this); }
            }

            /* Compute translation and yaw */
            Vector3 translation = Vector3.ZERO;
            if (this.mMovementInfo.IsPushedByArcaneLevitator)
                translation.y = ArcaneLevitatorSpeed.GetSpeed();
            else if (this.mMovementInfo.IsJumping)
                translation.y = JumpSpeed.GetSpeed();
            else
                translation.y = GravitySpeed.GetSpeed();

            if (this.mMovementInfo.IsAllowedToMove && !isDead)
            {
                translation += WALK_SPEED * this.mMovementInfo.MoveDirection * new Vector3(1, 0, 1);    // Ignores the y axis translation here
                this.mNode.Yaw(this.mMovementInfo.YawValue * frameTime);
            }

            this.mCollisionMgr.DrawPoints();

            if (!isDead)
            {
                translation = this.Translate(translation * frameTime);    // Apply the translation

                if (!this.mMovementInfo.IsJumping && !this.mMovementInfo.IsFalling && !this.mMovementInfo.IsPushedByArcaneLevitator)
                {
                    int zDirSign = (int)Mogre.Math.Sign(this.mMovementInfo.MoveDirection.z);
                    int prevZDirSign = (int)Mogre.Math.Sign(this.mPreviousDirection.z);

                    if (zDirSign != 0 && zDirSign != prevZDirSign)
                        this.mMesh.Walk(true, zDirSign);
                    else if (zDirSign == 0 && prevZDirSign != 0)
                        this.mMesh.Walk(false);
                }
                this.mPreviousDirection = this.mMovementInfo.MoveDirection;
                if (this.mMesh.AnimMgr.CurrentAnims.Count == 0) // By default apply idle anim
                {
                    this.mMesh.Idle();
                    this.mPreviousDirection = Vector3.ZERO;
                }
            }
            this.mMesh.Update(frameTime);
            this.mMovementInfo.ClearInfo();
        }

        private Vector3 Translate(Vector3 relTranslation)  // relTranslation is the translation relative to the player. Return the actual relative translation
        {
            Vector3 actualTranslation = this.mCollisionMgr.ComputeCollision(relTranslation * this.mNode.LocalAxes.Transpose());

            /* Here translate has been modified to avoid collisions */
            this.mMovementInfo.IsFalling = actualTranslation.y < 0;
            this.mMovementInfo.IsJumping = actualTranslation.y > 0 && JumpSpeed.IsJumping;

            this.mNode.Translate(actualTranslation);
            Vector3 blockPos = MainWorld.getRelativeFromAbsolute(this.FeetPosition);
            if (blockPos != this.mPreviousBlockPos)
            {
                this.mCharacMgr.World.onBlockLeave(this.mPreviousBlockPos, this);
                this.mCharacMgr.World.onBlockEnter(blockPos, this);
                this.mPreviousBlockPos = blockPos;
            }

            return actualTranslation * this.mNode.LocalAxes;
        }

        public void moveTo(Vector3 destination)
        {

            destination = MainWorld.getRelativeFromAbsolute(destination);
            this.mPathFinder = new PathFinder(destination, MainWorld.getRelativeFromAbsolute(this.mNode.Position), this.mCharacMgr.World.getIsland());

            if(this.mPathFinder.goal != null) {
                this.mIsWalking = true;

                this.mDirection = this.mPathFinder.goal.Head.Data - MainWorld.getRelativeFromAbsolute(this.mNode.Position);
                this.mNode.SetOrientation(0, 0, 180, 0);
            }
        }

        public void Hit(Bullet b)
        {
            this.mCharInfo.Life -= b.Damage;
            if (this.mCharInfo.IsPlayer && (this as VanillaPlayer).HUD != null) { (this as VanillaPlayer).HUD.UpdateLife(this.mCharInfo.Life, VanillaPlayer.DEFAULT_PLAYER_LIFE); }

            if (this.mCharInfo.Life <= 0)
            {
                this.mCharacMgr.StateMgr.WriteOnConsole(this.mCharInfo.Name + " died heroically for his nation.");

                if (!this.mCharInfo.IsPlayer) { (this.mMesh as Robot).Die(); }
                else { this.mCharacMgr.RemoveCharac(this); }
            }
        }

        public void Dispose()   // Must be called by the CharacMgr only. Use mCharacMgr.RemoveCharac(this) instead
        {
            this.mNode.RemoveAndDestroyAllChildren();
            this.mCharacMgr.SceneMgr.DestroySceneNode(this.mNode);
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

        public Island  getIsland()   { return this.mCharacMgr.World.getIsland(); }
        public Vector3 getPosition() { return this.BlockPosition; }

        public void setIsPushedByArcaneLevitator(bool value)
        {
            if (value && !this.mMovementInfo.IsPushedByArcaneLevitator)
                ArcaneLevitatorSpeed.StartLevitation();
            this.mMovementInfo.IsPushedByArcaneLevitator = value;
            this.mMesh.AnimMgr.DeleteAllAnims();
        }
    }
}