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
    public class BulletManager
    {
        private SceneManager mSceneMgr;
        private CharacMgr mCharacMgr;
        private MainWorld mWorld;
        private List<Bullet> mBullets;

        public SceneManager SceneMgr  { get { return this.mSceneMgr; } }
        public CharacMgr    CharacMgr { get { return this.mCharacMgr; } }
        public MainWorld    World     { get { return this.mWorld; } }
        
        public BulletManager(SceneManager sceneMgr, MainWorld world)
        {
            this.mSceneMgr = sceneMgr;
            this.mWorld = world;
            this.mBullets = new List<Bullet>();
        }

        public void AttachCharacMgr(CharacMgr characMgr)
        {
            this.mCharacMgr = characMgr;
        }

        public void AddBullet(Bullet b)
        {
            this.mBullets.Add(b);
        }

        public void RemoveBullet(Bullet b)
        {
            b.Dispose();
            this.mBullets.Remove(b);
        }

        public void Update(float frameTime)
        {
            for(int i = 0; i < this.mBullets.Count; i++)
            {
                if (!this.mBullets[i].Update(frameTime))
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
                this.mBullets[0].Dispose();
                this.mBullets.RemoveAt(0);
            }
        }
    }
}
