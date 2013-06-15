using API.Generator;
using Mogre;

using API.Geo.Cuboid;

using Game.World.Generator.Biomes;
using Game.World.Generator;
using Game.States;

namespace Game.World.Blocks {
    abstract class CrystalGate : VanillaBlock {
        public override void onBlockEnter(API.Ent.Entity e, Vector3 position) {
            Island current = e.getIsland();
            Biome nextBiome;
            API.Geo.World w = current.mWorld;

            if      (current.getBiome() is Hills)       { nextBiome = new Desert();    }
            else if (current.getBiome() is Desert)      { nextBiome = new Mountains(); }
            else /* (current.getBiome() is Mountains)*/ { nextBiome = new Plains(); }

            current.unload(false);
            w.setIsland(new RandomIsland(w.getSceneMgr().RootSceneNode.CreateChildSceneNode(Vector3.ZERO), new Vector2(current.getSize().x, current.getSize().z), nextBiome, w));
            w.getIsland().display();
            w.setSafeSpawnPoint();
            e.teleport(w.getSpawnPoint());
        }
    }
}
