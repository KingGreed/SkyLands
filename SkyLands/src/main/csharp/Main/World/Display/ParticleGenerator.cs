using System;
using Mogre;

namespace Game.World.Display {
    class ParticleGenerator {

        public static SceneNode mkParticle(SceneManager sceneMgr, string particle, SceneNode parentNode = null) {
            if (parentNode == null) {
                parentNode = sceneMgr.RootSceneNode;
            }
            ParticleSystem p = sceneMgr.CreateParticleSystem("particle" + Guid.NewGuid().ToString(), particle);
            SceneNode particleNode = parentNode.CreateChildSceneNode("Particle" + Guid.NewGuid().ToString());
            particleNode.AttachObject(p);

            return particleNode;
        }

        public static SceneNode mkParticle(SceneManager sceneMgr, Vector3 position, string particle, SceneNode parentNode = null) {
            SceneNode p = mkParticle(sceneMgr, particle, parentNode);
            p.SetPosition(position.x, position.y, position.z);
            return p;
        }
    }
}
