using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Generation.Biomes
{
    class PlainBiome : GrassyBiome {
        public PlainBiome(int id) : base(id) { this.setMinMax(60, 75); }
        public override string ToString() { return "Mountains"; }
    }
}
