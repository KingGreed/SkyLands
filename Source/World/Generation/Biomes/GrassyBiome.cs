using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Game.Material;

namespace Game.Generation.Biomes
{
    public abstract class GrassyBiome : Biome {
	    public GrassyBiome(int biomeId) : base(biomeId) { this.addTopCover(Materials.DIRT, Materials.GRASS); }
	}
}
