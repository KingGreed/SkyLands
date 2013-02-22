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
        private const float COL_HEIGHT_MARGE = 0.86f;
        private const float COL_SIDE_MARGE = 0.47f;

        protected CharacMgr     mCharacMgr;
        protected SceneNode     mNode;
        protected AnimationMgr  mAnimMgr;
        protected CharacterInfo mCharInfo;
        protected MovementInfo  mMovementInfo;
        protected AnimName[]    mRunAnims;
        protected AnimName[]    mJumpAnims;
        protected AnimName[]    mIdleAnims;
        private SceneNode[]     mHitPoints, mPoints;    // mPoints is used to show the cube of collision
        private Vector3         mPreviousTranslation;
        private GravitySpeed    mGravitySpeed;
        private JumpSpeed       mJumpSpeed;

        public SceneNode     Node     { get { return this.mNode; } }
        public bool          IsMoving { get { return this.mMovementInfo.IsMoving; } }
        public float         Height   { get { return this.CHARAC_SIZE.y; } }
        public CharacterInfo Info     { get { return this.mCharInfo; } }
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

            //Entity cube2 = sceneMgr.CreateEntity("cube.mesh");
            //this.mNode.AttachObject(cube2);

            this.mNode.AttachObject(ent);
            this.FeetPosition = this.mCharInfo.SpawnPoint + new Vector3(0, 300, 0);

            /* Collisions */
            Console.WriteLine(this.mNode.Position);
            this.mHitPoints = new SceneNode[8];
            this.mPoints = new SceneNode[this.mHitPoints.Length];
            for(int i = 0; i < this.mHitPoints.Length; i++)
            {
                this.mHitPoints[i] = this.mNode.CreateChildSceneNode(this.GetTranslation(i));
                this.mHitPoints[i].InheritScale = false;

                Entity cube = sceneMgr.CreateEntity("cube.mesh");
                this.mPoints[i] = sceneMgr.RootSceneNode.CreateChildSceneNode();
                this.mPoints[i].AttachObject(cube);
                this.mPoints[i].Scale(0.02f * Vector3.UNIT_SCALE);
                this.mPoints[i].SetVisible(false);
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

        private Vector3 GetTranslation(int i)
        {
            Vector3 translation = CHARAC_SIZE / 2 * COL_SIDE_MARGE;
            translation.y = this.mNode.Position.y - this.FeetPosition.y;
            if (i == 0 || i == 3 || i == 4 || i == 7) { translation.x *= -1; }
            if (i < 4) { translation.y *= -1; }
            else       { translation.y *= COL_HEIGHT_MARGE; }
            if (i == 2 || i == 3 || i == 6 || i == 7) { translation.z *= -1; }

            return translation;
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
            if (this.mCharInfo.IsPlayer) { (this as VanillaPlayer).Update(frameTime); }
            else                         { (this as VanillaNonPlayer).Update(frameTime); }

            /* Apply mMovementInfo */
            Vector3 translation = Vector3.ZERO;
            if (this.mMovementInfo.IsJumping)
                translation.y = this.mJumpSpeed.GetSpeed();
            else
                translation.y = this.mGravitySpeed.GetSpeed();

            if (this.mMovementInfo.IsMoving)
            {
                translation += WALK_SPEED * this.mMovementInfo.MoveDirection * new Vector3(1, 0, 1);    // Ignores the y axis translation here
                this.mNode.Yaw(this.mMovementInfo.YawValue * frameTime);
            }

            if ((this as VanillaPlayer).Input.WasMouseButtonPressed(MOIS.MouseButtonID.MB_Left))
                this.mCharacMgr.World.getIslandAt(this.mCharInfo.IslandLoc).addFaceToScene(API.Generic.BlockFace.upperFace, this.FeetPosition / MainWorld.CUBE_SIDE, "cube/sand");

            this.Translate(translation * frameTime);

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
                /*if (translation.x < 0 && this.mCharacMgr.World.HasCharacCollision(this.GetHitPoints(translation), this.mCharInfo.IslandLoc, CubeFace.rightFace))
                    translation.x = actualBlock.x - this.FeetPosition.x;
                if (translation.x > 0 && this.mCharacMgr.World.HasCharacCollision(this.GetHitPoints(translation), this.mCharInfo.IslandLoc, CubeFace.leftFace))
                    translation.x = this.FeetPosition.x - actualBlock.x;*/

                this.mMovementInfo.IsFalling = !this.mCharacMgr.World.HasCharacCollision(this.GetHitPoints(translation), this.mCharInfo.IslandLoc, CubeFace.underFace);
                if (translation.y < 0 && !this.mMovementInfo.IsFalling)  { translation.y = actualBlock.y - this.FeetPosition.y; }
                if (translation.y > 0 && this.mCharacMgr.World.HasCharacCollision(this.GetHitPoints(translation), this.mCharInfo.IslandLoc, CubeFace.upperFace))
                    translation.y = this.FeetPosition.y - actualBlock.y;

                /*if (translation.z < 0 && this.mCharacMgr.World.HasCharacCollision(this.GetHitPoints(translation), this.mCharInfo.IslandLoc, CubeFace.backFace))
                    translation.z = actualBlock.z - this.FeetPosition.z;
                if (translation.z > 0 && this.mCharacMgr.World.HasCharacCollision(this.GetHitPoints(translation), this.mCharInfo.IslandLoc, CubeFace.frontFace))
                    translation.z = this.FeetPosition.z - actualBlock.z;*/
            }

            /* Here translate has been modified to avoid collisions */
            this.mMovementInfo.IsJumping = translation.y > 0 && this.mJumpSpeed.IsJumping;

            /* Temp */
            /*API.Geo.Cuboid.Block block = this.mCharacMgr.World.getIslandAt(this.mCharInfo.IslandLoc).getBlock((this.mNode.Position - 15 * Vector3.UNIT_Y) / Game.World.MainWorld.CUBE_SIDE, false);
            if (!(block is Game.World.Blocks.AirBlock))
                translation.y = Game.World.MainWorld.CUBE_SIDE + 100;*/

            this.mNode.Translate(translation, Mogre.Node.TransformSpace.TS_LOCAL);
        }
    }
}
