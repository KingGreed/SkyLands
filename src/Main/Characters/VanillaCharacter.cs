using System;
using System.Collections.Generic;
using Mogre;

using Game.World;
using Game.Animation;

namespace Game.CharacSystem
{
    public abstract class VanillaCharacter
    {
        private readonly Vector3 CHARAC_SIZE = new Vector3(80, 110, 80);
        private const float WALK_SPEED = 300.0f;
        private const float GRAVITY_ACCEL_T0 = -650;
        private const float GRAVITY_ACCEL_TMAX = -800;
        private const float T_MAX = 4;  // Time until the character reach its max speed fall
        private const float GRAVITY_CONST_B = T_MAX * (GRAVITY_ACCEL_TMAX - 1) / (GRAVITY_ACCEL_T0 - GRAVITY_ACCEL_TMAX);
        private const float GRAVITY_CONST_A = GRAVITY_ACCEL_T0 * GRAVITY_CONST_B;

        protected CharacMgr     mCharacMgr;
        protected SceneNode     mNode;
        protected AnimationMgr  mAnimMgr;
        protected CharacterInfo mCharInfo;
        protected MovementInfo  mMovementInfo;
        protected AnimName[]    mRunAnims;
        protected AnimName[]    mJumpAnims;
        protected AnimName[]    mIdleAnims;
        private Timer           mTimeOfFall;
        private SceneNode[]     mHitPoints;
        private bool            mIsFalling;
        private bool            mEnableGravity = true; // temp
        private Vector3         mPreviousTranslation;

        public SceneNode     Node     { get { return this.mNode; } }
        public bool          IsMoving { get { return this.mMovementInfo.IsMoving; } }
        public float         Height   { get { return this.CHARAC_SIZE.y; } }
        public CharacterInfo Info     { get { return this.mCharInfo; } }
        public Vector3 FeetPosition
        {
            get { return this.mNode.Position - new Vector3(0, this.Height / 2 + 5, 0); }
            private set { this.mNode.SetPosition(value.x, value.y + this.Height / 2 + 5, value.z); }
        }
        public bool IsFalling
        {
            get { return this.mIsFalling; }
            private set
            {
                if (this.mIsFalling != value)
                {
                    this.mIsFalling = value;
                    if (this.mIsFalling)
                    {
                        this.mAnimMgr.SetAnims(AnimName.JumpLoop);
                        this.mTimeOfFall.Reset();
                    }
                    else
                        this.mAnimMgr.SetAnims(AnimName.JumpEnd);
                }
            }
        }

        protected VanillaCharacter(CharacMgr characMgr, string meshName, CharacterInfo charInfo)
        {
            this.mCharacMgr = characMgr;
            this.mCharInfo = charInfo;
            this.mMovementInfo = new MovementInfo();
            this.mTimeOfFall = new Timer();
            this.mIsFalling = false;
            this.mPreviousTranslation = Vector3.ZERO;

            /* Create entity and node */
            SceneManager sceneMgr = characMgr.SceneMgr;
            Entity ent = sceneMgr.CreateEntity("CharacterEnt_" + this.mCharInfo.Id, meshName);
            ent.Skeleton.BlendMode = SkeletonAnimationBlendMode.ANIMBLEND_CUMULATIVE;
            Entity swordL = sceneMgr.CreateEntity("Sword.mesh");
            ent.AttachObjectToBone("Sheath.L", swordL);
            Entity swordR = sceneMgr.CreateEntity("Sword.mesh");
            ent.AttachObjectToBone("Sheath.R", swordR);

            this.mNode = sceneMgr.RootSceneNode.CreateChildSceneNode("CharacterNode_" + this.mCharInfo.Id);
            this.mNode.AttachObject(ent);
            this.mNode.Scale(CHARAC_SIZE / ent.BoundingBox.Size);
            this.FeetPosition = this.mCharInfo.SpawnPoint + new Vector3(0, 300, 0);

            /* Collisions */
            this.mHitPoints = new SceneNode[8];
            for(int i = 0; i < this.mHitPoints.Length; i++)
            {
                this.mHitPoints[i] = this.mNode.CreateChildSceneNode(this.GetTranslation(i));
                this.mHitPoints[i].InheritScale = false;
            }

            /* Create Animations */
            this.mIdleAnims = new AnimName[] { AnimName.IdleBase, AnimName.IdleTop };
            this.mRunAnims  = new AnimName[] { AnimName.RunBase, AnimName.RunTop };
            this.mJumpAnims = new AnimName[] { AnimName.JumpStart, AnimName.JumpLoop, AnimName.JumpEnd };
            this.mAnimMgr   = new AnimationMgr(ent.AllAnimationStates);
            this.mAnimMgr.SetAnims(this.mIdleAnims);
        }

        private Vector3 GetTranslation(int i)
        {
            Vector3 translation = CHARAC_SIZE / 2;
            translation.y += 5;
            if (i == 0 || i == 3 || i == 4 || i == 7) { translation.x *= -1; }
            if (i < 4)                                { translation.y *= -1; }
            if (i == 2 || i == 3 || i == 6 || i == 7) { translation.z *= -1; }

            return translation;
        }

        public void Update(float frameTime)
        {
            /* Apply gravity */
            Vector3 translation = Vector3.ZERO;

            if ((this as VanillaPlayer).Input.WasMouseButtonPressed(MOIS.MouseButtonID.MB_Middle)) { mEnableGravity = !mEnableGravity; }
            if (mEnableGravity)
            {
                float sec = (float)this.mTimeOfFall.Milliseconds / 1000;
                if (sec >= T_MAX) { translation.y += GRAVITY_ACCEL_TMAX; }
                else { translation.y += (sec + GRAVITY_CONST_A) / (sec + GRAVITY_CONST_B);}
            }

            /* Actualise mMovementInfo */
            if (this.mCharInfo.IsPlayer) { (this as VanillaPlayer).Update(frameTime); }
            else                         { (this as VanillaNonPlayer).Update(frameTime); }

            /* Apply mMovementInfo */
            if (this.mMovementInfo.IsMoving)
            {
                translation += WALK_SPEED * this.mMovementInfo.MoveDirection;
                this.mNode.Yaw(this.mMovementInfo.YawValue * frameTime);
            }
            
            this.Translate(translation * frameTime);

            /* Update animations */
            if (translation.y > 0 && this.mPreviousTranslation.y <= 0) { this.mAnimMgr.SetAnims(AnimName.JumpStart); }
            else
            {
                if (translation.z > 0 && this.mPreviousTranslation.z <= 0)  { this.mAnimMgr.SetAnims(this.mRunAnims); }
                if (translation.z < 0 && this.mPreviousTranslation.z >= 0)  { this.mAnimMgr.SetAnims(this.mRunAnims); }
                if (translation.z == 0 && this.mPreviousTranslation.z != 0) { this.mAnimMgr.DeleteAnims(this.mRunAnims); }
            }
            this.mPreviousTranslation = translation;
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
            Vector3[] hitPoints = new Vector3[this.mHitPoints.Length];
            for (int i = 0; i < hitPoints.Length; i++)
                hitPoints[i] = this.mHitPoints[i].Parent.Position + this.mHitPoints[i].Position + translation;

            return hitPoints;
        }

        private void Translate(Vector3 translation)
        {
            Vector3 actualBlock = this.mCharacMgr.World.GetBlockAbsPosFromAbs(this);
            if (actualBlock != -Vector3.UNIT_SCALE)
            {
                if (translation.y < 0 && this.mCharacMgr.World.HasCharacCollision(this.GetHitPoints(translation), this.mCharInfo.IslandLoc, CubeFace.underFace))
                    translation.y = actualBlock.y - this.FeetPosition.y;
            }

            /* Here translate has been modified to avoid collisions */
            this.IsFalling = translation.y < 0;
            this.mNode.Translate(translation, Mogre.Node.TransformSpace.TS_LOCAL);
        }
    }
}
