using System;
using Mogre;

namespace Game.World.Display {
    class ParticleGenerator {

        public static SceneNode mkParticle(SceneManager sceneMgr, string particle) {
            ParticleSystem p = sceneMgr.CreateParticleSystem("particle" + Guid.NewGuid().ToString(), particle);
            SceneNode particleNode = sceneMgr.RootSceneNode.CreateChildSceneNode("Particle");
            particleNode.AttachObject(p);

            return particleNode;
        }

        public static SceneNode mkParticle(SceneManager sceneMgr, Vector3 position, string particle) {
            SceneNode p = mkParticle(sceneMgr, particle);
            p.SetPosition(position.x, position.y, position.z);
            return p;
        }
    }
}
