using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Generation.Biomes
{
    class DesertBiome : GrassyBiome {
        public DesertBiome(int biomeId) : base(biomeId) { this.setMinMax(56, 80); }
        public override string ToString() { return "Mountains"; }
    }
}
