using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LibNoise;

namespace LibNoise.Modules.Source
{
    public class Spheres : Module {
	    /// Default frequency value for the noise::module::Spheres noise module.
	    public static double DEFAULT_SPHERES_FREQUENCY = 1.0;

	    /// Frequency of the concentric spheres.
	    double frequency = DEFAULT_SPHERES_FREQUENCY;

	    public Spheres() : base(0) {}

	    public double getFrequency() {
		    return frequency;
	    }

	    public void setFrequency(double frequency) {
		    this.frequency = frequency;
	    }

	    public override int GetSourceModuleCount() {
		    return 0;
	    }

        public override double GetValue(double x, double y, double z) {
            double x1 = x;
            double y1 = y;
            double z1 = z;
            x1 *= frequency;
            y1 *= frequency;
            z1 *= frequency;

            double distFromCenter = MathHelper.sqrt(x1 * x1 + y1 * y1 + z1 * z1);
            double distFromSmallerSphere = distFromCenter - MathHelper.floor(distFromCenter);
            double distFromLargerSphere = 1.0 - distFromSmallerSphere;
            double nearestDist = Utils.GetMin(distFromSmallerSphere, distFromLargerSphere);
            return 1.0 - (nearestDist * 4.0); // Puts it in the -1.0 to +1.0 range.

        }

    }
}