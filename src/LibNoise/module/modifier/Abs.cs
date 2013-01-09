using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LibNoise;
using LibNoise.Modules;
using LibNoise.Modules.Source;

namespace LibNoise.Modules.Modifier
{
    public class Abs : Module {

	    public Abs() : base(1){ }

	    public override int GetSourceModuleCount() {
		    return 1;
	    }

	    public override double GetValue(double x, double y, double z) {
		    if (SourceModule == null)
			    throw new NoModuleException();
		    return Math.Abs(SourceModule[0].GetValue(x, y, z));
	    }

    }
}