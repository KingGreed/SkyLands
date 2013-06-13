using System;
using System.Collections.Generic;
using Mogre;

using API.Ent;
using API.Geo.Cuboid;

using Game.World;
using Game.Animation;
using Game.CharacSystem.AI;

namespace Game.CharacSystem
{
    public abstract class VanillaCharacter : Character
    {
        private const float WALK_SPEED = 350;
        private const float YAW_SPEED = 7;    // Only for forced movement
        private const float SQUARED_DIST_PRECISION = 200;
        private const float SPRINT_FACTOR = 1.65f;

        protected CharacMgr     mCharacMgr;
        protected SceneNode     mNode;
        protected MeshAnim      mMesh;
        protected CharacterInfo mCharInfo;
        private MovementInfo    mMovementInfo;
        protected CollisionMgr  mCollisionMgr;
        private Vector3         mPreviousDirection;
        private float           mTimeSinceDead;   // Wait the end of the animation 

        private PathFinder      mPathFinder;
        private Stack<Vector3>  mForcedDestination;
        private Degree          mYawGoal;   // The rotation the charac has to reach to go to the next forcedPoint
        private double          mLastSquaredDist;

        public SceneNode     Node            { get { return this.mNode; } }
        public MovementInfo  MovementInfo    { get { return this.mMovementInfo; } protected set { this.mMovementInfo = value; } }
        public Vector3       Size            { get { return this.mMesh.MeshSize; } }
        public CharacterInfo Info            { get { return this.mCharInfo; } }
        public CollisionMgr  CollisionMgr    { get { return this.mCollisionMgr; } }
        public bool          WaitForRemove   { get; private set; }
        public Vector3       FeetPosition
        {
            get           { return this.mNode.Position - new Vector3(0, this.Size.y / 2 + this.mMesh.FeetDiff, 0); }
            protected set { this.mNode.SetPosition(value.x, value.y + this.Size.y / 2 + this.mMesh.FeetDiff, value.z); }
        }

        public Vector3 BlockPosition
        {
            get { return MainWorld.getRelativeFromAbsolute(this.FeetPosition) + Vector3.NEGATIVE_UNIT_Z; }
        }

        protected VanillaCharacter(CharacMgr characMgr, CharacterInfo charInfo)
        {
            this.mCharacMgr = characMgr;
            this.mCharInfo = charInfo;
            this.MovementInfo = new MovementInfo(this.OnFall, this.OnJump);
            this.mPreviousDirection = Vector3.ZERO;
            this.mTimeSinceDead = 0;
            this.mNode = characMgr.SceneMgr.RootSceneNode.CreateChildSceneNode("CharacterNode_" + this.mCharInfo.Id);
            this.mLastSquaredDist = -1;
        }

        private void OnFall(bool isFalling)
        {
            if (isFalling)
            {
                this.mMesh.JumpLoop();
                GravitySpeed.Reset();
            }
            else if (this.mCharInfo.IsPlayer)
                ((Sinbad) this.mMesh).EndJump();
        }

        private void OnJump(bool isJumping)
        {
            if (isJumping)
            {
                this.mMesh.StartJump();
                JumpSpeed.Jump();
            }
            else { this.MovementInfo.IsFalling = true; }
        }

        public Degree GetYaw()  // Return the yaw between -180 and 180
        {
            Quaternion q = this.mNode.Orientation;
            return (Mogre.Math.Abs(q.w) < Mogre.Math.Abs(q.y) ? (new Degree(180) - q.Yaw) : (Degree)q.Yaw) - this.mMesh.InitialOrientation.Yaw;
        }

        public void Update(float frameTime)
        {
            Vector3 translation = Vector3.ZERO;

            if (this.mCharInfo.Life > 0)
            {
                if (this.mPathFinder != null)
                {
                    this.mForcedDestination = this.mPathFinder.ContinuePathFinding();
                    if (this.mForcedDestination != null)
                    {
                        this.mPathFinder = null;
                        this.ComputeNextYaw();
                        this.mLastSquaredDist = -1;
                    }
                }

                if (this.mForcedDestination != null)
                {
                    Vector3 diff = this.mForcedDestination.Peek() - this.FeetPosition; 
                    float squaredDistance = diff.x * diff.x + diff.z * diff.z;
                    if (this.mLastSquaredDist == -1 || squaredDistance > SQUARED_DIST_PRECISION)
                    {
                        Degree diffYaw = this.mYawGoal - this.GetYaw();
                        this.mMovementInfo.YawValue = YAW_SPEED * YawFactor.GetFactor(diffYaw);

                        float absDiffYaw = System.Math.Abs((((int)diffYaw.ValueAngleUnits) + 360) % 360);
                        if (absDiffYaw < 20)
                        {
                            float factor = 6 / absDiffYaw;
                            this.mMovementInfo.MoveDirection = this.mMesh.MoveForwardDir * (factor <= 1 ? factor : 1);

                            if (this.mForcedDestination.Peek().y > this.FeetPosition.y)
                                this.mMovementInfo.MoveDirection += Vector3.UNIT_Y;
                        }
                        this.mLastSquaredDist = squaredDistance;
                    }
                    else { this.PopForcedDest(); }
                }
                else
                {
                    if      (this.mCharInfo.IsPlayer)           { ((VanillaPlayer) this).Update(frameTime); }
                    else if (this.MovementInfo.IsAllowedToMove) { ((VanillaNonPlayer) this).Update(frameTime); }
                }

                if (this.MovementInfo.IsPushedByArcaneLevitator)
                    translation.y = ArcaneLevitatorSpeed.GetSpeed();
                else if (this.MovementInfo.IsJumping)
                    translation.y = JumpSpeed.GetSpeed();
                else
                    translation.y = GravitySpeed.GetSpeed();

                translation += WALK_SPEED * this.MovementInfo.MoveDirection * new Vector3(1, 0, 1) * (this.MovementInfo.Sprint ? SPRINT_FACTOR : 1); // Ignores the y axis translation here
                this.mNode.Yaw(this.MovementInfo.YawValue * frameTime);

                this.Translate(translation * frameTime);    // Apply the translation

                if (!this.MovementInfo.IsJumping && !this.MovementInfo.IsFalling && !this.MovementInfo.IsPushedByArcaneLevitator)
                {
                    int zDirSign = this.mMesh.Zdir(this.MovementInfo.MoveDirection);
                    int prevZDirSign = this.mMesh.Zdir(this.mPreviousDirection);

                    if (zDirSign != 0 && zDirSign != prevZDirSign)
                        this.mMesh.Walk(true, zDirSign);
                    else if (zDirSign == 0 && prevZDirSign != 0)
                        this.mMesh.Walk(false);
                }
                this.mPreviousDirection = this.MovementInfo.MoveDirection;
                if (this.mMesh.AnimMgr.CurrentAnims.Count == 0) // By default apply idle anim
                {
                    this.mMesh.Idle();
                    this.mPreviousDirection = Vector3.ZERO;
                }
            }
            else
            {
                this.mTimeSinceDead += frameTime;
                if (this.mTimeSinceDead >= 2) { this.WaitForRemove = true; }
            }

            this.mMesh.Update(frameTime);
            this.MovementInfo.ClearInfo();
        }

        private void Translate(Vector3 relTranslation)  // relTranslation is the translation relative to the player. Return the actual relative translation
        {
            Vector3 actualTranslation = this.mCollisionMgr.ComputeCollision(relTranslation * this.mNode.LocalAxes.Transpose());

            /* Here translate has been modified to avoid collisions */
            this.MovementInfo.IsFalling = actualTranslation.y < 0;
            this.MovementInfo.IsJumping = actualTranslation.y > 0 && JumpSpeed.IsJumping;

            Vector3 prevBlockPos = this.BlockPosition;
            this.mNode.Translate(actualTranslation);
            Vector3 actBlockPos = this.BlockPosition;

            if(this.mForcedDestination != null && this.mCollisionMgr.HasHorizontalCollisionEnded)
                this.ComputeNextYaw();

            if (prevBlockPos != actBlockPos)
            {
                this.mCharacMgr.World.onBlockLeave(prevBlockPos, this);
                this.mCharacMgr.World.onBlockEnter(actBlockPos, this);
            }
        }

        private void PopForcedDest()
        {
            this.mForcedDestination.Pop();
            if (this.mForcedDestination.Count == 0) 
                this.mForcedDestination = null;
            else
                this.ComputeNextYaw();
            this.mLastSquaredDist = -1;
        }

        private void ComputeNextYaw()
        {
            if (this.mForcedDestination.Count >= 1)
            {
                Vector3 diff = this.mForcedDestination.Peek() - this.FeetPosition;
                this.mYawGoal = Mogre.Math.ACos(new Vector2(diff.x, diff.z).NormalisedCopy.y) * System.Math.Sign(diff.x);
            }
            else
                this.mYawGoal = this.GetYaw();
        }

        public void MoveTo(Vector3 destination)
        {
            Vector3 diff = destination - this.FeetPosition;
            if (diff.x * diff.x + diff.z * diff.z < SQUARED_DIST_PRECISION) { return; }
            
            this.mPathFinder = new PathFinder(destination, this.BlockPosition, this.mCharacMgr.World.getIsland());
        }

        public void Hit(float damage)
        {
            this.mCharInfo.Life -= damage;

            if (this.mCharInfo.IsPlayer && this.mCharacMgr.StateMgr.Controller.GamePadState.IsConnected)
            {
                float force = damage / VanillaPlayer.DEFAULT_PLAYER_LIFE;
                this.mCharacMgr.StateMgr.Controller.Vibrate(force, force);
            }

            if (this.mCharInfo.Life <= 0)
            {
                this.MovementInfo.IsJumping = false;
                this.MovementInfo.IsPushedByArcaneLevitator = false;
                
                if (!this.mCharInfo.IsPlayer) { ((Robot) this.mMesh).Die(); }   // Don't remove the charac right now to play the animation
                else { this.WaitForRemove = true; }
            }
        }

        public void Dispose()   // Must be called by the CharacMgr only.
        {
            this.mNode.RemoveAndDestroyAllChildren();
            this.mCharacMgr.SceneMgr.DestroySceneNode(this.mNode);
        }

        /* Character */
        public String getName()                 { return this.mCharInfo.Name; }
        public String getDisplayName()          { return this.getName(); }
        public void setDisplayName(String name) { this.mCharInfo.Name = name; }
        public bool save()                      { throw new NotImplementedException(); }
        public Vector3 getSpawnPoint()          { return this.mCharInfo.SpawnPoint; }
        public void setSpawnPoint(Vector3 loc)  { this.mCharInfo.SpawnPoint = loc; }
        public void teleport(Vector3 loc)
        {
            this.mMovementInfo.ClearInfo();
            this.mMovementInfo.IsFalling = false;
            this.mMovementInfo.IsJumping = false;
            this.mMovementInfo.IsPushedByArcaneLevitator = false;
            this.mPreviousDirection = Vector3.ZERO;
            this.mForcedDestination.Clear();
            
            Vector3 prevBlockPos = this.BlockPosition;
            this.FeetPosition = loc;
            Vector3 actBlockPos = this.BlockPosition;

            if (prevBlockPos != actBlockPos)
            {
                this.mCharacMgr.World.onBlockLeave(prevBlockPos, this);
                this.mCharacMgr.World.onBlockEnter(actBlockPos, this);
            }
        }

        /* Entity */
        public int getId()                        { return this.mCharInfo.Id; }
        public CharacterInfo getCharacInfo()      { return this.mCharInfo; }
        public void remove()                      { this.WaitForRemove = true; }
        public bool isRemoved()                   { throw new NotImplementedException(); }
        public bool isSpawned()                   { throw new NotImplementedException(); }
        public void setSavable(bool savable)      { throw new NotImplementedException(); }
        public bool isSavable()                   { throw new NotImplementedException(); }
        public void setViewDistance(int distance) { throw new NotImplementedException(); }
        public int getViewDistance()              { throw new NotImplementedException(); }
        public Chunk getChunk()                   { throw new NotImplementedException(); }

        public Island getIsland() { return this.mCharacMgr.World.getIsland(); }
        public Vector3 getPosition() { return this.BlockPosition; }

        public void setIsPushedByArcaneLevitator(bool isPushedWanted)
        {
            if (isPushedWanted && !this.MovementInfo.IsPushedByArcaneLevitator)
                ArcaneLevitatorSpeed.StartLevitation();

            if (!isPushedWanted)
                GravitySpeed.Reset();

            this.MovementInfo.IsPushedByArcaneLevitator = isPushedWanted;
            this.mMesh.AnimMgr.DeleteAllAnims();
        }

        public virtual void updateTargets()
        {
            foreach (Faction faction in Enum.GetValues(typeof(Faction)))
            {
                if (faction != this.mCharInfo.Faction)
                {
                    List<VanillaCharacter> ennemies = this.mCharacMgr.GetFactionCharacters(faction);
                }
            }

        }
    }
}