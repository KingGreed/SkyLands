using Mogre;

using API.Geo.Cuboid;

using Game.World.Generator.Biomes;
using Game.States;

namespace Game.World.Blocks {
    abstract class CrystalGate : VanillaBlock {
        public override void onBlockEnter(API.Ent.Entity e, Vector3 position) {
            Island current = e.getIsland();
            API.Geo.World w = current.mWorld;
            TypeWorld next = TypeWorld.Hills;

            if      (current.getBiome() is Hills)     { next = TypeWorld.Desert;    }
            else if (current.getBiome() is Desert)    { next = TypeWorld.Mountain;  }
            else if (current.getBiome() is Mountains) { next = TypeWorld.Plains;    }

            StateManager s = w.getStateMgr();
            s.RequestStatePop();

            s.GameInfo = new GameInfo { Type = next };
            s.RequestStatePush(typeof(GameState));
        }
    }
}
