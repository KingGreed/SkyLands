using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LibNoise;
using LibNoise.Modules;
using LibNoise.Modules.Source;
using LibNoise.Modules.Modifier;

namespace LibNoise.Modules.Combiner
{
    public class Multiply : Module {

	    public Multiply() : base(2) {}

	    public override int GetSourceModuleCount() {
		    return 2;
	    }

	    public override double GetValue(double x, double y, double z) {
		    if (SourceModule[0] == null)
			    throw new NoModuleException();
		    if (SourceModule[1] == null)
			    throw new NoModuleException();

		    return SourceModule[0].GetValue(x, y, z) * SourceModule[1].GetValue(x, y, z);
	    }

    }
}