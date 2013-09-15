using System;
using Mogre;

using Game.RTS;

namespace Game.World.Display {
    public static class ParticleGenerator {

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

        public static void CreateBuildingRing(SceneManager sceneMgr, Vector3 position, Vector2 size, Faction fact)
        {
            float factor = size.Length / (10 * (float)System.Math.Sqrt(2));
            ParticleSystem p = sceneMgr.CreateParticleSystem("particle" + Guid.NewGuid(), "BuildingRing");
            ParticleEmitter emitter = p.GetEmitter(0);
            float r = fact == Faction.Red ? 1 : 0;
            float b = fact == Faction.Red ? 0 : 1;
            emitter.ColourRangeStart = new ColourValue(r, 0, b);
            emitter.ColourRangeEnd = new ColourValue(r / 2, 0, b / 2);
            emitter.EmissionRate *= factor;

            SceneNode particleNode = sceneMgr.RootSceneNode.CreateChildSceneNode("Particle" + Guid.NewGuid());
            particleNode.AttachObject(p);
            particleNode.Position = position;
            particleNode.Scale(factor, 0, factor);
        }
    }
}
