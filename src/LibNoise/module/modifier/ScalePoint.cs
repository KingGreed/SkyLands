using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LibNoise;
using LibNoise.Modules;
using LibNoise.Modules.Source;

namespace LibNoise.Modules.Modifier
{
    public class ScalePoint : Module {

	    /// Default scaling factor applied to the @a x coordinate for the
	    /// noise::module::ScalePoint noise module.
	    public const double DEFAULT_SCALE_POINT_X = 1.0;

	    /// Default scaling factor applied to the @a y coordinate for the
	    /// noise::module::ScalePoint noise module.
	    public const double DEFAULT_SCALE_POINT_Y = 1.0;

	    /// Default scaling factor applied to the @a z coordinate for the
	    /// noise::module::ScalePoint noise module.
	    public const double DEFAULT_SCALE_POINT_Z = 1.0;

	    /// Scaling factor applied to the @a x coordinate of the input value.
	    double xScale = DEFAULT_SCALE_POINT_X;

	    /// Scaling factor applied to the @a y coordinate of the input value.
	    double yScale = DEFAULT_SCALE_POINT_Y;

	    /// Scaling factor applied to the @a z coordinate of the input value.
	    double zScale = DEFAULT_SCALE_POINT_Z;

	    public ScalePoint() : base(1) { }

	    public double getxScale() {
		    return xScale;
	    }

	    public void setxScale(double xScale) {
		    this.xScale = xScale;
	    }

	    public double getyScale() {
		    return yScale;
	    }

	    public void setyScale(double yScale) {
		    this.yScale = yScale;
	    }

	    public double getzScale() {
		    return zScale;
	    }

	    public void setzScale(double zScale) {
		    this.zScale = zScale;
	    }

	    public override int GetSourceModuleCount() {
		    return 1;
	    }

	    public override double GetValue(double x, double y, double z) {
		    if (SourceModule[0] == null)
			    throw new NoModuleException();

		    return SourceModule[0].GetValue(x * xScale, y * yScale, z * zScale);
	    }

    }
}