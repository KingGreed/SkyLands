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
    public class Displace : Module {

	    public Displace() : base(4) {}

	    public override int GetSourceModuleCount() {
		    return 4;
	    }

	    public Module GetXDisplaceModule() {
		    if (SourceModule == null || SourceModule[1] == null) {
			    throw new NoModuleException();
		    }
		    return SourceModule[1];
	    }

	    public Module GetYDisplaceModule() {
		    if (SourceModule == null || SourceModule[2] == null) {
			    throw new NoModuleException();
		    }
		    return SourceModule[2];
	    }

	    public Module GetZDisplaceModule() {
		    if (SourceModule == null || SourceModule[3] == null) {
			    throw new NoModuleException();
		    }
		    return SourceModule[3];
	    }

	    public void SetXDisplaceModule(Module x) {
		    if (x == null)
			    throw new ArgumentException("x cannot be null");
		    SourceModule[1] = x;
	    }

	    public void SetYDisplaceModule(Module y) {
		    if (y == null)
			    throw new ArgumentException("y cannot be null");
		    SourceModule[2] = y;
	    }

	    public void SetZDisplaceModule(Module z) {
		    if (z == null)
			    throw new ArgumentException("z cannot be null");
		    SourceModule[3] = z;
	    }

	    public void SetDisplaceModules(Module x, Module y, Module z) {
		    SetXDisplaceModule(x);
		    SetYDisplaceModule(y);
		    SetZDisplaceModule(z);
	    }

	    public override double GetValue(double x, double y, double z) {
		    if (SourceModule[0] == null)
			    throw new NoModuleException();
		    if (SourceModule[1] == null)
			    throw new NoModuleException();
		    if (SourceModule[2] == null)
			    throw new NoModuleException();
		    if (SourceModule[3] == null)
			    throw new NoModuleException();

		    // Get the output values from the three displacement modules.  Add each
		    // value to the corresponding coordinate in the input value.
		    double xDisplace = x + (SourceModule[1].GetValue(x, y, z));
		    double yDisplace = y + (SourceModule[2].GetValue(x, y, z));
		    double zDisplace = z + (SourceModule[3].GetValue(x, y, z));

		    // Retrieve the output value using the offsetted input value instead of
		    // the original input value.
		    return SourceModule[0].GetValue(xDisplace, yDisplace, zDisplace);

	    }

    }
}