using System;
using System.Linq;
using Game.Shoot;
using Mogre;

using Game.Animation;
using Game.RTS;

namespace Game.CharacSystem
{
    class VanillaNonPlayer : VanillaCharacter
    {
        public const float DEFAULT_NPC_LIFE = 500;
        private const float FIRE_RATE = 1.2f,
                            GUN_DECALAGE = 5;

        private float mLeftGunTimer, mRightGunTimer;

        public VanillaCharacter Target { get; set; }
        
        public VanillaNonPlayer(CharacMgr characMgr, string meshName, CharacterInfo info) : base(characMgr, info)
        {
            SceneManager sceneMgr = characMgr.SceneMgr;
            Entity ent = sceneMgr.CreateEntity("CharacterEnt_" + this.mCharInfo.Id, meshName);
            ent.Skeleton.BlendMode = SkeletonAnimationBlendMode.ANIMBLEND_CUMULATIVE;
            ent.SetMaterialName("Robot_T");
            this.mMesh = new Robot(ent);

            this.mNode.AttachObject(ent);
            this.mNode.Scale(this.mMesh.MeshSize / ent.BoundingBox.Size);
            this.mNode.Orientation = this.mMesh.InitialOrientation;

            this.FeetPosition = this.mCharInfo.SpawnPoint;
            this.mCollisionMgr = new CollisionMgr(characMgr.SceneMgr, this.mCharacMgr.World, this);
            this.mLeftGunTimer = FIRE_RATE / 2; // Non synchronous shoot
        }

        public new void Update(float frameTime)
        {
            this.mLeftGunTimer += frameTime;
            this.mRightGunTimer += frameTime;

            if (this.Target == null) { return; }

            Vector3 targetPos = this.Target.Node.Position;
            Vector3 diff = targetPos - this.FeetPosition;
            Degree yawGoal = Mogre.Math.ACos(new Vector2(diff.x, diff.z).NormalisedCopy.y) * System.Math.Sign(diff.x);
            if(this.mForcedDestination.Count == 0) { this.YawTo(yawGoal); }

            float absDiffYaw = System.Math.Abs((((int)(yawGoal - this.GetYaw()).ValueAngleUnits) + 360) % 360);
            bool leftGun = this.mLeftGunTimer >= FIRE_RATE;
            if (absDiffYaw < 10 && (leftGun || this.mRightGunTimer >= FIRE_RATE))
            {
                this.mCharacMgr.BulletMgr.AddBullet(new Bullet(this.mCharacMgr.SceneMgr, this, GUN_DECALAGE * (leftGun ? -1 : 1), this.Target));
                if (leftGun) { this.mLeftGunTimer = 0; Console.WriteLine("*****Shoot******"); }
                else         { this.mRightGunTimer = 0; }
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
                    float distance = (ennemy.FeetPosition - this.FeetPosition).Length;
                    if (distance < actTargetDistance && distance < LIMIT_TARGET_DISTANCE)
                        this.Target = ennemy;
                }
            }
            
            base.updateTargets();
        }
    }
}
