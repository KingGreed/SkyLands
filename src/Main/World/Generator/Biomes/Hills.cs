using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generator;

namespace Game.World.Generator.Biomes
{
    class Hills : Biome
    {
        public Hills() : base((byte) 3) {
            this.mGroundCover.Add("Grass");
            this.mGroundCover.Add("Dirt");
            this.mGroundCover.Add("Dirt");

            this.setMinMax(53, 90);
        }


    }
}
