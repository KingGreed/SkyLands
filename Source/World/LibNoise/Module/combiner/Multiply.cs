using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.LibNoise.Module
{
    class Multiply : Module {
        public Multiply() : base(2) {}

	    public override int GetSourceModuleCount() { return 2; }

	    public override double GetValue(double x, double y, double z) {
		    if (SourceModule[0] == null) { throw new Exception("No module"); }
		    if (SourceModule[1] == null) { throw new Exception("No module"); }

		    return SourceModule[0].GetValue(x, y, z) * SourceModule[1].GetValue(x, y, z);
	    }

    }
}
