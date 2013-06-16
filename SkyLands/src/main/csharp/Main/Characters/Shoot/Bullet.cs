using Mogre;

using API.Generic;

using Game.CharacSystem;
using Game.World;
using Game.World.Display;
using Game.World.Blocks;

namespace Game.Shoot
{
    public class Bullet
    {
        public const float DEFAULT_RANGE = 30 * Cst.CUBE_SIDE,
                           DEFAULT_SPEED = 2000,
                           DEFAULT_DAMAGE = 2;
        
        private readonly VanillaCharacter mSource;
        private readonly SceneNode mYawNode, mPitchNode;
        private Ray     mRay;
        private Vector3 mForwardDir;
        private float   mDistTravalled;
        private bool    mAccurateTest;

        public float Speed  { get; set; }
        public float Range  { get; set; }
        public float Damage { get; set; }

        public Bullet(VanillaCharacter source, VanillaCharacter target, SceneNode pitchNode, SceneNode yawNode)
        {
            this.mSource = source;
            this.mAccurateTest = false;
            this.mForwardDir = Vector3.UNIT_Z;

            this.Speed = DEFAULT_SPEED;
            this.Range = DEFAULT_RANGE;
            this.Damage = DEFAULT_DAMAGE;

            target.Hit(this.Damage); // Hit the target now

            this.mPitchNode = pitchNode;
            this.mYawNode = yawNode;
        }
        
        public Bullet(VanillaCharacter source, SceneNode node, Vector3 forwardDir)
        {
            this.mSource = source;
            this.mYawNode = node;
            this.mRay = new Ray(this.mYawNode.Position, this.mYawNode.Orientation * Vector3.NEGATIVE_UNIT_Z);
            this.mAccurateTest = true;
            this.mForwardDir = forwardDir;
        }

        public bool Update(BulletManager bulletMgr, float frameTime)
        {
            if (this.mDistTravalled >= this.Range)
                return false;

            float distance = this.Speed * frameTime;
            this.mDistTravalled += distance;
            this.mYawNode.Translate(distance * this.mForwardDir, Node.TransformSpace.TS_LOCAL);

            if (this.mAccurateTest)
            {
                this.mRay.Origin = this.mYawNode.Position;

                if (!(bulletMgr.World.getIsland().getBlock(MainWorld.getRelativeFromAbsolute(this.mYawNode.Position), false) is Air))
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
            if (this.mPitchNode != null)
            {
                this.mPitchNode.RemoveAndDestroyAllChildren();
                sceneMgr.DestroySceneNode(this.mPitchNode);
            }
            else if(this.mYawNode != null)
            {
                this.mYawNode.RemoveAndDestroyAllChildren();
                sceneMgr.DestroySceneNode(this.mYawNode);
            }
        }
    }
}
