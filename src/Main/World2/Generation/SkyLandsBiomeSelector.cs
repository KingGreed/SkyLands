using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LibNoise;
using LibNoise.Modules;
using Game.Generation.Biomes;

namespace Game.Generation
{
    class SkyLandsBiomeSelector : BiomeSelector{
        private const float DEFAULT_SCALE = 2.5f;
        //
        private const float CONTINENTS_THRESHOLD = -0.05f;
        private const float LAND_THRESHOLD = 0;
        private const float SMALL_MOUNTAIN_THRESHOLD = 0.675f;
        private const float MOUNTAIN_THRESHOLD = 0.51f;
        private const float RIVER_THRESHOLD = 0.89f;
        private const float SWAMP_THRESHOLD = 6;
        private const float PLAINS_THRESHOLD = 5;
        private const float FOREST_THRESHOLD = 4;
        private const float JUNGLE_THRESHOLD = 3;
        private const float DESERT_THRESHOLD = 2;
        private const float TAIGA_THRESHOLD = 1;
        private const float TUNDRA_THRESHOLD = 0;
        private const float FOREST_HILLS_THRESHOLD = 0.5f;
        private const float DESERT_HILLS_THRESHOLD = 0.5f;
        private const float JUNGLE_HILLS_THRESHOLD = 0.5f;
        //
        private MountainsBiome mountain;

        public SkyLandsBiomeSelector() : this(DEFAULT_SCALE) { }

        public SkyLandsBiomeSelector(float scale) {
            this.mountain = new MountainsBiome(0);
        }

        public override Biome pickBiome(int x, int y, int z, int intSeed) {
            return this.mountain;
        }
    }
}
