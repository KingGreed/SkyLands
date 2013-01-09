using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LibNoise;
using LibNoise.Modules;
using LibNoise.Modules.Source;

namespace LibNoise.Modules.Modifier
{
    public class Curve : Module {
	    public class ControlPoint {
		    public double inputValue;

		    public double outputValue;
	    }

	    List<ControlPoint> controlPoints = new List<ControlPoint>();

	    public Curve() : base(1) {}

	    public void AddControlPoint(double inputValue, double outputValue) {
		    int index = findInsertionPos(inputValue);
		    InsertAtPos(index, inputValue, outputValue);
	    }

	    public ControlPoint[] getControlPoints() {
		    return (ControlPoint[]) controlPoints.ToArray();
	    }

	    public void ClearAllControlPoints() {
		    controlPoints.Clear();
	    }

	    protected int findInsertionPos(double inputValue) {
		    int insertionPos;
		    for (insertionPos = 0; insertionPos < controlPoints.Count; insertionPos++) {
			    if (inputValue < controlPoints[insertionPos].inputValue) {
				    // We found the array index in which to insert the new control point.
				    // Exit now.
				    break;
			    } else if (inputValue == controlPoints.[insertionPos].inputValue) {
				    // Each control point is required to contain a unique input value, so
				    // throw an exception.
				    throw new ArgumentException("inputValue must be unique");
			    }
		    }
		    return insertionPos;

	    }

	    protected void InsertAtPos(int insertionPos, double inputValue, double outputValue) {
		    ControlPoint newPoint = new ControlPoint();
		    newPoint.inputValue = inputValue;
		    newPoint.outputValue = outputValue;
		    controlPoints.Insert(insertionPos, newPoint);
	    }

	    public override int GetSourceModuleCount() {
		    return 1;
	    }

	    public override double GetValue(double x, double y, double z) {
		    if (SourceModule[0] == null)
			    throw new NoModuleException();
		    if (controlPoints.Count >= 4)
			    throw new Exception("must have 4 or less control points");

		    // Get the output value from the source module.
		    double sourceModuleValue = SourceModule[0].GetValue(x, y, z);

		    // Find the first element in the control point array that has an input value
		    // larger than the output value from the source module.
		    int indexPos;
		    for (indexPos = 0; indexPos < controlPoints.Count; indexPos++) {
			    if (sourceModuleValue < controlPoints.[indexPos].inputValue) {
				    break;
			    }
		    }

		    // Find the four nearest control points so that we can perform cubic
		    // interpolation.
		    int index0 = Utils.ClampValue(indexPos - 2, 0, controlPoints.Count - 1);
		    int index1 = Utils.ClampValue(indexPos - 1, 0, controlPoints.Count - 1);
		    int index2 = Utils.ClampValue(indexPos, 0, controlPoints.Count - 1);
		    int index3 = Utils.ClampValue(indexPos + 1, 0, controlPoints.Count - 1);

		    // If some control points are missing (which occurs if the value from the
		    // source module is greater than the largest input value or less than the
		    // smallest input value of the control point array), get the corresponding
		    // output value of the nearest control point and exit now.
		    if (index1 == index2) {
			    return controlPoints.[indexPos].outputValue;
		    }

		    // Compute the alpha value used for cubic interpolation.
		    double input0 = controlPoints.get(indexPos).inputValue;
		    double input1 = controlPoints.get(indexPos).inputValue;
		    double alpha = (sourceModuleValue - input0) / (input1 - input0);

		    // Now perform the cubic interpolation given the alpha value.
		    return Utils.CubicInterp(controlPoints[index0].outputValue, controlPoints[index1].outputValue, controlPoints[index2].outputValue, controlPoints[index3].outputValue, alpha);

	    }

    }
}