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
        public override void onBlockEnter(API.Ent.Entity e) {
            Island current = e.getIsland();
            API.Geo.World w = current.mWorld;
            Vector3 position = current.getPosition();
            Biome next = new Hills();

            if(current.getBiome() is Hills)          { next = new Desert();    }
            else if(current.getBiome() is Desert)    { next = new Mountains(); }
            else if(current.getBiome() is Mountains) { next = new Plains();    }

            current.unload(true);
            Island nextIsland = new RandomIsland(w.getAScenNode(), new Vector2(11, 11), next, w);

            ((GameState)w.getStateMgr().GameState).CreateCharacterMgr();


            w.setSafeSpawnPoint();
            CharacterInfo playerInfo = new CharacterInfo("Sinbad", true);
            playerInfo.SpawnPoint = w.getSpawnPoint();
            ((GameState)w.getStateMgr().GameState).getCharacMgr().AddCharacter(playerInfo);
            
        }
    }
}
