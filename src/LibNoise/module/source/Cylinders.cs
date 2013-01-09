using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LibNoise;
using LibNoise.Modules;

namespace LibNoise.Modules.Source
{
    public class Cylinders : Module {
	    public const double DEFAULT_CYLINDERS_FREQUENCY = 1.0;

	    double frequency = DEFAULT_CYLINDERS_FREQUENCY;

	    public Cylinders() :base(0) {}

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
            double z1 = z;
            double x1 = x;
            x1 *= frequency;
            z1 *= frequency;

            double distFromCenter = MathHelper.sqrt(x1 * x1 + z1 * z1);
            double distFromSmallerSphere = distFromCenter - MathHelper.floor(distFromCenter);
            double distFromLargerSphere = 1.0 - distFromSmallerSphere;
            double nearestDist = Utils.GetMin(distFromSmallerSphere, distFromLargerSphere);
            return 1.0 - (nearestDist * 4.0); // Puts it in the -1.0 to +1.0 range.

        }

    }
}
