using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generator;
using Game.World.Generator.Decorators;

namespace Game.World.Generator.Biomes
{
    class Mountains : Biome
    {
        public Mountains() : base((byte) 2, new Pine(), new Portals()) {
            this.mGroundCover.Add(new string[] { "Grass" });
            this.mGroundCover.Add(new string[] { "Dirt" });
            this.mGroundCover.Add(new string[] { "Dirt" });

            this.setMinMax(32.5, 256);
        }
    }
}
