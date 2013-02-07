using System;
using System.Collections.Generic;
using Mogre;
using MogreNewt;
using MogreNewt.CollisionPrimitives;

using Game.World;

namespace Game.CharacSystem
{
    public abstract class VanillaCharacter
    {
        private readonly Vector3 CHARAC_SIZE = new Vector3(70, 100, 70);
        private const float WALK_SPEED = 30.0f;
        /*private const float GRAVITY_ACCEL_T0 = -750;
        private const float GRAVITY_ACCEL_TMAX = -1000;
        private const float T_MAX = 4;  // Time until the character reach its max speed fall
        private const float GRAVITY_CONST_B = T_MAX * (GRAVITY_ACCEL_TMAX - 1) / (GRAVITY_ACCEL_T0 - GRAVITY_ACCEL_TMAX);
        private const float GRAVITY_CONST_A = GRAVITY_ACCEL_T0 * GRAVITY_CONST_B;*/

        protected CharacMgr     mCharacMgr;
        protected SceneNode     mNode;
        protected AnimationMgr  mAnimMgr;
        protected CharacterInfo mCharInfo;
        protected MovementInfo  mMovementInfo;
        protected Anim[]        mRunAnims;
        private Anim[]          mIdleAnims;
        private Body            mBody;

        public SceneNode Node         { get { return this.mNode; } }
        public bool      IsMoving     { get { return this.mMovementInfo.IsMoving; } }
        public float     Height       { get { return this.mBody.BoundingBox.Size.y + 13; } }
        public Vector3   FeetPosition
        {
            get { return this.mNode.Position - new Vector3(0, this.Height / 2, 0); }
            set { this.mNode.SetPosition(value.x, value.y + this.Height / 2, value.z); }
        }

        protected VanillaCharacter(CharacMgr characMgr, string meshName, CharacterInfo charInfo)
        {
            this.mCharacMgr = characMgr;
            this.mCharInfo = charInfo;
            this.mMovementInfo = new MovementInfo();
            
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

            /* Collisions */
            TreeCollisionSceneParser col = new TreeCollisionSceneParser(this.mCharacMgr.World.getNewtWorld());
            col.ParseScene(this.mNode, true, this.mCharInfo.Id);
            this.mBody = new Body(this.mCharacMgr.World.getNewtWorld(), col, 0, false);
            col.Dispose();
            this.mBody.AttachNode(this.mNode);
            this.mBody.AutoSleep = true;
            this.FeetPosition = this.mCharInfo.SpawnPoint;

            /* Create Animations */
            this.mIdleAnims = new Anim[] { Anim.IdleBase, Anim.IdleTop };
            this.mRunAnims  = new Anim[] { Anim.RunBase, Anim.RunTop };
            this.mAnimMgr   = new AnimationMgr(ent.AllAnimationStates);
            this.mAnimMgr.SetAnims(this.mIdleAnims);
        }

        public void Update(float frameTime)
        {
            /* Apply gravity */
            Vector3 translation = new Vector3();
            /*if (!this.mCharacMgr.World.HasCharacCollision(this.mBlocksCollision, CubeFace.underFace))
            {
                if (!this.mAnimMgr.CurrentAnims.Contains(Anim.JumpLoop))
                {
                    this.mAnimMgr.SetAnims(Anim.JumpLoop);
                    this.mTimeOfFall.Reset();
                }

                float sec = (float)this.mTimeOfFall.Milliseconds / 1000;
                if (sec >= T_MAX) { translation += GRAVITY_ACCEL_TMAX * Vector3.UNIT_Y; }
                else { translation += (sec + GRAVITY_CONST_A) / (sec + GRAVITY_CONST_B) * Vector3.UNIT_Y; }
            }
            else if (this.mAnimMgr.CurrentAnims.Contains(Anim.JumpLoop))  // The character has reach the ground
                this.mAnimMgr.SetAnims(Anim.JumpEnd);
            float sec = (float)this.mTimeOfFall.Milliseconds / 1000;
            if (sec >= T_MAX) { translation += GRAVITY_ACCEL_TMAX * Vector3.UNIT_Y; }
            else { translation += (sec + GRAVITY_CONST_A) / (sec + GRAVITY_CONST_B) * Vector3.UNIT_Y; }*/

            /* Actualise mMovementInfo */
            if (this.mCharInfo.IsPlayer) { (this as VanillaPlayer).Update(frameTime); }
            else                         { (this as VanillaNonPlayer).Update(frameTime); }

            /* Apply mMovementInfo */
            if (this.mMovementInfo.IsMoving)
            {
                translation += WALK_SPEED * this.mMovementInfo.MoveDirection;
                //this.mNode.Yaw(this.mMovementInfo.YawValue * frameTime);
                this.mBody.AddTorque(new Vector3(0, 0, this.mMovementInfo.YawValue));
            }
            //this.mNode.Translate(translation * frameTime, Mogre.Node.TransformSpace.TS_LOCAL);
            this.mBody.Velocity = translation * this.mNode.LocalAxes.Transpose();
            //this.mBody.AddForce(translation * this.mNode.LocalAxes.Transpose());

            /* Update animations */
            if (this.mAnimMgr.CurrentAnims.Count == 0) { this.mAnimMgr.AddAnims(this.mIdleAnims); } // By default apply idle anim
            this.mAnimMgr.Update(frameTime);
            this.mMovementInfo.ClearInfo();
        }

        public void Dispose()
        {
            this.mBody.Dispose();
        }
    }
}
