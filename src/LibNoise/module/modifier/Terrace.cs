using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LibNoise;
using LibNoise.Modules;
using LibNoise.Modules.Source;


namespace LibNoise.Modules.Modifier
{
    public class Terrace : Module {
	    /// Number of control points stored in this noise module.
	    int controlPointCount = 0;

	    /// Determines if the terrace-forming curve between all control points
	    /// is inverted.
	    bool invertTerraces = false;

	    /// Array that stores the control points.
	    double[] ControlPoints = new double[0];

	    public Terrace() : base(1) {}

	    public bool isInvertTerraces() {
		    return invertTerraces;
	    }

	    public void setInvertTerraces(bool invertTerraces) {
		    this.invertTerraces = invertTerraces;
	    }

	    public int getControlPointCount() {
		    return controlPointCount;
	    }

	    public double[] getControlPoints() {
		    return ControlPoints;
	    }

	    public void AddControlPoint(double value) {
		    int insertionPos = FindInsertionPos(value);
		    InsertAtPos(insertionPos, value);
	    }

	    public void ClearAllControlPoints() {
		    ControlPoints = null;
		    controlPointCount = 0;

	    }

	    public void MakeControlPoints(int controlPointCount) {
		    if (controlPointCount < 2) {
			    throw new ArgumentException("Must have more than 2 control points");
		    }

		    ClearAllControlPoints();

		    double terraceStep = 2.0 / (controlPointCount - 1.0);
		    double curValue = -1.0;
		    for (int i = 0; i < controlPointCount; i++) {
			    AddControlPoint(curValue);
			    curValue += terraceStep;
		    }

	    }

	    protected int FindInsertionPos(double value) {
		    int insertionPos;
		    for (insertionPos = 0; insertionPos < controlPointCount; insertionPos++) {
			    if (value < ControlPoints[insertionPos]) {
				    // We found the array index in which to insert the new control point.
				    // Exit now.
				    break;
			    } else if (value == ControlPoints[insertionPos]) {
				    // Each control point is required to contain a unique value, so throw
				    // an exception.
				    throw new ArgumentException("Value must be unique");
			    }
		    }
		    return insertionPos;

	    }

	    protected void InsertAtPos(int insertionPos, double value) {
		    // Make room for the new control point at the specified position within
		    // the control point array.  The position is determined by the value of
		    // the control point; the control points must be sorted by value within
		    // that array.
		    double[] newControlPoints = new double[controlPointCount + 1];
		    for (int i = 0; i < controlPointCount; i++) {
			    if (i < insertionPos) {
				    newControlPoints[i] = ControlPoints[i];
			    } else {
				    newControlPoints[i + 1] = ControlPoints[i];
			    }
		    }

		    ControlPoints = newControlPoints;
		    ++controlPointCount;

		    // Now that we've made room for the new control point within the array,
		    // add the new control point.
		    ControlPoints[insertionPos] = value;

	    }

	    public override int GetSourceModuleCount() {
		    return 1;
	    }

	    public override double GetValue(double x, double y, double z) {
		    if (SourceModule[0] == null)
			    throw new NoModuleException();

		    // Get the output value from the source module.
		    double sourceModuleValue = SourceModule[0].GetValue(x, y, z);

		    // Find the first element in the control point array that has a value
		    // larger than the output value from the source module.
		    int indexPos;
		    for (indexPos = 0; indexPos < controlPointCount; indexPos++) {
			    if (sourceModuleValue < ControlPoints[indexPos]) {
				    break;
			    }
		    }

		    // Find the two nearest control points so that we can map their values
		    // onto a quadratic curve.
		    int index0 = Utils.ClampValue(indexPos - 1, 0, controlPointCount - 1);
		    int index1 = Utils.ClampValue(indexPos, 0, controlPointCount - 1);

		    // If some control points are missing (which occurs if the output value from
		    // the source module is greater than the largest value or less than the
		    // smallest value of the control point array), get the value of the nearest
		    // control point and exit now.
		    if (index0 == index1) {
			    return ControlPoints[index1];
		    }

		    // Compute the alpha value used for linear interpolation.
		    double value0 = ControlPoints[index0];
		    double value1 = ControlPoints[index1];
		    double alpha = (sourceModuleValue - value0) / (value1 - value0);
		    if (invertTerraces) {
			    alpha = 1.0 - alpha;
			    double temp = value0;
			    value0 = value1;
			    value1 = temp;
		    }

		    // Squaring the alpha produces the terrace effect.
		    alpha *= alpha;

		    // Now perform the linear interpolation given the alpha value.
		    return Utils.LinearInterp(value0, value1, alpha);

	    }

    }
}