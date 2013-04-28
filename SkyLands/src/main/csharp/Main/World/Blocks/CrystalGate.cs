using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using API.Geo.Cuboid;
using API.Generator;

using Game.World.Generator.Biomes;
using Game.World.Generator;
using Game.States;
using Game.CharacSystem;



namespace Game.World.Blocks {
    abstract class CrystalGate : VanillaBlock {
        public override void onBlockEnter(API.Ent.Entity e, Vector3 position) {
            Island current = e.getIsland();
            API.Geo.World w = current.mWorld;
            GameInfo.TypeWorld next = GameInfo.TypeWorld.Hills;

            if(current.getBiome() is Hills)          { next = GameInfo.TypeWorld.Desert;    }
            else if(current.getBiome() is Desert)    { next = GameInfo.TypeWorld.Mountain;  }
            else if(current.getBiome() is Mountains) { next = GameInfo.TypeWorld.Plains;    }

            StateManager s = w.getStateMgr();
            s.RequestStatePop();

            GameInfo g = new GameInfo();
            g.Type = next;

            s.GameInfo = new GameInfo();
            s.RequestStatePush(typeof(GameState));

            
        }
    }
}
