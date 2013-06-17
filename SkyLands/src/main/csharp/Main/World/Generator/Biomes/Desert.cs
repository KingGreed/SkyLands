using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generator;

using Game.World.Generator.Decorators;

namespace Game.World.Generator.Biomes
{
    class Desert : Biome
    {
        public Desert() : base((byte) 1, new CactusDecorator(), new Pyramid(), new Tower(), new Spawner(), new KingOfTheHill()) {
            this.mGroundCover.Add(new string[] { "Sand", "Yellow Sand", "Red Sand" });
            this.mGroundCover.Add(new string[] { "Sand", "Yellow Sand", "Red Sand" });
            this.mGroundCover.Add(new string[] { "Sand", "Yellow Sand", "Red Sand" });
 
            this.setMinMax(63, 74);
        }


    }
}
