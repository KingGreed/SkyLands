using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Ent;
using API.Geo;
using API.Generic;

using LibNoise;
using LibNoise.Modules;

using Mogre;

namespace API.Generator
{

    /**
     * Utilities for World generation
     */
    public class WorldGeneratorUtils {
	    private const int  HASH_SHIFT = 19;
	    private const long HASH_SHIFT_MASK = (1L << HASH_SHIFT) - 1;

	    /**
	     * Returns the particular seed a Random should use for a position
	     *
	     * The meaning of the x, y and z coordinates can be determined by the
	     * generator.
	     *
	     * This gives consistent results for world generation.
	     *
	     * The extra seed allows multiple Randoms to be returned for the same
	     * position for use by populators and different stages of generation.
	     *
	     * @param world the World
	     * @param x the x coordinate
	     * @param y the y coordinate
	     * @param z the z coordinate
	     * @param extraSeed the extra seed value
	     */
	    public static long getSeed(World world, int x, int y, int z, int extraSeed) {
		    return getSeed(world.getSeed(), x, y, z, extraSeed);
	    }

	    /**
	     * Returns the particular seed a Random should use for a position
	     *
	     * The meaning of the x, y and z coordinates can be determined by the
	     * generator.
	     *
	     * This gives consistent results for world generation.
	     *
	     * The extra seed allows multiple Randoms to be returned for the same
	     * position for use by populators and different stages of generation.
	     *
	     * @param seed the original seed
	     * @param x the x coordinate
	     * @param y the y coordinate
	     * @param z the z coordinate
	     * @param extraSeed the extra seed value
	     */
	    public static long getSeed(long seed, int x, int y, int z, int extraSeed) {
		    seed += (seed << HASH_SHIFT) + (seed >> 64 - HASH_SHIFT & HASH_SHIFT_MASK) + extraSeed;
		    seed += (seed << HASH_SHIFT) + (seed >> 64 - HASH_SHIFT & HASH_SHIFT_MASK) + x;
		    seed += (seed << HASH_SHIFT) + (seed >> 64 - HASH_SHIFT & HASH_SHIFT_MASK) + y;
		    seed += (seed << HASH_SHIFT) + (seed >> 64 - HASH_SHIFT & HASH_SHIFT_MASK) + z;

		    return seed;
	    }

	    /**
	     * Gets a pre-seeded random for a particular position.
	     *
	     * The meaning of the x, y and z coordinates can be determined by the
	     * generator.
	     *
	     * The extra seed allows multiple Randoms to be returned for the same
	     * position for use by populators and different stages of generation.
	     *
	     * @param world the World
	     * @param x the x coordinate
	     * @param y the y coordinate
	     * @param z the z coordinate
	     * @param extraSeed the extra seed value
	     * @return the random
	     */
	    public static Random getRandom(World world, int x, int y, int z, int extraSeed) {
		    return getRandom(world.getSeed(), x, y, z, extraSeed);
	    }

	    /**
	     * Gets a pre-seeded random for a particular position.
	     *
	     * The meaning of the x, y and z coordinates can be determined by the
	     * generator.
	     *
	     * The extra seed allows multiple Randoms to be returned for the same
	     * position for use by populators and different stages of generation.
	     *
	     * @param seed the original seed
	     * @param x the x coordinate
	     * @param y the y coordinate
	     * @param z the z coordinate
	     * @param extraSeed the extra seed value
	     * @return the random
	     */
	    public static Random getRandom(long seed, int x, int y, int z, int extraSeed) {
		    Random rng = new Random((int)getSeed(seed, x, y, z, extraSeed));
		    return rng;
	    }

	    /**
	     * Gets an array of Randoms for a cuboid of positions.
	     *
	     * The meaning of the x, y and z coordinates can be determined by the
	     * generator.
	     *
	     * The indexes for the array are arranged as array[x, y, z].
	     *
	     * array[sizeX][sizeY][sizeZ] is Random corresponding to the the given
	     * central position.
	     *
	     * The array is a 3d array of size (2 * sizeX + 1, 2 * sizeY + 1, 2 * sizeZ
	     * + 1)
	     *
	     * @param array The array of randoms to seed
	     * @param world the World containing the Chunk
	     * @param x the x coordinate for the centre of the array (blockX << 4)
	     * @param y the y coordinate for the centre of the array (blockY << 4)
	     * @param z the z coordinate for the centre of the array (blockZ << 4)
	     * @param sizeX the X distance to the edge
	     * @param sizeY the Y distance to the edge
	     * @param sizeZ the Z distance to the edge
	     * @param extraSeed the extra seed value for the randoms
	     * @return the random array
	     */
	    public Random[, ,] seedRandomArray(Random[, ,] array, World world, int x, int y, int z, int sizeX, int sizeY, int sizeZ, int extraSeed) {
		    int lz = 2 * sizeX + 1;
		    int ly = 2 * sizeY + 1;
		    int lx = 2 * sizeZ + 1;

		    for (int cx = 0; cx < lx; cx++) {
			    for (int cy = 0; cy < ly; cy++) {
				    for (int cz = 0; cz < lz; cz++) {
					    array[cx, cy, cz] = new Random((int)getSeed(world, x + cx - sizeX, y + cy - sizeY, z + cz - sizeZ, extraSeed));
				    }
			    }
		    }

		    return array;
	    }

	    /**
	     * Generates a 1D noise map using reduced sampling and linear interpolation
	     *
	     * @param noiseGenerator The noise generator module
	     * @param xSize The size of the 1D map
	     * @param samplingRate The sampling rate to use. xSize % samplingRate must
	     * return 0.
	     * @param x The x coord
	     * @param y The y coord
	     * @param z The z coord
	     * @throws IllegalArgumentException if the noise generator is null, the
	     * samplign rate is zero, or xSize % samplingRate doesn't return 0
	     * @return The noise map
	     */
	    public static double[] fastNoise(Module noiseGenerator, int xSize, int samplingRate, int x, int y, int z) {
		    if (noiseGenerator == null) {
			    throw new ArgumentException("noiseGenerator cannot be null");
		    }
		    if (samplingRate == 0) {
			    throw new ArgumentException("samplingRate cannot be 0");
		    }
		    if (xSize % samplingRate != 0) {
			    throw new ArgumentException("xSize % samplingRate must return 0");
		    }
		    double[] noiseArray = new double[xSize + 1];
		    for (int xx = 0; xx <= xSize; xx += samplingRate) {
			    noiseArray[xx] = noiseGenerator.GetValue(xx + x, y, z);
		    }
		    for (int xx = 0; xx < xSize; xx++) {
			    if (xx % samplingRate != 0) {
				    int nx = (xx / samplingRate) * samplingRate;
				    noiseArray[xx] = MathHelper.lerp(xx, nx, nx + samplingRate,
						    noiseArray[nx], noiseArray[nx + samplingRate]);
			    }
		    }
		    return noiseArray;
	    }

	    /**
	     * Generates a 2D noise map using reduced sampling and bilinear
	     * interpolation.
	     *
	     * @param noiseGenerator The noise generator module
	     * @param xSize The x size of the 2D map
	     * @param zSize The z size of the 2D map
	     * @param samplingRate The sampling rate to use. xSize % samplingRate and
	     * zSize % samplingRate must return 0.
	     * @param x The x coord
	     * @param y The y coord
	     * @param z The z coord
	     * @throws IllegalArgumentException if the noise generator is null, the
	     * samplign rate is zero, or xSize % samplingRate or zSize % samplingRate
	     * doesn't return 0
	     * @return The noise map
	     */
	    public static double[,] fastNoise(Module noiseGenerator, int xSize, int zSize, int samplingRate, int x, int y, int z) {
		    if (noiseGenerator == null) {
			    throw new ArgumentException("noiseGenerator cannot be null");
		    }
		    if (samplingRate == 0) {
			    throw new ArgumentException("samplingRate cannot be 0");
		    }
		    if (xSize % samplingRate != 0) {
			    throw new ArgumentException("xSize % samplingRate must return 0");
		    }
		    if (zSize % samplingRate != 0) {
			    throw new ArgumentException("zSize % samplingRate must return 0");
		    }
		    double[, ] noiseArray = new double[xSize + 1, zSize + 1];
		    for (int xx = 0; xx <= xSize; xx += samplingRate) {
			    for (int zz = 0; zz <= zSize; zz += samplingRate) {
				    noiseArray[xx, zz] = noiseGenerator.GetValue(xx + x, y, z + zz);
			    }
		    }
		    for (int xx = 0; xx < xSize; xx++) {
			    for (int zz = 0; zz < zSize; zz++) {
				    if (xx % samplingRate != 0 || zz % samplingRate != 0) {
					    int nx = (xx / samplingRate) * samplingRate;
					    int nz = (zz / samplingRate) * samplingRate;
					    noiseArray[xx, zz] = MathHelper.biLerp(xx, zz, noiseArray[nx, nz],
							    noiseArray[nx, nz + samplingRate], noiseArray[nx + samplingRate, nz],
							    noiseArray[nx + samplingRate, nz + samplingRate], nx, nx + samplingRate,
							    nz, nz + samplingRate);
				    }
			    }
		    }
		    return noiseArray;
	    }

	    /**
	     * Generates a 3D noise map using reduced sampling and trilinear
	     * interpolation.
	     *
	     * @param noiseGenerator The noise generator module
	     * @param xSize The x size of the 3D map
	     * @param ySize The y size of the 3D map
	     * @param zSize The z size of the 3D map
	     * @param samplingRate The sampling rate to use. xSize % samplingRate, ySize
	     * % samplingRate and zSize % samplingRate must return 0.
	     * @param x The x coord
	     * @param y The y coord
	     * @param z The z coord
	     * @throws IllegalArgumentException if the noise generator is null, the
	     * samplign rate is zero, or xSize % samplingRate, ySize % samplingRate or
	     * zSize % samplingRate doesn't return 0
	     * @return The noise map
	     */
	    public static double[, ,] fastNoise(Module noiseGenerator, int xSize, int ySize, int zSize,
			    int samplingRate, int x, int y, int z) {
		    if (noiseGenerator == null) {
			    throw new ArgumentException("noiseGenerator cannot be null");
		    }
		    if (samplingRate == 0) {
			    throw new ArgumentException("samplingRate cannot be 0");
		    }
		    if (xSize % samplingRate != 0) {
			    throw new ArgumentException("xSize % samplingRate must return 0");
		    }
		    if (ySize % samplingRate != 0) {
			    throw new ArgumentException("ySize % samplingRate must return 0");
		    }
		    if (zSize % samplingRate != 0) {
			    throw new ArgumentException("zSize % samplingRate must return 0");
		    }
		    double[, ,] noiseArray = new double[xSize + 1, ySize + 1, zSize + 1];
		    for (int xx = 0; xx <= xSize; xx += samplingRate) {
			    for (int yy = 0; yy <= ySize; yy += samplingRate) {
				    for (int zz = 0; zz <= zSize; zz += samplingRate) {
					    noiseArray[xx, yy, zz] = noiseGenerator.GetValue(xx + x, y + yy, z + zz);
				    }
			    }
		    }
		    for (int xx = 0; xx < xSize; xx++) {
			    for (int yy = 0; yy < ySize; yy++) {
				    for (int zz = 0; zz < zSize; zz++) {
					    if (xx % samplingRate != 0 || yy % samplingRate != 0 || zz % samplingRate != 0) {
						    int nx = (xx / samplingRate) * samplingRate;
						    int ny = (yy / samplingRate) * samplingRate;
						    int nz = (zz / samplingRate) * samplingRate;
						    noiseArray[xx, yy, zz] = MathHelper.triLerp(xx, yy, zz,
								    noiseArray[nx, ny, nz], noiseArray[nx, ny + samplingRate, nz],
								    noiseArray[nx, ny, nz + samplingRate], noiseArray[nx, ny + samplingRate, nz + samplingRate],
								    noiseArray[nx + samplingRate, ny, nz], noiseArray[nx + samplingRate, ny + samplingRate, nz],
								    noiseArray[nx + samplingRate, ny, nz + samplingRate], noiseArray[nx + samplingRate, ny + samplingRate, nz + samplingRate],
								    nx, nx + samplingRate, ny, ny + samplingRate, nz, nz + samplingRate);
					    }
				    }
			    }
		    }
		    return noiseArray;
	    }
    }
}