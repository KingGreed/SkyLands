using Game.CharacSystem;

using Mogre;

namespace Game.Shoot
{
    class ShootCube
    {
        private const float SCALE_SPEED = 1.3f;
        private const float INIT_SCALE = 0.05f;
        private const float MAX_SCALE = 0.12f;
        private const float FIRE_RATE = 430;   // Fire every x ms

        private readonly BulletManager mBulletMgr;
        private readonly VanillaPlayer mPlayer;
        private readonly Timer         mTimeSinceLastBall;
        private SceneNode              mNode;
        private bool                   mCreated;

        public string Material { get; set; }

        public ShootCube(BulletManager bulletManager, VanillaPlayer player)
        {
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
                    this.mNode.SetScale(MAX_SCALE * Vector3.UNIT_SCALE);
                    this.Burst();
                }
            }
        }

        public void Burst()
        {
            if (!this.mCreated) { return; }

            this.mCreated = false;
            this.mTimeSinceLastBall.Reset();
            this.mBulletMgr.AddBullet(new Bullet(this.mBulletMgr, this.mPlayer, this.mNode)
            {
                Damage = (0.5f + (this.mNode.GetScale().x - INIT_SCALE) / (MAX_SCALE - INIT_SCALE)) * 130,
                Range = Bullet.DEFAULT_RANGE,
                Speed = 1200
            });
        }

        private void Create()
        {
            Entity cube = this.mBulletMgr.SceneMgr.CreateEntity("cube.mesh");
            cube.SetMaterialName(this.Material);

            /* Determine the location */
            Camera cam = this.mPlayer.MainPlayerCam;
            Ray ray = cam.GetCameraToViewportRay(0.5f, 0.5f);
            ray.Origin = this.mPlayer.FeetPosition + Vector3.UNIT_Y * this.mPlayer.Size / 1.2f;

            this.mNode = this.mPlayer.Node.CreateChildSceneNode(ray.GetPoint(50));// this.mBulletMgr.SceneMgr.RootSceneNode.CreateChildSceneNode(ray.GetPoint(50));
            this.mNode.SetVisible(true);
            //this.mNode.
            this.mNode.InheritScale = false;
            this.mNode.SetScale(INIT_SCALE * Vector3.UNIT_SCALE);
            this.mNode.InheritOrientation = false;
            this.mNode.Orientation = cam.RealOrientation;
            this.mNode.AttachObject(cube);
            this.mCreated = true;
        }
    }
}
