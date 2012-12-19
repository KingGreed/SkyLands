using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Game.LibNoise;
using Game.LibNoise.Module;
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
        private Perlin continentsBase = new Perlin();
        private Turbulence continents = new Turbulence();
        //
        private Voronoi landBase = new Voronoi();
        private Turbulence land = new Turbulence();
        //
        private Turbulence rivers = new Turbulence();
        //
        private Perlin detailBase = new Perlin();
        private Turbulence detail = new Turbulence();
        private MountainsBiome mountain;

        public SkyLandsBiomeSelector() : this(DEFAULT_SCALE) { }

        public SkyLandsBiomeSelector(float scale) {
            continentsBase.setFrequency(0.007 / scale);
            continentsBase.setOctaveCount(1);

            continents.SetSourceModule(0, continentsBase);
            continents.setFrequency(0.02);
            continents.setPower(20);
            continents.setRoughness(1);
            //
            landBase.setFrequency(0.007 / scale);
            landBase.setDisplacement(1);

            land.SetSourceModule(0, landBase);
            land.setFrequency(0.004);
            land.setPower(70);
            //
            Cylinders riversNoise = new Cylinders();
            riversNoise.setFrequency(0.0025);

            rivers.SetSourceModule(0, riversNoise);
            rivers.setFrequency(0.0085);
            rivers.setRoughness(3);
            rivers.setPower(100);
            //
            detailBase.setFrequency(0.01 / scale);
            detailBase.setOctaveCount(1);

            detail.SetSourceModule(0, detailBase);
            detail.setFrequency(0.03);
            detail.setPower(20);
            detail.setRoughness(1);

            this.mountain = new MountainsBiome(0);
        }

        public override Biome pickBiome(int x, int y, int z, int intSeed) {
            return this.mountain;
            continents.setSeed(intSeed * 2);
            landBase.setSeed(intSeed * 3);
            land.setSeed(intSeed * 5);
            rivers.setSeed(intSeed * 7);
            detailBase.setSeed(intSeed * 11);
            detail.setSeed(intSeed * 13);

            float continentValue = (float) continents.GetValue(x, 0, z);
            if (continentValue > CONTINENTS_THRESHOLD) {
                if (continentValue > LAND_THRESHOLD) {
                    if (continentValue > SMALL_MOUNTAIN_THRESHOLD) {
                        if (rivers.GetValue(x, 0, z) > RIVER_THRESHOLD) {
                            //return RIVER;
                        }
                        if (continentValue < MOUNTAIN_THRESHOLD) {
                            return new MountainsBiome(0);
                        }
                        //return SMALL_MOUNTAINS;
                    }
                    float landValue = (float) land.GetValue(x, 0, z) * 3.5f + 3.5f;
                    /*if (landValue > SWAMP_THRESHOLD) {
                        if (rivers.GetValue(x, 0, z) > RIVER_THRESHOLD) {
                            return RIVER;
                        }
                        return SWAMP;
                    }*/
                    if (landValue > PLAINS_THRESHOLD) {
                        if (rivers.GetValue(x, 0, z) > RIVER_THRESHOLD) {
                            //return RIVER;
                        }
                        return new PlainBiome(1);
                    }
                    float hillValue = (float) detail.GetValue(x, 500, z);
                    /*if (landValue > FOREST_THRESHOLD) {
                        if (rivers.GetValue(x, 0, z) > RIVER_THRESHOLD) {
                            //return RIVER;
                        }
                        if (hillValue > FOREST_HILLS_THRESHOLD) {
                            //return FOREST_HILLS;
                        }
                        //return FOREST;
                    }
                    if (landValue > JUNGLE_THRESHOLD) {
                        if (rivers.GetValue(x, 0, z) > RIVER_THRESHOLD) {
                            return RIVER;
                        }
                        if (hillValue > JUNGLE_HILLS_THRESHOLD) {
                            return JUNGLE_HILLS;
                        }
                        return JUNGLE;
                    }*/
                    if (landValue > DESERT_THRESHOLD) {
                        if (rivers.GetValue(x, 0, z) > RIVER_THRESHOLD) {
                            //return RIVER;
                        }
                        if (hillValue > DESERT_HILLS_THRESHOLD) {
                            //return DESERT_HILLS;
                        }
                        return new DesertBiome(1);
                    }
                }
                /*if (rivers.GetValue(x, 0, z) > RIVER_THRESHOLD) {
                    return RIVER;
                }
                return BEACH;*/
            }
            return new PlainBiome(1);
        }
    }
}
