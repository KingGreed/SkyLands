using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LibNoise;
using LibNoise.Modules;

namespace LibNoise.Modules.Source
{
    public class Billow : Module {

	    public const double DEFAULT_BILLOW_FREQUENCY = 1.0;

	    public const double DEFAULT_BILLOW_LACUNARITY = 2.0;

	    public const int DEFAULT_BILLOW_OCTAVE_COUNT = 6;

	    public const double DEFAULT_BILLOW_PERSISTENCE = 0.5;

	    public const NoiseQuality DEFAULT_BILLOW_QUALITY = NoiseQuality.STANDARD;

	    public const int DEFAULT_BILLOW_SEED = 0;

	    public const int BILLOW_MAX_OCTAVE = 30;

	    protected double frequency = DEFAULT_BILLOW_FREQUENCY;
	    protected double lacunarity = DEFAULT_BILLOW_LACUNARITY;
	    protected NoiseQuality quality = DEFAULT_BILLOW_QUALITY;
	    protected double persistence = DEFAULT_BILLOW_PERSISTENCE;
	    protected int seed = DEFAULT_BILLOW_SEED;
	    protected int octaveCount = DEFAULT_BILLOW_OCTAVE_COUNT;

	    public Billow() : base(0){}

	    public int getOctaveCount() {
		    return octaveCount;
	    }

	    public void setOctaveCount(int octaveCount) {
		    if (octaveCount < 1 || octaveCount > BILLOW_MAX_OCTAVE) {
			    throw new ArgumentException("octaveCount must be between 1 and BILLOW_MAX_OCTAVE: " + BILLOW_MAX_OCTAVE);
		    }
		    this.octaveCount = octaveCount;
	    }

	    public double getFrequency() {
		    return frequency;
	    }

	    public void setFrequency(double frequency) {
		    this.frequency = frequency;
	    }

	    public double getLacunarity() {
		    return lacunarity;
	    }

	    public void setLacunarity(double lacunarity) {
		    this.lacunarity = lacunarity;
	    }

	    public NoiseQuality getQuality() {
		    return quality;
	    }

	    public void setQuality(NoiseQuality quality) {
		    this.quality = quality;
	    }

	    public double getPersistence() {
		    return persistence;
	    }

	    public void setPersistence(double persistance) {
		    this.persistence = persistance;
	    }

	    public int getSeed() {
		    return seed;
	    }

	    public void setSeed(int seed) {
		    this.seed = seed;
	    }

	    public override int GetSourceModuleCount() {
		    return 0;
	    }

        public override double GetValue(double x, double y, double z) {
            double z1 = z;
            double y1 = y;
            double x1 = x;
            double value = 0.0;
            double signal;
            double curPersistence = 1.0;
            double nx, ny, nz;
            int seed;

            x1 *= frequency;
            y1 *= frequency;
            z1 *= frequency;

            for (int curOctave = 0; curOctave < octaveCount; curOctave++) {

                // Make sure that these floating-point values have the same range as a 32-
                // bit integer so that we can pass them to the coherent-noise functions.
                nx = Utils.MakeInt32Range(x1);
                ny = Utils.MakeInt32Range(y1);
                nz = Utils.MakeInt32Range(z1);

                // Get the coherent-noise value from the input value and add it to the
                //  result.
                seed = (this.seed + curOctave);
                signal = Noise.GradientCoherentNoise3D(nx, ny, nz, seed, quality);
                signal = 2.0 * Math.Abs(signal) - 1.0;
                value += signal * curPersistence;

                // Prepare the next octave.
                x1 *= lacunarity;
                y1 *= lacunarity;
                z1 *= lacunarity;
                curPersistence *= persistence;
            }
            value += 0.5;

            return value;
        }

    }
}