using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generator;

using Game.World.Generator.Decorators;

namespace Game.World.Generator.Biomes
{
    class Plains : Biome
    {
        public Plains() : base((byte) 1, new TreeDecorator(), new DarkTower()) {
            this.mGroundCover.Add(new string[] {"Grass"});
            this.mGroundCover.Add(new string[] {"Dirt"});
            this.mGroundCover.Add(new string[] {"Dirt"});

            this.setMinMax(71, 90);
        }


    }
}
