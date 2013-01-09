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
    public class Blend : Module {

	    public Blend() : base(3) {}

	    public Module getControlModule() {
		    if (SourceModule[2] == null)
			    throw new NoModuleException();
		    return SourceModule[2];
	    }

	    public void setControlModule(Module module) {
		    if (module == null)
			    throw new ArgumentException("Control Module cannot be null");
		    SourceModule[2] = module;
	    }

	    public override int GetSourceModuleCount() {
		    return 3;
	    }

	    public override double GetValue(double x, double y, double z) {
		    if (SourceModule[0] == null)
			    throw new NoModuleException();
		    if (SourceModule[1] == null)
			    throw new NoModuleException();
		    if (SourceModule[2] == null)
			    throw new NoModuleException();

		    double v0 = SourceModule[0].GetValue(x, y, z);
		    double v1 = SourceModule[1].GetValue(x, y, z);
		    double alpha = (SourceModule[2].GetValue(x, y, z) + 1.0) / 2.0;
		    return Utils.LinearInterp(v0, v1, alpha);

	    }

    }
}