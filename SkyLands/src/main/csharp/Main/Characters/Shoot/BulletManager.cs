using System.Collections.Generic;
using Mogre;

using Game.CharacSystem;
using Game.World;

namespace Game.Shoot
{
    public class BulletManager
    {
        private readonly List<Bullet> mBullets;

        public SceneManager SceneMgr  { get; private set; }
        public CharacMgr    CharacMgr { get; private set; }
        public MainWorld    World     { get; private set; }

        public BulletManager(SceneManager sceneMgr, MainWorld world)
        {
            this.SceneMgr = sceneMgr;
            this.World = world;
            this.mBullets = new List<Bullet>();
        }

        public void AttachCharacMgr(CharacMgr characMgr) { this.CharacMgr = characMgr; }

        public void AddBullet(Bullet b) { this.mBullets.Add(b); }

        public void RemoveBullet(Bullet b)
        {
            b.Dispose(this.SceneMgr);
            this.mBullets.Remove(b);
        }

        public void Update(float frameTime)
        {
            for(int i = 0; i < this.mBullets.Count; i++)
            {
                if (!this.mBullets[i].Update(this, frameTime))
                {
                    this.RemoveBullet(this.mBullets[i]);
                    i--;
                }
            }
        }

        public void Dispose()
        {
            while (this.mBullets.Count > 0)
            {
                this.mBullets[0].Dispose(this.SceneMgr);
                this.mBullets.RemoveAt(0);
            }
        }
    }
}
