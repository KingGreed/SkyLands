using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LibNoise;

namespace LibNoise.Modules
{
    public class Cache : Module {
	    /// The cached output value at the cached input value.
	    double cachedValue;

	    /// Determines if a cached output value is stored in this noise
	    /// module.
	    bool isCached = false;

	    // @a x coordinate of the cached input value.
	    double xCache;

	    /// @a y coordinate of the cached input value.
	    double yCache;

	    /// @a z coordinate of the cached input value.
	    double zCache;

	    public Cache() : base(1) {}

	    public override int GetSourceModuleCount() {
		    return 1;
	    }

	    public override void SetSourceModule(int index, Module sourceModule) {
		    base.SetSourceModule(index, sourceModule);
		    isCached = false;
	    }

	    public override double GetValue(double x, double y, double z) {
		    if (SourceModule[0] == null)
			    throw new NoModuleException();

		    if (!(isCached && x == xCache && y == yCache && z == zCache)) {
			    cachedValue = SourceModule[0].GetValue(x, y, z);
			    xCache = x;
			    yCache = y;
			    zCache = z;
		    }
		    isCached = true;
		    return cachedValue;
	    }

    }
}