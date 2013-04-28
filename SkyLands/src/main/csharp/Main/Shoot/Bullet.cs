using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Game.World.Generator;
using API.Generic;
using Game.CharacSystem;
using Game.World;

using Mogre;

namespace Game.Shoot
{
    public class Bullet
    {
        private const float MAX_LIFE_TIME = 2.5f;
        
        private BulletManager mBulletMgr;
        private SceneNode mNode;
        private VanillaCharacter mCharacSource;
        private float mSpeed, mTimeSinceCreation;
        private Ray mRay;
        private float mDamage;

        public float Damage { get { return this.mDamage; } }

        public Bullet(BulletManager bulletMgr, VanillaCharacter source, float damage)
        {
            this.Constr(bulletMgr, null, source, 1500, damage);
        }

        public Bullet(BulletManager bulletMgr, SceneNode node, VanillaCharacter source, float damage, float speed = 1200)
        {
            this.Constr(bulletMgr, node, source, speed, damage);
        }

        private void Constr(BulletManager bulletMgr, SceneNode node, VanillaCharacter source, float speed, float damage)
        {
            this.mBulletMgr = bulletMgr;
            this.mNode = node;
            this.mRay = new Ray(this.mNode.Position, this.mNode.Orientation * Vector3.NEGATIVE_UNIT_Z);
            this.mSpeed = speed;
            this.mTimeSinceCreation = 0;
            this.mDamage = damage;
            this.mCharacSource = source;
        }

        public bool Update(float frameTime)
        {
            if (this.mTimeSinceCreation >= MAX_LIFE_TIME)
                return false;
            else
            {
                this.mTimeSinceCreation += frameTime;
                float distance = this.mSpeed * frameTime;
                this.mNode.Translate(distance * Vector3.NEGATIVE_UNIT_Z, Node.TransformSpace.TS_LOCAL);
                this.mRay.Origin = this.mNode.Position;

                if (!(this.mBulletMgr.World.getIsland().getBlock(MainWorld.getRelativeFromAbsolute(this.mNode.Position), false) is Game.World.Blocks.Air))
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
                            if (id != this.mCharacSource.Info.Id)
                            {
                                this.mBulletMgr.CharacMgr.GetCharacterById(id).Hit(this);
                                return false;
                            }
                        }
                    }
                }

                return true;
            }
        }

        /*private void CreateCube(Vector3 pos)    // For test purpose
        {
            int faceNumber = 0;
            ManualObject ball = new ManualObject("fireBall-" + Guid.NewGuid().ToString());
            ball.Begin("fireball", RenderOperation.OperationTypes.OT_TRIANGLE_LIST);
            foreach (BlockFace face in Enum.GetValues(typeof(BlockFace)))
            {
                for (int i = 0; i < 4; i++)
                {
                    ball.Position(VanillaMultiBlock.blockPointCoords[(int)face * 4 + i] / 10); ball.TextureCoord(VanillaMultiBlock.textureCoord[(int)face * 4 + i]);
                    ball.Normal(VanillaMultiBlock.normals[(int)face]);
                    faceNumber++;
                }
                ball.Quad((uint)faceNumber - 4, (uint)faceNumber - 3, (uint)faceNumber - 2, (uint)faceNumber - 1);
            }
            ball.End();

            SceneNode node = this.mBulletMgr.SceneMgr.RootSceneNode.CreateChildSceneNode();
            node.Position = pos;
            node.AttachObject(ball);
        }*/

        public void Dispose()   // Has to be called from the BulletManager only. Else use mBulletManager.RemoveBullet(this) instead
        {
            this.mNode.RemoveAndDestroyAllChildren();
            this.mBulletMgr.SceneMgr.DestroySceneNode(this.mNode);
        }
    }
}
