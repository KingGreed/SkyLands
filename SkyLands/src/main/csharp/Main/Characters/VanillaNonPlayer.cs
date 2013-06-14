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
        public const float  DEFAULT_NPC_LIFE = 500;
        private const float FIRE_RATE = 1.6f,
                            GUN_DECALAGE = 5;

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
                this.mCharacMgr.BulletMgr.AddBullet(new Bullet(this.mCharacMgr.SceneMgr, this, this.Target, GUN_DECALAGE * (this.mIsLeftGun ? -1 : 1)));
                this.mGunTimer = 0;
                this.mIsLeftGun = !this.mIsLeftGun;

                ((Robot)this.mMesh).Shoot();
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
