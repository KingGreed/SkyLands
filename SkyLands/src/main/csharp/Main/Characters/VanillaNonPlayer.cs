using System;
using System.Linq;
using API.Generic;
using Game.Shoot;
using Mogre;

using Game.Animation;
using Game.RTS;

namespace Game.CharacSystem
{
    class VanillaNonPlayer : VanillaCharacter
    {
        public const float  DEFAULT_NPC_LIFE = 500;
        private const float FIRE_RATE = 0.7f,
                            GUN_DECALAGE = 30;

        private float mGunTimer;
        private bool mIsLeftGun;

        public VanillaCharacter Target { get; set; }
        
        public VanillaNonPlayer(CharacMgr characMgr, string meshName, CharacterInfo info) : base(characMgr, info)
        {
            SceneManager sceneMgr = characMgr.SceneMgr;
            Entity ent = sceneMgr.CreateEntity("CharacterEnt_" + this.mCharInfo.Id, meshName);
            ent.Skeleton.BlendMode = SkeletonAnimationBlendMode.ANIMBLEND_CUMULATIVE;
            ent.SetMaterialName("Robot_T");
            this.mMesh = new Robot(ent);

            this.mNode = characMgr.SceneMgr.RootSceneNode.CreateChildSceneNode("CharacterNode_" + this.mCharInfo.Id, Vector3.ZERO, this.mMesh.InitialOrientation);
            this.mNode.AttachObject(ent);
            this.mNode.Scale(this.mMesh.MeshSize / ent.BoundingBox.Size);
            //this.mNode.Orientation = this.mMesh.InitialOrientation;

            this.FeetPosition = this.mCharInfo.SpawnPoint;
            this.mCollisionMgr = new CollisionMgr(characMgr.SceneMgr, this.mCharacMgr.World, this);
        }

        public new void Update(float frameTime)
        {
            this.mGunTimer += frameTime;

            if (this.Target == null) { return; }

            Vector3 targetPos = this.Target.Node.Position;
            Vector3 diff = targetPos - this.FeetPosition;
            Degree yawGoal = Mogre.Math.ACos(new Vector2(diff.x, diff.z).NormalisedCopy.y) * System.Math.Sign(diff.x);
            if(this.mForcedDestination.Count == 0) { this.YawTo(yawGoal); }

            float absDiffYaw = System.Math.Abs((((int)(yawGoal - this.GetYaw()).ValueAngleUnits) + 360) % 360);
            if (absDiffYaw < 10 && this.mGunTimer >= FIRE_RATE)
            {
                Vector3 decalage;
                Degree yaw = this.GetYaw();
                float val = yaw.ValueAngleUnits;
                if (val < 90 || (val >= 180 && val < 270))
                    decalage = (-Vector3.UNIT_X * Mogre.Math.Sin(val) + Vector3.UNIT_Z * Mogre.Math.Cos(val)) * GUN_DECALAGE;
                else
                {
                    val -= 90;
                    decalage = (Vector3.UNIT_X * Mogre.Math.Cos(val) - Vector3.UNIT_Z * Mogre.Math.Sin(val)) * GUN_DECALAGE;
                }
                if (this.mCharacMgr.BulletMgr.AddBulletAndHit(this, decalage * (this.mIsLeftGun ? -1 : 1), this.Target))
                    ((Robot)this.mMesh).Shoot();

                this.mGunTimer = 0;
                this.mIsLeftGun = !this.mIsLeftGun;
            }
        }

        public override void updateTargets()
        {
            float actTargetDistance = this.Target != null ? (this.Target.FeetPosition - this.FeetPosition).Length : LIMIT_TARGET_DISTANCE + 1;
            this.Target = null;
            foreach (Faction faction in Enum.GetValues(typeof(Faction)).Cast<Faction>().Where(faction => faction != this.mCharInfo.Faction))
            {
                foreach (VanillaCharacter ennemy in this.mCharacMgr.GetFactionCharacters(faction))
                {
                    Vector3 diff = ennemy.FeetPosition - this.FeetPosition;
                    float distance = diff.Length;
                    if (distance < actTargetDistance && distance < LIMIT_TARGET_DISTANCE && System.Math.Abs(diff.y / Cst.CUBE_SIDE) <= 6)
                        this.Target = ennemy;
                }
            }
            
            base.updateTargets();
        }
    }
}
