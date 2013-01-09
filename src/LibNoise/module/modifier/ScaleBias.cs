using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LibNoise;
using LibNoise.Modules;
using LibNoise.Modules.Source;

namespace LibNoise.Modules.Modifier
{
    public class ScaleBias : Module {
	    /// Default bias for the noise::module::ScaleBias noise module.
	    public const double DEFAULT_BIAS = 0.0;

	    /// Default scale for the noise::module::ScaleBias noise module.
	    public const double DEFAULT_SCALE = 1.0;

	    /// Bias to apply to the scaled output value from the source module.
	    double bias = DEFAULT_BIAS;

	    /// Scaling factor to apply to the output value from the source
	    /// module.
	    double scale = DEFAULT_SCALE;

	    public ScaleBias() : base(1) {}

	    public double getBias() {
		    return bias;
	    }

	    public void setBias(double bias) {
		    this.bias = bias;
	    }

	    public double getScale() {
		    return scale;
	    }

	    public void setScale(double scale) {
		    this.scale = scale;
	    }

	    public override int GetSourceModuleCount() {
		    return 1;
	    }

	    public override double GetValue(double x, double y, double z) {
		    if (SourceModule[0] == null)
			    throw new NoModuleException();

		    return SourceModule[0].GetValue(x, y, z) * scale + bias;
	    }

    }
}