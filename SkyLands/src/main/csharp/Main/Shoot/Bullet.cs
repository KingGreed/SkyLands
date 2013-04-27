using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Game.World.Generator;
using API.Generic;
using Game.CharacSystem;

using Mogre;

namespace Game.Shoot
{
    public class Bullet
    {
        private SceneManager mSceneMgr;
        private SceneNode mNode;
        private float mSpeed;
        private Ray mRay;

        public Bullet(SceneManager sceneMgr, Degree yaw)
        {
            this.Constr(sceneMgr, null, 1500, yaw);
        }

        public Bullet(SceneManager sceneMgr, SceneNode node, Degree yaw, float speed = 1200)
        {
            this.Constr(sceneMgr, node, speed, yaw);
        }

        private void Constr(SceneManager sceneMgr, SceneNode node, float speed, Degree yaw)
        {
            this.mSceneMgr = sceneMgr;
            this.mNode = node;
            //Vector3 dir = Vector3.UNIT_X * Mogre.Math.Sin(yaw) + Vector3.UNIT_Z * Mogre.Math.Cos(yaw);
            //this.mRay = new Ray(this.mNode.Position, new Vector3(1, 0, 0));
            this.mSpeed = speed;

        }

        public void Update(float frameTime)
        {
            Vector3 translation = this.mSpeed * frameTime * Vector3.UNIT_Z * this.mNode.LocalAxes;
            float distance = (this.mNode.Position - translation).Length;

            RaySceneQuery raySQuery = this.mSceneMgr.CreateRayQuery(mRay);
            raySQuery.SetSortByDistance(true);

            Vector3 rayHitPoint;

            foreach (RaySceneQueryResultEntry raySQREntry in raySQuery.Execute())
            {
                rayHitPoint = mRay.Origin + (mRay.Direction * raySQREntry.distance);
                
                if (raySQREntry.movable != null)
                {
                    
                }
            }

            this.mNode.Translate(this.mSpeed * frameTime * Vector3.NEGATIVE_UNIT_Z, Node.TransformSpace.TS_LOCAL);
        }
    }
}
