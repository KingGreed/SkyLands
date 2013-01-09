using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LibNoise;

namespace LibNoise.Modules.Modifier
{
    public class Invert : Module {

	    public Invert() : base(1) {}

	    public override int GetSourceModuleCount() {
		    return 1;
	    }

	    public override double GetValue(double x, double y, double z) {
		    if (SourceModule[0] == null)
			    throw new NoModuleException();
		    return -(SourceModule[0].GetValue(x, y, z));
	    }

    }
}