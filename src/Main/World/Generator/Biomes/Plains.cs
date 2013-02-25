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
        public Plains() : base((byte) 1, new TreeDecorator()) {
            this.mGroundCover.Add("Grass");
            this.mGroundCover.Add("Dirt");
            this.mGroundCover.Add("Dirt");

            this.setMinMax(71, 90);
        }


    }
}
