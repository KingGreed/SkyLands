using System;
using Mogre;

using Game.World;
using Game.Animation;
using Game.Shoot;

namespace Game.CharacSystem
{
    class VanillaNonPlayer : VanillaCharacter
    {
        public VanillaNonPlayer(CharacMgr characMgr, string meshName, CharacterInfo info) : base(characMgr, meshName, info)
        {
            SceneManager sceneMgr = characMgr.SceneMgr;
            Entity ent = sceneMgr.CreateEntity("CharacterEnt_" + this.mCharInfo.Id, meshName);
            ent.Skeleton.BlendMode = SkeletonAnimationBlendMode.ANIMBLEND_CUMULATIVE;
            ent.SetMaterialName("Robot_T");
            this.mMesh = new Robot(ent);

            this.mNode.AttachObject(ent);
            this.mNode.Scale(this.mMesh.MeshSize / ent.BoundingBox.Size);

            this.FeetPosition = this.mCharInfo.SpawnPoint;
            this.mCollisionMgr = new CollisionMgr(characMgr.SceneMgr, this.mCharacMgr.World, this);
        }

        public new void Update(float frameTime)
        {
        }
    }
}
