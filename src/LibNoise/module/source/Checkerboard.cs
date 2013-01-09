using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LibNoise;
using LibNoise.Modules;

namespace LibNoise.Modules.Source
{
    public class Checkerboard : Module {

	    public Checkerboard() : base(0){}

	    public override int GetSourceModuleCount() {

		    return 0;
	    }

	    public override double GetValue(double x, double y, double z) {
		    int ix = (int) (MathHelper.floor(Utils.MakeInt32Range(x)));
		    int iy = (int) (MathHelper.floor(Utils.MakeInt32Range(y)));
		    int iz = (int) (MathHelper.floor(Utils.MakeInt32Range(z)));
		    return ((ix & 1 ^ iy & 1 ^ iz & 1) != 0) ? -1.0 : 1.0;
	    }

    }
}