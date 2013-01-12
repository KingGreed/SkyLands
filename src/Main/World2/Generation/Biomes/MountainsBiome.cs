using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;
using Material = API.Generic.Material;

namespace Game.Generation.Biomes
{
    class MountainsBiome : Biome
    {
        public MountainsBiome(int id) : base(id){
            this.addTopCover(Material.DIRT, Material.GRASS, Material.STONE);
            this.setMinMax(32.5f, 160);
        }

        public override string ToString() { return "Mountains"; }
    }
}
