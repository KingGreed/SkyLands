using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generator;

namespace Game.World.Generator.Biomes
{
    class Mountains : Biome
    {
        public Mountains() : base((byte) 2) {
            this.mGroundCover.Add("Grass");
            this.mGroundCover.Add("Dirt");
            this.mGroundCover.Add("Dirt");

            this.setMinMax(32.5, 256);
        }


    }
}
