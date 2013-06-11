using System;
using System.Collections.Generic;
using Mogre;

using Game.World;
using Game.Animation;
using Game.Characters.IA;

using API.Ent;
using API.Geo.Cuboid;
using API.Generic;

namespace Game.CharacSystem
{
    public abstract class VanillaCharacter : Character
    {
        private const float WALK_SPEED = 350;
        private const float YAW_SPEED = 7;    // Only for forced movement
        private const float SPRINT_FACTOR = 1.65f;

        protected CharacMgr     mCharacMgr;
        protected SceneNode     mNode;
        protected MeshAnim      mMesh;
        protected CharacterInfo mCharInfo;
        private MovementInfo    mMovementInfo;
        protected CollisionMgr  mCollisionMgr;
        private Vector3         mPreviousDirection;
        private Vector3         mPreviousBlockPos;
        private float           mTimeSinceDead;   // Wait the end of the animation 

        private PathFinder      mPathFinder;
        private Queue<Vector3>  mForcedDestination;
        private Radian          mYawGoal;   // The rotation the charac has to reach to go to the next forcedPoint
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
            get { return MainWorld.getRelativeFromAbsolute(this.FeetPosition); }
        }

        protected VanillaCharacter(CharacMgr characMgr, CharacterInfo charInfo)
        {
            this.mCharacMgr = characMgr;
            this.mCharInfo = charInfo;
            this.MovementInfo = new MovementInfo(OnFall, OnJump);
            this.mPreviousDirection = Vector3.ZERO;
            this.mTimeSinceDead = 0;
            this.mPreviousBlockPos = -1 * Vector3.UNIT_SCALE;
            this.mForcedDestination = new Queue<Vector3>();
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
            Degree deg = this.mNode.Orientation.Yaw - this.mMesh.InitialOrientation.Yaw;

            if (Mogre.Math.Abs(this.mNode.Orientation.w) < Mogre.Math.Abs(this.mNode.Orientation.y))    // isOnBottom
            {
                deg *= -1;
                deg -= Mogre.Math.Sign(deg.ValueAngleUnits) * new Degree(180); // +180 if on left -180 else
            }
            return deg;
        }

        public void Update(float frameTime)
        {
            Vector3 translation = Vector3.ZERO;

            if (this.mCharInfo.Life > 0)
            {
                if (this.mForcedDestination.Count > 0)
                {
                    float x = this.FeetPosition.x - this.mForcedDestination.Peek().x;
                    float z = this.FeetPosition.z - this.mForcedDestination.Peek().z;
                    float squaredDistance = x * x + z * z;
                    if (this.mLastSquaredDist == -1 || squaredDistance > 100)
                    {
                        Radian actYaw = this.GetYaw();
                        this.mMovementInfo.YawValue = YAW_SPEED * YawFactor.GetFactor(this.mYawGoal - actYaw);
                        //this.mMovementInfo.MoveDirection = this.mMesh.MoveForwardDir;
                        //this.mMovementInfo.MoveDirection = (this.FeetPosition - this.mForcedDestination.Peek()).NormalisedCopy;
                        //this.mNode.Translate((this.mForcedDestination.Peek() - this.FeetPosition).NormalisedCopy * WALK_SPEED * frameTime);
                        //translation = (this.FeetPosition - this.mForcedDestination.Peek()).NormalisedCopy * WALK_SPEED;
                        this.mLastSquaredDist = squaredDistance;

                        //if (squaredDistance < 4000 && this.mForcedDestination.Peek().y > this.FeetPosition.y)
                            //this.mMovementInfo.MoveDirection += Vector3.UNIT_Y;
                    }
                    else
                    {
                        this.mForcedDestination.Dequeue();
                        this.ComputeNextYaw();
                        this.mLastSquaredDist = -1;
                    }
                }
                else
                {
                    if      (this.mCharInfo.IsPlayer)            { ((VanillaPlayer) this).Update(frameTime); }
                    else if (this.MovementInfo.IsAllowedToMove) { ((VanillaNonPlayer)this).Update(frameTime); }
                }

                if (this.MovementInfo.IsPushedByArcaneLevitator)
                {
                    translation.y = ArcaneLevitatorSpeed.GetSpeed();
                    Radian actYaw = this.GetYaw();
                    Console.WriteLine(actYaw.ValueAngleUnits);
                }
                else if (this.MovementInfo.IsJumping)
                    translation.y = JumpSpeed.GetSpeed();
                else
                    translation.y = GravitySpeed.GetSpeed();

                translation += WALK_SPEED * this.MovementInfo.MoveDirection * new Vector3(1, 0, 1) * (this.MovementInfo.Sprint ? SPRINT_FACTOR : 1);    // Ignores the y axis translation here
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

            this.mNode.Translate(actualTranslation);
            Vector3 blockPos = MainWorld.getRelativeFromAbsolute(this.FeetPosition);
            if (blockPos != this.mPreviousBlockPos)
            {
                this.mCharacMgr.World.onBlockLeave(this.mPreviousBlockPos, this);
                this.mCharacMgr.World.onBlockEnter(blockPos, this);
                this.mPreviousBlockPos = blockPos;
            }
        }

        private void ComputeNextYaw()
        {
            if (this.mForcedDestination.Count >= 1)
            {
                Vector3 ab = 100 * Vector3.UNIT_Z;//this.FeetPosition - this.mNode.ConvertLocalToWorldPosition(100 * this.mMesh.MoveForwardDir);
                Vector3 ac = this.mForcedDestination.Peek() - this.FeetPosition;

                Vector2 abNormalized = new Vector2(ab.x, ab.z).NormalisedCopy;
                Vector2 acNormalized = new Vector2(ac.x, ac.z).NormalisedCopy;

                this.mYawGoal = -Mogre.Math.ACos(abNormalized.DotProduct(acNormalized)) - this.GetYaw();
                //this.mNode.SetOrientation(Mogre.Math.Cos(this.mYawGoal / 2), 0, Mogre.Math.Sin(this.mYawGoal / 2), 0);
                //this.mYawGoal -= this.GetYaw().ValueAngleUnits;
            }
        }

        public void MoveTo(Vector3 destination)
        {
            this.mForcedDestination.Enqueue(destination);
            this.ComputeNextYaw();
            /*this.mPathFinder = new PathFinder(MainWorld.getRelativeFromAbsolute(destination), MainWorld.getRelativeFromAbsolute(this.mNode.Position), this.mCharacMgr.World.getIsland());

            if (this.mPathFinder.Goal != null)
            {
                if (this.mPathFinder.Goal.Size > 0) { this.mForcedDestination.Clear(); }
                while (this.mPathFinder.Goal.Size > 0)
                {
                    this.mForcedDestination.Enqueue(this.mPathFinder.Goal.Head.Data * Cst.CUBE_SIDE);
                    this.mPathFinder.Goal.RemoveFirst();
                }
                this.ComputeNextYaw();
            }*/
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
        public void teleport(Vector3 loc)       { this.FeetPosition = loc; }
        public bool save()                      { throw new NotImplementedException(); }
        public Vector3 getSpawnPoint()          { return this.mCharInfo.SpawnPoint; }
        public void setSpawnPoint(Vector3 loc)  { this.mCharInfo.SpawnPoint = loc; }

        /* Entity */
        public int getId()                        { return this.mCharInfo.Id; }
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
    }
}