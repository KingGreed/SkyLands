using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Generation.Biomes
{
    public class RiverBiome : GrassyBiome {
	    public RiverBiome(int biomeId) : base(biomeId) { setMinMax(49, 55); }
        public override string ToString() { return "Mountains"; }
    }
}
