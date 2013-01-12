using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Material = API.Generic.Material;

namespace Game.Generation.Biomes
{
    public abstract class GrassyBiome : Biome {
	    public GrassyBiome(int biomeId) : base(biomeId) { this.addTopCover(Material.DIRT, Material.GRASS); }
	}
}
