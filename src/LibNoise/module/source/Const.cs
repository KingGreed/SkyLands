using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LibNoise;
using LibNoise.Modules;

namespace LibNoise.Modules.Source
{
    public class Const : Module {
	    double value = 0.0;

	    public Const() : base(0) {}

	    public double getValue() {
		    return value;
	    }

	    public void setValue(double value) {
		    this.value = value;
	    }

	    public override int GetSourceModuleCount() {
		    return 0;
	    }

	    public override double GetValue(double x, double y, double z) {
		    return value;
	    }

    }
}