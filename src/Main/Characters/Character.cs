using System;
using System.Collections.Generic;
using Mogre;

using Game.World;

namespace Game.CharacSystem
{
    /* Mother class of Player and NonPlayer */
    public abstract class Character
    {
        private enum HitPoint
        {
            downFrontLeft, downFrontRight, downBackLeft, downBackRight,
            upFrontLeft, upFrontRight, upBackLeft, upBackRight,
        }

        private MainWorld mWorld;

        private readonly Vector3 SCALE_CHARAC = new Vector3(9, 10, 9);
        private const float WALK_SPEED = 300.0f;
        private const float GRAVITY_ACCEL_T0 = -750;
        private const float GRAVITY_ACCEL_TMAX = -1000;
        private const float T_MAX = 4;  // Time until the character reach its max speed fall
        private const float GRAVITY_CONST_B = T_MAX * (GRAVITY_ACCEL_TMAX - 1) / (GRAVITY_ACCEL_T0 - GRAVITY_ACCEL_TMAX);
        private const float GRAVITY_CONST_A = GRAVITY_ACCEL_T0 * GRAVITY_CONST_B;

        protected SceneNode mNode;
        protected AnimationMgr mAnimMgr;
        protected CharacterInfo mCharInfo;
        protected MovementInfo mMovementInfo;
        protected Anim[] mRunAnims;
        private Anim[] mIdleAnims;
        private Vector3 mBoundingBoxSize;
        private Timer mTimeOfFall;
        private bool mIsPlayer;

        public SceneNode Node { get { return this.mNode; } }
        public float Height   { get { return this.mBoundingBoxSize.y; } }
        public bool IsMoving  { get { return this.mMovementInfo.IsMoving; } }
        public Vector3 FeetPosition
        {
            get { return this.mNode.Position - new Vector3(0, this.Height / 2 + 5, 0); }
            set { this.mNode.SetPosition(value.x, value.y + this.Height / 2 + 5, value.z); }
        }

        protected Character(SceneManager sceneMgr, string meshName, CharacterInfo charInfo, bool isPlayer, MainWorld world)
        {
            this.mWorld = world;
            this.mIsPlayer = isPlayer;
            this.mCharInfo = charInfo;
            this.mMovementInfo = new MovementInfo();
            this.mTimeOfFall = new Timer();
            
            /* Create entity and node */
            Entity playerEnt = sceneMgr.CreateEntity("CharacterEnt_" + this.mCharInfo.Id, meshName);
            playerEnt.Skeleton.BlendMode = SkeletonAnimationBlendMode.ANIMBLEND_CUMULATIVE;
            Entity swordL = sceneMgr.CreateEntity("Sword.mesh");
            playerEnt.AttachObjectToBone("Sheath.L", swordL);
            Entity swordR = sceneMgr.CreateEntity("Sword.mesh");
            playerEnt.AttachObjectToBone("Sheath.R", swordR);

            this.mNode = sceneMgr.RootSceneNode.CreateChildSceneNode("CharacterNode_" + this.mCharInfo.Id);
            this.mNode.AttachObject(playerEnt);
            this.mNode.Scale(SCALE_CHARAC);

            this.mBoundingBoxSize = playerEnt.BoundingBox.Size * SCALE_CHARAC;
            this.FeetPosition = this.mCharInfo.SpawnPoint;

            /* Temp */
            /*Entity ent = sceneMgr.CreateEntity("cube.mesh");
            SceneNode node = mNode.CreateChildSceneNode();//sceneMgr.RootSceneNode.CreateChildSceneNode();
            node.InheritScale = false;
            node.Scale(new Vector3(0.1f, 0.1f, 0.1f));
            node.AttachObject(ent);*/

            /* Create Animations */
            this.mIdleAnims = new Anim[] { Anim.IdleBase, Anim.IdleTop };
            this.mRunAnims  = new Anim[] { Anim.RunBase, Anim.RunTop };
            this.mAnimMgr   = new AnimationMgr(playerEnt.AllAnimationStates);
            this.mAnimMgr.SetAnims(this.mIdleAnims);
        }

        public void Update(float frameTime)
        {
            /* Apply gravity */
            if (!this.HasFaceCollision(CubeFace.underFace))
            {
                if (!this.mAnimMgr.CurrentAnims.Contains(Anim.JumpLoop))
                {
                    this.mAnimMgr.SetAnims(Anim.JumpLoop);
                    this.mTimeOfFall.Reset();
                }

                float sec = this.mTimeOfFall.Milliseconds / 1000;
                float translation;
                if (sec >= T_MAX) { translation = GRAVITY_ACCEL_TMAX; }
                else { translation = (sec + GRAVITY_CONST_A) / (sec + GRAVITY_CONST_B); }

                this.mNode.Translate(new Vector3(0, translation * frameTime, 0), Mogre.Node.TransformSpace.TS_LOCAL);
            }

            // If the character has reach the ground
            if (this.mAnimMgr.CurrentAnims.Contains(Anim.JumpLoop) && this.HasFaceCollision(CubeFace.underFace))
            {
                this.mAnimMgr.SetAnims(Anim.JumpEnd);

                /* Reposition the character at the exact position */
                Vector3 translation = new Vector3(0, 1 - this.FeetPosition.y % MainWorld.CUBE_SIDE, 0);
                if (this.FeetPosition.y > 0) { translation.y += MainWorld.CUBE_SIDE; }
                this.mNode.Translate(translation * frameTime, Mogre.Node.TransformSpace.TS_LOCAL);
            }

            /* Actualise mMovementInfo */
            if (this.mIsPlayer) { ((Player)this).Update(frameTime); }
            else                { ((NonPlayer)this).Update(frameTime); }

            /* Apply mMovementInfo */
            if (this.mMovementInfo.IsMoving)
            {
                this.mNode.Translate(WALK_SPEED * this.mMovementInfo.MoveDirection * frameTime, Mogre.Node.TransformSpace.TS_LOCAL);
                this.mNode.Yaw(new Degree(this.mMovementInfo.YawValue));
            }

            if (this.mAnimMgr.CurrentAnims.Count == 0) { this.mAnimMgr.AddAnims(this.mIdleAnims); } // By default apply idle anim

            this.mAnimMgr.Update(frameTime);
            this.mMovementInfo.ClearInfo();
        }

        private bool HasFaceCollision(CubeFace face)
        {
            Vector3[] coordsToTest = this.GetFacesPoints(face);

            foreach(Vector3 coord in coordsToTest)
                if (this.mWorld.hasCollision(coord, face)) { return true; }

            return false;
        }

        private Vector3[] GetFacesPoints(CubeFace face)
        {
            Vector3[] coord = new Vector3[4];

            string faceName = Enum.GetName(typeof(CubeFace), face);
            faceName = faceName.Remove(faceName.Length - 4);    // Remove "Face"
            if (faceName == "under") { faceName = "down"; }
            else if (faceName == "upper") { faceName = "up"; }

            string[] hitPointsNames = Enum.GetNames(typeof(HitPoint));
            for (int i = 0, j = 0; j < 4 && i < 8; i++)
                if (hitPointsNames[i].Contains(faceName)) { coord[j] = this.GetHitPoint((HitPoint)i); j++; }

            return coord;
        }

        private Vector3 GetHitPoint(HitPoint point)
        {
            Vector3 coord = this.FeetPosition;

            if      (point == HitPoint.downFrontLeft)  { coord += new Vector3(this.mBoundingBoxSize.x / 2, 0, this.mBoundingBoxSize.z / 2); }
            else if (point == HitPoint.downFrontRight) { coord += new Vector3(-this.mBoundingBoxSize.x / 2, 0, this.mBoundingBoxSize.z / 2); }
            else if (point == HitPoint.downBackLeft)   { coord += new Vector3(this.mBoundingBoxSize.x / 2, 0, -this.mBoundingBoxSize.z / 2); }
            else if (point == HitPoint.downBackRight)  { coord += new Vector3(-this.mBoundingBoxSize.x / 2, 0, -this.mBoundingBoxSize.z / 2); }
            else if (point == HitPoint.upFrontLeft)    { coord += new Vector3(this.mBoundingBoxSize.x / 2, this.Height, this.mBoundingBoxSize.z / 2); }
            else if (point == HitPoint.upFrontRight)   { coord += new Vector3(-this.mBoundingBoxSize.x / 2, this.Height, this.mBoundingBoxSize.z / 2); }
            else if (point == HitPoint.upBackLeft)     { coord += new Vector3(this.mBoundingBoxSize.x / 2, this.Height, -this.mBoundingBoxSize.z / 2); }
            else  /*(point == HitPoint.upBackRight)*/  { coord += new Vector3(-this.mBoundingBoxSize.x / 2, this.Height, -this.mBoundingBoxSize.z / 2); }

            return coord;
        }
    }
}
