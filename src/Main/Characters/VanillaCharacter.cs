using System.Collections.Generic;
using System.Linq;
using Mogre;

using Game.World;
using Game.Animation;

namespace Game.CharacSystem
{
    public abstract class VanillaCharacter
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
        private GravitySpeed    mGravitySpeed;
        private JumpSpeed       mJumpSpeed;

        //MoveForward variable
        private Vector3         mStartingPoint;
        private int             mDist;
        private bool            mIsWalking;
        private AStar           mPath;

        public SceneNode     Node            { get { return this.mNode; } }
        public bool          IsAllowedToMove { get { return this.mMovementInfo.IsAllowedToMove; } set { this.mMovementInfo.IsAllowedToMove = value; } }
        public float         Height          { get { return CHARAC_SIZE.y; } }
        public CharacterInfo Info            { get { return this.mCharInfo; } }
        public Vector3       FeetPosition
        {
            get         { return this.mNode.Position - new Vector3(0, this.Height / 2 + 8, 0); }
            private set { this.mNode.SetPosition(value.x, value.y + this.Height / 2 + 8, value.z); }
        }

        protected VanillaCharacter(CharacMgr characMgr, string meshName, CharacterInfo charInfo)
        {
            this.mCharacMgr = characMgr;
            this.mCharInfo = charInfo;
            this.mMovementInfo = new MovementInfo(OnFall, OnJump);
            this.mPreviousDirection = Vector3.ZERO;
            this.mGravitySpeed = new GravitySpeed();
            this.mJumpSpeed = new JumpSpeed();

            /* Create entity and node */
            SceneManager sceneMgr = characMgr.SceneMgr;
            Entity ent = sceneMgr.CreateEntity("CharacterEnt_" + this.mCharInfo.Id, meshName);
            ent.Skeleton.BlendMode = SkeletonAnimationBlendMode.ANIMBLEND_CUMULATIVE;
            Entity swordL = sceneMgr.CreateEntity("Sword.mesh");
            ent.AttachObjectToBone("Sheath.L", swordL);
            Entity swordR = sceneMgr.CreateEntity("Sword.mesh");
            ent.AttachObjectToBone("Sheath.R", swordR);

            this.mNode = sceneMgr.RootSceneNode.CreateChildSceneNode("CharacterNode_" + this.mCharInfo.Id);
            this.mCollisionMgr = new CollisionMgr(sceneMgr, this.mCharacMgr.World, this);

            this.mStartingPoint = Vector3.ZERO;
            this.mDist          = 0;
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
                this.mGravitySpeed.Reset();
            }
            else { this.mAnimMgr.SetAnims(AnimName.JumpEnd); }
        }

        private void OnJump(bool isJumping)
        {
            if (isJumping)
            {
                this.mAnimMgr.SetAnims(AnimName.JumpStart, AnimName.JumpLoop);
                this.mJumpSpeed.Jump();
            }
            else { this.mMovementInfo.IsFalling = true; }
        }

        public Degree GetYaw()  // Return the yaw between -180 and 180
        {
            Degree deg = this.mNode.Orientation.Yaw;

            if (Math.Abs(this.mNode.Orientation.w) < Math.Abs(this.mNode.Orientation.y))    // isOnBottom
            {
                deg *= -1;
                deg -= Math.Sign(deg.ValueAngleUnits) * new Degree(180); // +180 if on left -180 else
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
                if ((this.mNode.Position - this.mPath.mPath.Peek()).Length > 50) {
                    this.mMovementInfo.MoveDirection = Vector3.UNIT_Z;
                }
                else {
                    this.mPath.mPath.Dequeue();

                    if (this.mPath.mPath.Count == 0) { this.mIsWalking = false; }
                    
                }
            }

            /* Compute translation and yaw */
            Vector3 translation = Vector3.ZERO;
            if (this.mMovementInfo.IsJumping)
                translation.y = this.mJumpSpeed.GetSpeed();
            else
                translation.y = this.mGravitySpeed.GetSpeed();

            if (this.mMovementInfo.IsAllowedToMove)
            {
                translation += WALK_SPEED * this.mMovementInfo.MoveDirection * new Vector3(1, 0, 1);    // Ignores the y axis translation here
                this.mNode.Yaw(this.mMovementInfo.YawValue * frameTime);
            }

            this.mCollisionMgr.DrawPoints();

            translation = this.Translate(translation * frameTime);    // Apply the translation

            /* Update animations */
            if (!this.mMovementInfo.IsJumping && !this.mMovementInfo.IsFalling)
            {
                int zDirSign = (int)Math.Sign(this.mMovementInfo.MoveDirection.z);
                int prevZDirSign = (int)Math.Sign(this.mPreviousDirection.z);

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
            this.mMovementInfo.IsJumping = actualTranslation.y > 0 && this.mJumpSpeed.IsJumping;
            this.mNode.Translate(actualTranslation);

            return actualTranslation * this.mNode.LocalAxes;
        }

        public void moveTo(Vector3 destination) 
        {
            this.mIsWalking     = true;
            this.mStartingPoint = this.mNode.Position;
            this.mPath          = new AStar(this.mCharacMgr.World.getIslandAt(this.mCharInfo.IslandLoc));
            this.mPath.goTo(this.mStartingPoint, destination);


            Vector3 a = this.mNode.Position;
            Vector3 b = this.mPath.mPath.Peek();
            Vector3 c = this.mNode.Position;
            c.z -= 100;

            float dA = (b - c).Length;
            float dB = (a - c).Length;
            float dC = (a - b).Length;

            //this.mNode.Orientation = new Quaternion(new Radian((dA * dA + dC * dC + dB * dB) / (2 * dA * dC)).ValueAngleUnits, Vector3.UNIT_Y);

        }

        public bool isMoveForwardFinished() { return this.mStartingPoint == this.mNode.Position; }
    }
}