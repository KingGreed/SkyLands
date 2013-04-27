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
    public class BulletManager
    {
        private SceneManager mSceneMgr;
        private List<Bullet> mBullets;
        
        public BulletManager(SceneManager sceneMgr)
        {
            this.mSceneMgr = sceneMgr;
            this.mBullets = new List<Bullet>();
        }

        public void AddBullet(Bullet b)
        {
            this.mBullets.Add(b);
        }

        public void Update(float frameTime)
        {
            foreach (Bullet b in this.mBullets)
            {
                b.Update(frameTime);
            }
        }
    }
}
