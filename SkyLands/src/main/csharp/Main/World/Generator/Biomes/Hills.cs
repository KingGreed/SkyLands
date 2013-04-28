using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generator;
using Game.World.Generator.Decorators;


namespace Game.World.Generator.Biomes
{
    class Hills : Biome
    {
        public Hills() : base((byte) 3, new Portals()) {
            this.mGroundCover.Add(new string[] { "Grass" });
            this.mGroundCover.Add(new string[] { "Dirt" });
            this.mGroundCover.Add(new string[] { "Dirt" });

            this.setMinMax(53, 90);
        }


    }
}
