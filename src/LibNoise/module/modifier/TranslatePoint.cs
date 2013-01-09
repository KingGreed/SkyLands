using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LibNoise;
using LibNoise.Modules;
using LibNoise.Modules.Source;

namespace LibNoise.Modules.Modifier
{
    public class TranslatePoint : Module {
	    /// Default translation factor applied to the @a x coordinate for the
	    /// noise::module::TranslatePoint noise module.
	    public const double DEFAULT_TRANSLATE_POINT_X = 0.0;

	    /// Default translation factor applied to the @a y coordinate for the
	    /// noise::module::TranslatePoint noise module.
	    public const double DEFAULT_TRANSLATE_POINT_Y = 0.0;

	    /// Default translation factor applied to the @a z coordinate for the
	    /// noise::module::TranslatePoint noise module.
	    public const double DEFAULT_TRANSLATE_POINT_Z = 0.0;

	    /// Translation amount applied to the @a x coordinate of the input
	    /// value.
	    double xTranslation = DEFAULT_TRANSLATE_POINT_X;

	    /// Translation amount applied to the @a y coordinate of the input
	    /// value.
	    double yTranslation = DEFAULT_TRANSLATE_POINT_Y;

	    /// Translation amount applied to the @a z coordinate of the input
	    /// value.
	    double zTranslation = DEFAULT_TRANSLATE_POINT_Z;

	    public TranslatePoint() : base(1) {}

	    public double getXTranslation() {
		    return xTranslation;
	    }

	    public void setXTranslation(double xTranslation) {
		    this.xTranslation = xTranslation;
	    }

	    public double getYTranslation() {
		    return yTranslation;
	    }

	    public void setYTranslation(double yTranslation) {
		    this.yTranslation = yTranslation;
	    }

	    public double getZTranslation() {
		    return zTranslation;
	    }

	    public void setZTranslation(double zTranslation) {
		    this.zTranslation = zTranslation;
	    }

	    public void setTranslations(double x, double y, double z) {
		    setXTranslation(x);
		    setYTranslation(y);
		    setZTranslation(z);
	    }

	    public override int GetSourceModuleCount() {
		    return 1;
	    }

	    public override double GetValue(double x, double y, double z) {
		    if (SourceModule[0] == null)
			    throw new NoModuleException();

		    return SourceModule[0].GetValue(x + xTranslation, y + yTranslation, z + zTranslation);
	    }

    }
}