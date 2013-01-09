using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LibNoise;

namespace LibNoise.Modules.Modifier
{
    public class Exponent : Module {
	    public const double DEFAULT_EXPONENT = 1.0;
	    protected double exponent = DEFAULT_EXPONENT;

	    public Exponent() : base(1) {}

	    public double getExponent() {
		    return exponent;
	    }

	    public void setExponent(double exponent) {
		    this.exponent = exponent;
	    }

	    public override int GetSourceModuleCount() {
		    return 1;
	    }

	    public override double GetValue(double x, double y, double z) {
		    if (SourceModule[0] == null)
			    throw new NoModuleException();
		    double value = SourceModule[0].GetValue(x, y, z);
		    return (Math.Pow(Math.Abs((value + 1.0) / 2.0), exponent) * 2.0 - 1.0);
	    }

    }
}