using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LibNoise;
using LibNoise.Modules;
using LibNoise.Modules.Source;

namespace LibNoise.Modules.Modifier
{
    public class RotatePoint : Module {

	    /// Default @a x rotation angle for the noise::module::RotatePoint noise
	    /// module.
	    public const double DEFAULT_ROTATE_X = 0.0;

	    /// Default @a y rotation angle for the noise::module::RotatePoint noise
	    /// module.
	    public const double DEFAULT_ROTATE_Y = 0.0;

	    /// Default @a z rotation angle for the noise::module::RotatePoint noise
	    /// module.
	    public const double DEFAULT_ROTATE_Z = 0.0;

	    double xAngle = DEFAULT_ROTATE_X;
	    double yAngle = DEFAULT_ROTATE_Y;
	    double zAngle = DEFAULT_ROTATE_Z;

	    /// An entry within the 3x3 rotation matrix used for rotating the
	    /// input value.
	    double x1Matrix;

	    /// An entry within the 3x3 rotation matrix used for rotating the
	    /// input value.
	    double x2Matrix;

	    /// An entry within the 3x3 rotation matrix used for rotating the
	    /// input value.
	    double x3Matrix;

	    /// An entry within the 3x3 rotation matrix used for rotating the
	    /// input value.
	    double y1Matrix;

	    /// An entry within the 3x3 rotation matrix used for rotating the
	    /// input value.
	    double y2Matrix;

	    /// An entry within the 3x3 rotation matrix used for rotating the
	    /// input value.
	    double y3Matrix;

	    /// An entry within the 3x3 rotation matrix used for rotating the
	    /// input value.
	    double z1Matrix;

	    /// An entry within the 3x3 rotation matrix used for rotating the
	    /// input value.
	    double z2Matrix;

	    /// An entry within the 3x3 rotation matrix used for rotating the
	    /// input value.
	    double z3Matrix;

	    public RotatePoint() : base(1) {
		    setAngles(DEFAULT_ROTATE_X, DEFAULT_ROTATE_Y, DEFAULT_ROTATE_Z);
	    }

	    public void setAngles(double x, double y, double z) {
		    double xCos, yCos, zCos, xSin, ySin, zSin;
		    xCos = MathHelper.cos(x * Utils.DEG_TO_RAD);
		    yCos = MathHelper.cos(y * Utils.DEG_TO_RAD);
		    zCos = MathHelper.cos(z * Utils.DEG_TO_RAD);
		    xSin = MathHelper.sin(x * Utils.DEG_TO_RAD);
		    ySin = MathHelper.sin(y * Utils.DEG_TO_RAD);
		    zSin = MathHelper.sin(z * Utils.DEG_TO_RAD);

		    x1Matrix = ySin * xSin * zSin + yCos * zCos;
		    y1Matrix = xCos * zSin;
		    z1Matrix = ySin * zCos - yCos * xSin * zSin;
		    x2Matrix = ySin * xSin * zCos - yCos * zSin;
		    y2Matrix = xCos * zCos;
		    z2Matrix = -yCos * xSin * zCos - ySin * zSin;
		    x3Matrix = -ySin * xCos;
		    y3Matrix = xSin;
		    z3Matrix = yCos * xCos;

		    this.xAngle = x;
		    this.yAngle = y;
		    this.zAngle = z;

	    }

	    public double getxAngle() {
		    return xAngle;
	    }

	    public void setxAngle(double xAngle) {
		    setAngles(xAngle, yAngle, zAngle);
	    }

	    public double getyAngle() {
		    return yAngle;
	    }

	    public void setyAngle(double yAngle) {
		    setAngles(xAngle, yAngle, zAngle);
	    }

	    public double getzAngle() {
		    return zAngle;
	    }

	    public void setzAngle(double zAngle) {
		    setAngles(xAngle, yAngle, zAngle);
	    }

	    public override int GetSourceModuleCount() {
		    return 1;
	    }

	    public override double GetValue(double x, double y, double z) {
		    if (SourceModule[0] == null)
			    throw new NoModuleException();

		    double nx = (x1Matrix * x) + (y1Matrix * y) + (z1Matrix * z);
		    double ny = (x2Matrix * x) + (y2Matrix * y) + (z2Matrix * z);
		    double nz = (x3Matrix * x) + (y3Matrix * y) + (z3Matrix * z);
		    return SourceModule[0].GetValue(nx, ny, nz);

	    }

    }
}