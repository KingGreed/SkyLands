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
            TypeWorld newWorld;

            if      (current.getBiome() is Hills)       { newWorld = TypeWorld.Desert; }
            else if (current.getBiome() is Desert)      { newWorld = TypeWorld.Mountain; }
            else /* (current.getBiome() is Mountains)*/ { newWorld = TypeWorld.Plains; }

            StateManager.ChangeIsland(newWorld);
        }
    }
}
