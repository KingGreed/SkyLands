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
            this.mGroundCover.Add(new string[] { "Grass" });
            this.mGroundCover.Add(new string[] { "Dirt" });
            this.mGroundCover.Add(new string[] { "Dirt" });

            this.setMinMax(32.5, 256);
        }


    }
}
