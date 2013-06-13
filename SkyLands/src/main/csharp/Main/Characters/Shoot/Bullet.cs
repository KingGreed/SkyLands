using Mogre;

using API.Generic;
using Game.CharacSystem;
using Game.World;

namespace Game.Shoot
{
    public class Bullet
    {
        public const int DEFAULT_RANGE = 30 * Cst.CUBE_SIDE;
        
        private readonly BulletManager mBulletMgr;
        private readonly VanillaCharacter mSource;
        private readonly SceneNode mNode;
        private Ray mRay;
        private float mDistTravalled;

        public float Speed         { get; set; }
        public float Range         { get; set; }
        public float Damage        { get; set; }

        public Bullet(BulletManager bulletMgr, VanillaCharacter source, SceneNode node)
        {
            this.mBulletMgr = bulletMgr;
            this.mSource = source;
            this.mNode = node;
            this.mRay = new Ray(this.mNode.Position, this.mNode.Orientation * Vector3.NEGATIVE_UNIT_Z);
        }

        public bool Update(float frameTime)
        {
            if (this.mDistTravalled > this.Range)
                return false;

            float distance = this.Speed * frameTime;
            this.mDistTravalled += distance;
            this.mNode.Translate(distance * Vector3.NEGATIVE_UNIT_Z, Node.TransformSpace.TS_LOCAL);
            this.mRay.Origin = this.mNode.Position;

            if (!(this.mBulletMgr.World.getIsland().getBlock(MainWorld.getRelativeFromAbsolute(this.mNode.Position), false) is World.Blocks.Air))
                return false;

            RaySceneQuery raySQuery = this.mBulletMgr.SceneMgr.CreateRayQuery(this.mRay);
            raySQuery.SetSortByDistance(true);
            //Game.World.Display.StaticRectangle.DrawLine(this.mBulletMgr.SceneMgr, this.mRay.Origin, this.mRay.GetPoint(distance));

            foreach (RaySceneQueryResultEntry raySQREntry in raySQuery.Execute())
            {
                if (raySQREntry.movable != null && raySQREntry.distance > 0 && raySQREntry.distance <= distance)
                {
                    string[] s = raySQREntry.movable.Name.Split('_');
                    if (s.Length == 2 && s[0] == "CharacterEnt")
                    {
                        int id = int.Parse(s[1]);
                        if (id != this.mSource.Info.Id)
                        {
                            this.mBulletMgr.CharacMgr.GetCharacterById(id).Hit(this.Damage);
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public virtual void Dispose()   // Has to be called from the BulletManager only. Else use mBulletManager.RemoveBullet(this) instead
        {
            this.mNode.RemoveAndDestroyAllChildren();
            this.mBulletMgr.SceneMgr.DestroySceneNode(this.mNode);
        }
    }
}
