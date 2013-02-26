using System;
using System.Collections.Generic;
using Mogre;

using Game.World;
using Game.Animation;
using Game.IGConsole;

namespace Game.CharacSystem
{
    public abstract class VanillaCharacter
    {
        private readonly Vector3 CHARAC_SIZE = new Vector3(80, 110, 80);
        private const float WALK_SPEED = 350.0f;
        private const float COL_SIDE_MARGE = 0.7f;

        protected CharacMgr     mCharacMgr;
        protected SceneNode     mNode;
        protected AnimationMgr  mAnimMgr;
        protected CharacterInfo mCharInfo;
        protected MovementInfo  mMovementInfo;
        protected AnimName[]    mRunAnims;
        protected AnimName[]    mJumpAnims;
        protected AnimName[]    mIdleAnims;
        private SceneNode[]     mPoints;    // mPoints is used to show the cube of collision
        private Degree[]        mHitDegrees;
        private float           mHitRadius;
        private Vector3         mPreviousTranslation;
        private GravitySpeed    mGravitySpeed;
        private JumpSpeed       mJumpSpeed;

        //MoveForward variable
        private Vector3         mStartingPoint;
        private int             mDist;
        private bool            mIsWalking;
        private AStar           mPath;

        public SceneNode     Node            { get { return this.mNode; } }
        public bool          IsAllowedToMove { get { return this.mMovementInfo.IsAllowedToMoved; } set { this.mMovementInfo.IsAllowedToMoved = value; } }
        public float         Height          { get { return this.CHARAC_SIZE.y; } }
        public CharacterInfo Info            { get { return this.mCharInfo; } }
        public SceneNode[]   Points          { get { return this.mPoints; } }
        public Vector3       FeetPosition
        {
            get         { return this.mNode.Position - new Vector3(0, this.Height / 2 + 5, 0); }
            private set { this.mNode.SetPosition(value.x, value.y + this.Height / 2 + 5, value.z); }
        }

        protected VanillaCharacter(CharacMgr characMgr, string meshName, CharacterInfo charInfo)
        {
            this.mCharacMgr = characMgr;
            this.mCharInfo = charInfo;
            this.mMovementInfo = new MovementInfo(OnFall, OnJump);
            this.mPreviousTranslation = Vector3.ZERO;
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

            this.mStartingPoint = Vector3.ZERO;
            this.mDist          = 0;
            this.mIsWalking     = false;

            this.mNode.AttachObject(ent);
            this.FeetPosition = this.mCharInfo.SpawnPoint;// +new Vector3(0, 300, 0);

            /* Collisions */
            //this.mHitPoints = new Vector3[8];
            this.mHitRadius = CHARAC_SIZE.x / 2 * COL_SIDE_MARGE;
            this.mHitDegrees = new Degree[8];
            this.mPoints = new SceneNode[this.mHitDegrees.Length];
            for (int i = 0; i < this.mHitDegrees.Length; i++)
            {
                this.mHitDegrees[i] = this.GetDegree(i);

                Entity cube = sceneMgr.CreateEntity("cube.mesh");
                this.mPoints[i] = sceneMgr.RootSceneNode.CreateChildSceneNode();
                this.mPoints[i].AttachObject(cube);
                this.mPoints[i].Scale(0.02f * Vector3.UNIT_SCALE);
                this.mPoints[i].SetVisible(false);
            }

            /* Create Animations */
            this.mIdleAnims = new AnimName[] { AnimName.IdleBase, AnimName.IdleTop };
            this.mRunAnims  = new AnimName[] { AnimName.RunBase, AnimName.RunTop };
            this.mJumpAnims = new AnimName[] { AnimName.JumpStart, AnimName.JumpLoop, AnimName.JumpEnd };
            this.mAnimMgr   = new AnimationMgr(ent.AllAnimationStates);
            this.mAnimMgr.SetAnims(this.mIdleAnims);

            this.mNode.Scale(CHARAC_SIZE / ent.BoundingBox.Size);
        }

        private Degree GetDegree(int i)
        {
            Degree deg;

            if      (i % 4 == 0)         { deg = 135; }
            else if ((i + 1) % 4 == 0)   { deg = 45; }
            else if ((i + 2) % 4 == 0)   { deg = -45; }
            else  /*((i + 3) % 4 == 0)*/ { deg = -135; }

            return deg;
            
            /*Vector3 translation = CHARAC_SIZE / 2 * COL_SIDE_MARGE;
            translation.y = this.mNode.Position.y - this.FeetPosition.y;
            if (i == 0 || i == 3 || i == 4 || i == 7) { translation.x *= -1; }
            if (i < 4) { translation.y *= -1; }
            else       { translation.y *= COL_HEIGHT_MARGE; }
            if (i == 2 || i == 3 || i == 6 || i == 7) { translation.z *= -1; }

            return translation;*/
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
        }

        public void Update(float frameTime)
        {
            /* Actualise mMovementInfo */
            if (this.mMovementInfo.IsAllowedToMoved)
            {
                if (this.mCharInfo.IsPlayer) { (this as VanillaPlayer).Update(frameTime); }
                else { (this as VanillaNonPlayer).Update(frameTime); }
            }
            //MoveForward
            if (this.mIsWalking)
            {
                if (this.mNode.Position != this.mPath.mPath.Peek()) {
                    this.mNode.Translate((this.mPath.mPath.Peek() - this.mNode.Position) * WALK_SPEED * frameTime);
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

            if (this.mMovementInfo.IsAllowedToMoved)
            {
                translation += WALK_SPEED * this.mMovementInfo.MoveDirection * new Vector3(1, 0, 1);    // Ignores the y axis translation here
                this.mNode.Yaw(this.mMovementInfo.YawValue * frameTime);
            }

            /* Temp - Show Points */
            if (this.mCharInfo.IsPlayer && (this as VanillaPlayer).Input.WasKeyPressed(MOIS.KeyCode.KC_F4))
                foreach (SceneNode node in this.mPoints) { node.FlipVisibility(); }
            Vector3[] points = this.GetHitPoints();
            for (int i = 0; i < this.mPoints.Length; i++)
                this.mPoints[i].Position = points[i];


            /* Update animations */
            if (!this.mMovementInfo.IsJumping && !this.mMovementInfo.IsFalling)
            {
                if ((translation.z > 0 && this.mPreviousTranslation.z <= 0) || 
                    (translation.z < 0 && this.mPreviousTranslation.z >= 0))  { this.mAnimMgr.SetAnims(this.mRunAnims); }
                if (translation.z == 0 && this.mPreviousTranslation.z != 0)   { this.mAnimMgr.DeleteAnims(this.mRunAnims); }
            }

            this.mPreviousTranslation = this.Translate(translation * frameTime);    // Apply the translation
            if (this.mAnimMgr.CurrentAnims.Count == 0) // By default apply idle anim
            { 
                this.mAnimMgr.AddAnims(this.mIdleAnims);
                this.mPreviousTranslation = Vector3.ZERO;
            }
            this.mAnimMgr.Update(frameTime);
            this.mMovementInfo.ClearInfo();
        }

        private Vector3[] GetHitPoints() { return this.GetHitPoints(Vector3.ZERO); }
        private Vector3[] GetHitPoints(Vector3 translation)
        {
            Vector3[] hitPoints = new Vector3[this.mHitDegrees.Length];

            for (int i = 0; i < hitPoints.Length; i++)
            {
                hitPoints[i] = this.FeetPosition;

                bool isOnLeft = this.mNode.Orientation.Yaw >= 0;
                bool isOnTop = Mogre.Math.Abs(this.mNode.Orientation.w) >= Mogre.Math.Abs(this.mNode.Orientation.y);

                Degree deg = this.mNode.Orientation.Yaw;
                if (!isOnTop)
                {
                    if (isOnLeft)
                        deg = new Degree(180) - this.mNode.Orientation.Yaw;
                    else
                        deg = -new Degree(180) - this.mNode.Orientation.Yaw;
                }
                deg *= -1;
                deg += this.mHitDegrees[i];

                hitPoints[i].x += this.mHitRadius * Mogre.Math.Cos(deg);
                hitPoints[i].z += this.mHitRadius * Mogre.Math.Sin(deg);

                if (i >= 4) { hitPoints[i].y += this.Height; }
            }

            return hitPoints;
        }

        private Vector3 Translate(Vector3 translation)  // Return the actual translation
        {
            Vector3 colissionBlockRelPos;
            
            if (translation.x < 0 && this.mCharacMgr.World.HasCharacCollision(this.GetHitPoints(translation), this.mCharInfo.IslandLoc, CubeFace.leftFace, out colissionBlockRelPos))
                    translation.x = 0;
                if (translation.x > 0 && this.mCharacMgr.World.HasCharacCollision(this.GetHitPoints(translation), this.mCharInfo.IslandLoc, CubeFace.rightFace, out colissionBlockRelPos))
                    translation.x = 0;

                this.mMovementInfo.IsFalling = !this.mCharacMgr.World.HasCharacCollision(this.GetHitPoints(translation), this.mCharInfo.IslandLoc, CubeFace.underFace, out colissionBlockRelPos);
                if (translation.y < 0 && !this.mMovementInfo.IsFalling)
                    translation.y = (colissionBlockRelPos.y + 1) * MainWorld.CUBE_SIDE - this.FeetPosition.y;
                if (translation.y > 0 && this.mCharacMgr.World.HasCharacCollision(this.GetHitPoints(translation), this.mCharInfo.IslandLoc, CubeFace.upperFace, out colissionBlockRelPos))
                {
                    translation.y = -(colissionBlockRelPos.y * MainWorld.CUBE_SIDE - this.FeetPosition.y - this.Height);
                    if (translation.y > 0)
                        translation.y = 0;
                }

                if (translation.z < 0 && this.mCharacMgr.World.HasCharacCollision(this.GetHitPoints(translation), this.mCharInfo.IslandLoc, CubeFace.backFace, out colissionBlockRelPos))
                    translation.z = 0;
                if (translation.z > 0 && this.mCharacMgr.World.HasCharacCollision(this.GetHitPoints(translation), this.mCharInfo.IslandLoc, CubeFace.frontFace, out colissionBlockRelPos))
                    translation.z = 0;

            /* Here translate has been modified to avoid collisions */
            this.mMovementInfo.IsJumping = translation.y > 0 && this.mJumpSpeed.IsJumping;
            this.mNode.Translate(translation, Mogre.Node.TransformSpace.TS_LOCAL);

            return translation;
        }

        public void moveTo(Vector3 destination) 
        {
            this.mIsWalking     = true;
            this.mStartingPoint = this.mNode.Position;
            this.mPath          = new AStar(this.mCharacMgr.World.getIslandAt(this.mCharInfo.IslandLoc));
            this.mPath.goTo(this.mStartingPoint, destination);

            Vector3 src = this.mNode.Orientation * Vector3.UNIT_X;
            Quaternion quat = src.GetRotationTo(this.mPath.mPath.Peek() - this.mNode.Position);
            mNode.Rotate(quat);
        }


        public bool isMoveForwardFinished() { return this.mStartingPoint == this.mNode.Position; }
    }
}