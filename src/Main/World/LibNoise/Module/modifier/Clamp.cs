using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.LibNoise.Module
{
        public class Clamp : Module {

	    private double lowerBound = 0.0;
	    private double upperBound = 1.0;

	    public Clamp() : base(1) {}

	    public double       getLowerBound()                    { return lowerBound; }
	    public void         setLowerBound(double lowerBound)   { this.lowerBound = lowerBound; }
	    public double       getUpperBound()                    { return upperBound; } 
	    public void         setUpperBound(double upperBound)   { this.upperBound = upperBound; }
	    public override int GetSourceModuleCount()             { return 1; }

	    public override double GetValue(double x, double y, double z) {
		    if (SourceModule[0] == null) { throw new Exception("No module"); }

		    double value = SourceModule[0].GetValue(x, y, z);
		    if (value < lowerBound)      { return lowerBound; }
            else if (value > upperBound) { return upperBound; } 
            else                         { return value; }

	    }
    }
}
