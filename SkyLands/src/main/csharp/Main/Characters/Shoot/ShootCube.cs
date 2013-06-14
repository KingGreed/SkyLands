using Game.CharacSystem;

using Mogre;

namespace Game.Shoot
{
    class ShootCube
    {
        private const float SCALE_SPEED = 1.3f;
        private const float INIT_SCALE = 0.05f;
        private const float MAX_SCALE = 0.12f;
        private const float FIRE_RATE = 0.43f;

        private readonly BulletManager mBulletMgr;
        private readonly VanillaPlayer mSource;
        private readonly Timer         mTimeSinceLastBall;
        private SceneNode              mNode;
        private readonly Camera        mCamera;
        private Ray                    mRay;
        private bool                   mCreated;

        public string Material { get; set; }

        public ShootCube(BulletManager bulletManager, VanillaPlayer source)
        {
            this.mSource = source;
            this.mBulletMgr = bulletManager;
            this.mTimeSinceLastBall = new Timer();
            this.mCamera = this.mBulletMgr.SceneMgr.GetCamera("Camera");
            this.mRay = this.mCamera.GetCameraToViewportRay(0.5f, 0.5f);
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
                this.Reposition(); 
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
            this.mBulletMgr.AddBullet(new Bullet(this.mSource, this.mNode)
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

            this.mNode = this.mBulletMgr.SceneMgr.RootSceneNode.CreateChildSceneNode();
            this.mNode.SetScale(INIT_SCALE * Vector3.UNIT_SCALE);
            this.mNode.AttachObject(cube);
            this.Reposition();
            this.mCreated = true;
        }

        private void Reposition()
        {
            this.mRay.Origin = this.mSource.FeetPosition + Vector3.UNIT_Y * this.mSource.Size / 1.2f;
            this.mNode.Position = this.mRay.GetPoint(50);

            this.mNode.Orientation = this.mCamera.RealOrientation;
        }
    }
}
