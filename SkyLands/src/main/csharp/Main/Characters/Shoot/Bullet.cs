using Mogre;

using API.Generic;
using API.Geo.Cuboid;

using Game.CharacSystem;
using Game.World;
using Game.World.Display;
using Game.World.Blocks;

namespace Game.Shoot
{
    public class Bullet
    {
        public const float DEFAULT_RANGE = 30 * Cst.CUBE_SIDE,
                           DEFAULT_SPEED = 100,
                           DEFAULT_DAMAGE = 5;
        
        private readonly VanillaCharacter mSource;
        private readonly SceneNode        mNode;
        private Ray                       mRay;
        private float                     mDistTravalled;
        private bool                      mAccurateTest;

        public float Speed  { get; set; }
        public float Range  { get; set; }
        public float Damage { get; set; }

        public Bullet(SceneManager sceneMgr, VanillaCharacter source, float decalage, VanillaCharacter target)
        {
            this.mSource = source;
            this.mAccurateTest = false;
            Vector3 position = source.Node.Position + Vector3.UNIT_X*decalage;
            float targetDistance = (target.Node.Position - source.Node.Position).Length;

            this.Speed = DEFAULT_SPEED;
            this.Range = DEFAULT_RANGE;
            this.Damage = DEFAULT_DAMAGE;

            this.mRay = new Ray(position, (target.Node.Position - position).NormalisedCopy);
            Vector3 relBlockPos;
            Block actBlock;
            float blockDistance = VanillaBlock.getBlockOnRay(source.getIsland(), this.mRay, this.Range, 30, out relBlockPos, out actBlock);
            if (targetDistance < blockDistance) // Hit the target now
                target.Hit(this.Damage);

            this.mNode = sceneMgr.RootSceneNode.CreateChildSceneNode();
            this.mNode.Position = position;
            this.mNode.Orientation = new Quaternion();
            this.mNode.AttachObject(StaticRectangle.CreateLine(sceneMgr, Vector3.ZERO, new Vector3(0, 0, 50), ColoredMaterials.YELLOW));
            //StaticRectangle.DrawLine(sceneMgr, position, target.Node.Position, ColoredMaterials.YELLOW);
        }
        
        public Bullet(VanillaCharacter source, SceneNode node)
        {
            this.mSource = source;
            this.mNode = node;
            this.mRay = new Ray(this.mNode.Position, this.mNode.Orientation * Vector3.NEGATIVE_UNIT_Z);
            this.mAccurateTest = true;
        }

        public bool Update(BulletManager bulletMgr, float frameTime)
        {
            if (this.mDistTravalled > this.Range)
                return false;

            float distance = this.Speed * frameTime;
            this.mDistTravalled += distance;
            this.mNode.Translate(distance * Vector3.NEGATIVE_UNIT_Z, Node.TransformSpace.TS_LOCAL);

            if (this.mAccurateTest)
            {
                this.mRay.Origin = this.mNode.Position;

                if (!(bulletMgr.World.getIsland().getBlock(MainWorld.getRelativeFromAbsolute(this.mNode.Position), false) is World.Blocks.Air))
                    return false;

                RaySceneQuery raySQuery = bulletMgr.SceneMgr.CreateRayQuery(this.mRay);
                raySQuery.SetSortByDistance(true);

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
                                bulletMgr.CharacMgr.GetCharacterById(id).Hit(this.Damage);
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        public virtual void Dispose(SceneManager sceneMgr)   // Has to be called from the BulletManager only.
        {
            this.mNode.RemoveAndDestroyAllChildren();
            sceneMgr.DestroySceneNode(this.mNode);
        }
    }
}
