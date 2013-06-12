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
        private float           mJumpHeight;

        private PathFinder      mPathFinder;
        private Queue<Vector3>  mForcedDestination;
        private Degree          mYawGoal;   // The rotation the charac has to reach to go to the next forcedPoint
        private double          mLastSquaredDist;
        private bool            mWasCollision, mHasLanded;
        private int             mIsBlocked;
        private SceneNode       temp, temp2, temp3, temp4;

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
            this.mPreviousBlockPos = -1 * Vector3.UNIT_SCALE;
            this.mForcedDestination = new Queue<Vector3>();
            this.mNode = characMgr.SceneMgr.RootSceneNode.CreateChildSceneNode("CharacterNode_" + this.mCharInfo.Id);
            this.mLastSquaredDist = -1;
            this.mHasLanded = true;

            ManualObject[] wires = new ManualObject[12];
            wires[0] = Game.World.Display.StaticRectangle.CreateLine(this.mCharacMgr.StateMgr.SceneMgr, Vector3.ZERO, Vector3.UNIT_X * Cst.CUBE_SIDE, "line_material_red");
            wires[1] = Game.World.Display.StaticRectangle.CreateLine(this.mCharacMgr.StateMgr.SceneMgr, Vector3.UNIT_X * Cst.CUBE_SIDE, new Vector3(1, 0, 1) * Cst.CUBE_SIDE, "line_material_red");
            wires[2] = Game.World.Display.StaticRectangle.CreateLine(this.mCharacMgr.StateMgr.SceneMgr, new Vector3(1, 0, 1) * Cst.CUBE_SIDE, Vector3.UNIT_Z * Cst.CUBE_SIDE, "line_material_red");
            wires[3] = Game.World.Display.StaticRectangle.CreateLine(this.mCharacMgr.StateMgr.SceneMgr, Vector3.UNIT_Z * Cst.CUBE_SIDE, Vector3.ZERO, "line_material_red");
            wires[4] = Game.World.Display.StaticRectangle.CreateLine(this.mCharacMgr.StateMgr.SceneMgr, Vector3.UNIT_Y * Cst.CUBE_SIDE, new Vector3(1, 1, 0) * Cst.CUBE_SIDE, "line_material_red");
            wires[5] = Game.World.Display.StaticRectangle.CreateLine(this.mCharacMgr.StateMgr.SceneMgr, new Vector3(1, 1, 0) * Cst.CUBE_SIDE, Vector3.UNIT_SCALE * Cst.CUBE_SIDE, "line_material_red");
            wires[6] = Game.World.Display.StaticRectangle.CreateLine(this.mCharacMgr.StateMgr.SceneMgr, Vector3.UNIT_SCALE * Cst.CUBE_SIDE, new Vector3(0, 1, 1) * Cst.CUBE_SIDE, "line_material_red");
            wires[7] = Game.World.Display.StaticRectangle.CreateLine(this.mCharacMgr.StateMgr.SceneMgr, new Vector3(0, 1, 1) * Cst.CUBE_SIDE, Vector3.UNIT_Y * Cst.CUBE_SIDE, "line_material_red");
            wires[8] = Game.World.Display.StaticRectangle.CreateLine(this.mCharacMgr.StateMgr.SceneMgr, Vector3.ZERO, Vector3.UNIT_Y * Cst.CUBE_SIDE, "line_material_red");
            wires[9] = Game.World.Display.StaticRectangle.CreateLine(this.mCharacMgr.StateMgr.SceneMgr, Vector3.UNIT_X * Cst.CUBE_SIDE, new Vector3(1, 1, 0) * Cst.CUBE_SIDE, "line_material_red");
            wires[10] = Game.World.Display.StaticRectangle.CreateLine(this.mCharacMgr.StateMgr.SceneMgr, new Vector3(1, 0, 1) * Cst.CUBE_SIDE, Vector3.UNIT_SCALE * Cst.CUBE_SIDE, "line_material_red");
            wires[11] = Game.World.Display.StaticRectangle.CreateLine(this.mCharacMgr.StateMgr.SceneMgr, Vector3.UNIT_Z * Cst.CUBE_SIDE, new Vector3(0, 1, 1) * Cst.CUBE_SIDE, "line_material_red");
            temp4 = this.mCharacMgr.StateMgr.SceneMgr.RootSceneNode.CreateChildSceneNode();
            foreach (ManualObject wire in wires)
                temp4.AttachObject(wire);
            temp4.SetVisible(false);
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
                this.mHasLanded = false;
                this.mJumpHeight = 0;
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
                //Vector3 relPos = this.BlockPosition; + MainWorld.getRelativeFromAbsolute(-this.mNode.LocalAxes.Transpose() * this.mMesh.MoveForwardDir * 60);
                //temp4.Position = MainWorld.getRelativeFromAbsolute(this.mForcedDestination.Peek()) * Cst.CUBE_SIDE;

                //if (this.mIsBlocked > 10) { this.DequeueForcedDest(); }
                
                if (this.mForcedDestination.Count > 0)
                {
                    temp4.SetVisible(true);
                    temp4.Position = (MainWorld.getRelativeFromAbsolute(this.mForcedDestination.Peek()) + Vector3.NEGATIVE_UNIT_Z) * Cst.CUBE_SIDE; 
                    
                    float x = this.FeetPosition.x - this.mForcedDestination.Peek().x;
                    float z = this.FeetPosition.z - this.mForcedDestination.Peek().z;
                    float squaredDistance = x * x + z * z;
                    if (this.mLastSquaredDist == -1 || squaredDistance > 100)
                    {
                        Degree actYaw = this.GetYaw();
                        this.mMovementInfo.YawValue = YAW_SPEED * YawFactor.GetFactor(this.mYawGoal - actYaw) / 5;

                        if (temp != null)
                            this.mCharacMgr.StateMgr.SceneMgr.DestroyManualObject((ManualObject)temp.GetAttachedObject(0));
                        if (temp2 != null)
                            this.mCharacMgr.StateMgr.SceneMgr.DestroyManualObject((ManualObject)temp2.GetAttachedObject(0));
                        if (temp3 != null)
                            this.mCharacMgr.StateMgr.SceneMgr.DestroyManualObject((ManualObject)temp3.GetAttachedObject(0));
                        Vector3 p = this.FeetPosition + 50 * Vector3.UNIT_Y;
                        Vector3 p1 = new Vector3(Mogre.Math.Sin(actYaw) * 100, 0, Mogre.Math.Cos(actYaw) * 100);
                        Vector3 p2 = new Vector3(Mogre.Math.Sin(this.mYawGoal) * 100, 0, Mogre.Math.Cos(this.mYawGoal) * 100);
                        temp = World.Display.StaticRectangle.DrawLine(this.mCharacMgr.StateMgr.SceneMgr, p, p + p1, "line_material_red");
                        temp3 = World.Display.StaticRectangle.DrawLine(this.mCharacMgr.StateMgr.SceneMgr, p, p + p2, "line_material_green");
                        temp2 = World.Display.StaticRectangle.DrawLine(this.mCharacMgr.StateMgr.SceneMgr, p, this.mForcedDestination.Peek(), "line_material_blue");

                        float diffYaw = System.Math.Abs((this.mYawGoal - actYaw).ValueAngleUnits);
                        if (diffYaw < 20)
                        {
                            float factor = 6 / diffYaw;
                            this.mMovementInfo.MoveDirection = this.mMesh.MoveForwardDir * (factor <= 1 ? factor : 1) / 5;

                            if (this.mForcedDestination.Peek().y > this.FeetPosition.y)
                            {
                                //if (this.mWasCollision ||
                                //!(this.getIsland().getBlock(relPos, false) is Game.World.Blocks.Air))
                                this.mMovementInfo.MoveDirection += Vector3.UNIT_Y;
                            }
                        }
                        this.mLastSquaredDist = squaredDistance;
                    }
                    else { this.DequeueForcedDest(); }
                }
                else
                {
                    if      (this.mCharInfo.IsPlayer)            { ((VanillaPlayer) this).Update(frameTime); }
                    else if (this.MovementInfo.IsAllowedToMove)  { ((VanillaNonPlayer)this).Update(frameTime); }
                }

                if (this.MovementInfo.IsPushedByArcaneLevitator)
                    translation.y = ArcaneLevitatorSpeed.GetSpeed();
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
            Vector3 actualTranslation = this.mCollisionMgr.ComputeCollision(relTranslation * this.mNode.LocalAxes.Transpose(), out this.mWasCollision);

            /* Here translate has been modified to avoid collisions */
            this.MovementInfo.IsFalling = actualTranslation.y < 0;
            this.MovementInfo.IsJumping = actualTranslation.y > 0 && JumpSpeed.IsJumping;

            /* For forced movement */
            if (this.mForcedDestination.Count > 0 && this.mMovementInfo.YawValue < 0.05 && actualTranslation.SquaredLength < 4)
                this.mIsBlocked++;
            else
                this.mIsBlocked = 0;
            if (!this.mHasLanded)
            {
                if (actualTranslation.y == 0) { this.mHasLanded = true; }
                else                          { this.mJumpHeight += actualTranslation.y; }
            }

            this.mNode.Translate(actualTranslation);
            Vector3 blockPos = MainWorld.getRelativeFromAbsolute(this.FeetPosition);
            if (blockPos != this.mPreviousBlockPos)
            {
                this.mCharacMgr.World.onBlockLeave(this.mPreviousBlockPos, this);
                this.mCharacMgr.World.onBlockEnter(blockPos, this);
                this.mPreviousBlockPos = blockPos;
            }
        }

        private void DequeueForcedDest()
        {
            this.mForcedDestination.Dequeue();
            this.ComputeNextYaw();
            this.mLastSquaredDist = -1;
        }

        private void ComputeNextYaw()
        {
            if (this.mForcedDestination.Count >= 1)
            {
                Vector3 ac = this.mForcedDestination.Peek() - this.FeetPosition;
                this.mYawGoal = Mogre.Math.ACos(new Vector2(ac.x, ac.z).NormalisedCopy.y);
            }
            else
                this.mYawGoal = this.GetYaw();
        }

        public void MoveTo(Vector3 destination)
        {
            this.mPathFinder = new PathFinder(MainWorld.getRelativeFromAbsolute(destination), MainWorld.getRelativeFromAbsolute(this.mNode.Position), this.mCharacMgr.World.getIsland());

            if (this.mPathFinder.Goal != null)
            {
                if (this.mPathFinder.Goal.Size > 0) { this.mForcedDestination.Clear(); }
                while (this.mPathFinder.Goal.Size > 1)
                {
                    Vector3 relPos = this.mPathFinder.Goal.Head.Data;
                    this.mForcedDestination.Enqueue(Cst.CUBE_SIDE * (relPos + new Vector3(0.5f, 0, -0.5f)));
                    this.mPathFinder.Goal.RemoveFirst();
                }
                this.mForcedDestination.Enqueue(destination);   // Enqueue the exact destination for the last one
                this.mPathFinder.Goal.RemoveFirst();
                this.ComputeNextYaw();
            }
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