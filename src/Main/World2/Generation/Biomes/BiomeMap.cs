using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Game.Generation.Biomes
{
    public class BiomeMap {
	    private Dictionary<int, Biome> map;
	    private BiomeSelector selector;

	    public BiomeMap() {
		    map = new Dictionary<int, Biome>();
	    }

	    public Biome getBiomeRaw(int index) {
		    return map[System.Math.Abs(index) % map.Count];
	    }

	    public void setSelector(BiomeSelector selector) {
		    this.selector = selector;
		    selector.parent = this;
	    }

	    public void addBiome(Biome biome) {
		    map.Add(map.Count, biome);
	    }

	    public Biome getBiome(int x, int z, int seed) {
		    return getBiome(x, 0, z, seed);
	    }

	    public Biome getBiome(Vector3 position, int seed) {
		    return getBiome((int) position.x, (int) position.y, (int) position.z, seed);
	    }

	    public Biome getBiome(int x, int y, int z, int seed) {
		    if (selector == null) {
			    throw new Exception("Biome Selector is null and cannot set a selector");
		    }
		    return selector.pickBiome(x, y, z, seed);
	    }

	    public HashSet<Biome> getBiomes() {
		    return new HashSet<Biome>(map.Values);
	    }

	    public BiomeSelector getSelector() {
		    return selector;
	    }

    }
}
