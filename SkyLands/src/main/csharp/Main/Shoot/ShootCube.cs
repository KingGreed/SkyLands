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
    class ShootCube
    {
        private const float SCALE_SPEED = 1.4f;
        private const float MAX_SCALE = 1.9f;
        private const float FIRE_RATE = 500;   // Fire every 500ms
        
        private SceneManager  mSceneMgr;
        private VanillaPlayer mPlayer;
        private BulletManager mBulletMgr;
        private SceneNode     mNode;
        private Timer         mTimeSinceLastBall;
        private bool          mCreated = false;
        private string        mMaterial;

        public string Material { set { this.mMaterial = value; } }

        public ShootCube(SceneManager sceneMgr, VanillaPlayer player, BulletManager bulletManager)
        {
            this.mSceneMgr = sceneMgr;
            this.mPlayer = player;
            this.mBulletMgr = bulletManager;
            this.mTimeSinceLastBall = new Timer();
        }

        public void Grow(float frameTime, bool allowCreation)
        {
            if (!mCreated)
            {
                if (allowCreation && this.mTimeSinceLastBall.Milliseconds >= FIRE_RATE)
                    this.Create();
            }
            else
            {
                this.mNode.Scale((1 + SCALE_SPEED * frameTime) * Vector3.UNIT_SCALE);
                if (this.mNode.GetScale().x >= MAX_SCALE)
                {
                    this.mNode.Scale(MAX_SCALE * Vector3.UNIT_SCALE);
                    this.Burst();
                }
            }
        }

        public void Burst()
        {
            if (this.mCreated)
            {
                this.mCreated = false;
                this.mTimeSinceLastBall.Reset();
                this.mBulletMgr.AddBullet(new Bullet(this.mBulletMgr, this.mNode, this.mPlayer, this.mNode.GetScale().x * 150 - 60));
            }
        }

        private void Create()
        {
            int faceNumber = 0;
            ManualObject ball = new ManualObject(this.mMaterial + "-" + Guid.NewGuid().ToString());
            ball.Begin(this.mMaterial, RenderOperation.OperationTypes.OT_TRIANGLE_LIST);
            foreach(BlockFace face in Enum.GetValues(typeof(BlockFace)))
            {
                for(int i = 0; i < 4; i++)
                {
                    ball.Position(VanillaMultiBlock.blockPointCoords[(int)face * 4 + i]/10); ball.TextureCoord(VanillaMultiBlock.textureCoord[(int)face * 4 + i]);
                    ball.Normal(VanillaMultiBlock.normals[(int)face]);
                    faceNumber++;
                }
                ball.Quad((uint)faceNumber - 4, (uint)faceNumber - 3, (uint)faceNumber - 2, (uint)faceNumber - 1);
            }
            ball.End();

            /* Determine the location */
            Ray ray = this.mPlayer.Camera.GetCameraToViewportRay(0.5f, 0.5f);
            ray.Origin = this.mPlayer.FeetPosition + Vector3.UNIT_Y * this.mPlayer.Size / 1.2f;

            this.mNode = /*this.mPlayer.Node.CreateChildSceneNode(ray.GetPoint(50));*/ this.mSceneMgr.RootSceneNode.CreateChildSceneNode(ray.GetPoint(50));
            this.mNode.InheritScale = false;
            this.mNode.InheritOrientation = false;
            this.mNode.Orientation = this.mPlayer.Camera.RealOrientation;
            this.mNode.AttachObject(ball);
            this.mCreated = true;
        }
    }
}
