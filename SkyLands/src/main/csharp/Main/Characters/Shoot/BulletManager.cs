using System.Collections.Generic;
using API.Generic;
using Game.World.Display;
using Mogre;

using Game.CharacSystem;
using Game.World;
using Game.World.Blocks;

namespace Game.Shoot
{
    public class BulletManager
    {
        private readonly List<Bullet> mBullets;

        public SceneManager  SceneMgr  { get; private set; }
        public CharacMgr     CharacMgr { get; private set; }
        public API.Geo.World World     { get; private set; }

        public BulletManager(SceneManager sceneMgr, API.Geo.World world)
        {
            this.SceneMgr = sceneMgr;
            this.World = world;
            this.mBullets = new List<Bullet>();
        }

        public void AttachCharacMgr(CharacMgr characMgr) { this.CharacMgr = characMgr; }

        public bool AddBulletAndHit(VanillaCharacter source, Vector3 decalage, VanillaCharacter target)
        {
            Vector3 sourcePoint = source.FeetPosition + (source.Size.y / 2) * Vector3.UNIT_Y + decalage;
            Vector3 targetPoint = target.FeetPosition + target.Size.y / 2 * Vector3.UNIT_Y;
            Vector3 diff = targetPoint - sourcePoint;
            if (Math.Abs(diff.y / Cst.CUBE_SIDE) > 6) { return false; }

            Degree pitch = -Math.ATan2(diff.y,  diff.z);
            Degree yaw = source.GetYaw();
            if (yaw.ValueAngleUnits > 90 && yaw.ValueAngleUnits < 270)
            {
                yaw *= -1;
                yaw += new Degree(180);
            }

            float targetDistance = diff.Length;
            Ray ray = new Ray(sourcePoint + Vector3.NEGATIVE_UNIT_Z, diff.NormalisedCopy);
            float blockDistance = VanillaBlock.getBlockOnRay(source.getIsland(), ray, Bullet.DEFAULT_RANGE, 30);
            if (targetDistance > blockDistance) { return false; }

            SceneNode pitchNode = this.SceneMgr.RootSceneNode.CreateChildSceneNode();
            pitchNode.Position = sourcePoint;
            pitchNode.Pitch(pitch);

            SceneNode yawNode = pitchNode.CreateChildSceneNode();
            yawNode.Yaw(yaw);
            yawNode.AttachObject(StaticRectangle.CreateLine(this.SceneMgr, Vector3.ZERO, new Vector3(0, 0, 15), ColoredMaterials.YELLOW));

            this.mBullets.Add(new Bullet(source, target, pitchNode, yawNode));
            return true;
        }

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
