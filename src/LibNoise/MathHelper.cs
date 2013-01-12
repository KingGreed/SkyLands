using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Vector3 = Mogre.Vector3;
using Vector2 = Mogre.Vector2;

namespace LibNoise
{
    /**
     * Class containing various mathematical functions
     */
    public class MathHelper {
	    /**
	     * A "close to zero" double epsilon value for use
	     */

	    /**
	     * A "close to zero" float epsilon value for use
	     */

	    public const double PI = Math.PI;

	    public const double SQUARED_PI = PI * PI;

	    public const double HALF_PI = 0.5 * PI;

	    public const double QUARTER_PI = 0.5 * HALF_PI;

	    public const double TWO_PI = 2.0 * PI;

	    public const double THREE_PI_HALVES = TWO_PI - HALF_PI;

	    public const double DEGTORAD = PI / 180.0;

	    public const double RADTODEG = 180.0 / PI;

	    public readonly double SQRTOFTWO = Math.Sqrt(2.0);

	    public readonly double HALF_SQRTOFTWO = 0.5 * Math.Sqrt(2.0);

	    /**
	     * Calculates the squared length of all axis offsets given
	     *
	     * @param values of the axis to get the squared length of
	     * @return the squared length
	     */
	    public static double lengthSquared(params double[] values) {
		    double rval = 0;
		    foreach(double value in values) {
			    rval += value * value;
		    }
		    return rval;
	    }

	    /**
	     * Calculates the length of all axis offsets given
	     *
	     * @param values of the axis to get the length of
	     * @return the length
	     */
	    public static double length(params double[] values) {
		    return Math.Sqrt(lengthSquared(values));
	    }

	    /**
	     * Gets the difference between two angles
	     * This value is always positive (0 - 180)
	     *
	     * @param angle1
	     * @param angle2
	     * @return the positive angle difference
	     */
	    public static float getAngleDifference(float angle1, float angle2) {
		    return Math.Abs(wrapAngle(angle1 - angle2));
	    }

	    /**
	     * Gets the difference between two radians
	     * This value is always positive (0 - PI)
	     *
	     * @param radian1
	     * @param radian2
	     * @return the positive radian difference
	     */
	    public static double getRadianDifference(double radian1, double radian2) {
		    return Math.Abs(wrapRadian(radian1 - radian2));
	    }

	    /**
	     * Wraps the angle between -180 and 180 degrees
	     *
	     * @param angle to wrap
	     * @return -180 > angle <= 180
	     */
	    public static float wrapAngle(float angle) {
		    angle %= 360f;
		    if (angle <= -180) {
			    return angle + 360;
		    } else if (angle > 180) {
			    return angle - 360;
		    } else {
			    return angle;
		    }
	    }

	    /**
	     * Wraps a byte between 0 and 256
	     * 
	     * @param value to wrap
	     * @return 0 >= byte < 256
	     */
	    public static byte wrapByte(int value) {
		    value %= 256;
		    if (value < 0) {
			    value += 256;
		    }
		    return (byte) value;
	    }
	
	    /**
	     * Wraps the radian between -PI and PI
	     *
	     * @param radian to wrap
	     * @return -PI > radian <= PI
	     */
	    public static double wrapRadian(double radian) {
		    radian %= TWO_PI;
		    if (radian <= -PI) {
			    return radian + TWO_PI;
		    } else if (radian > PI) {
			    return radian - TWO_PI;
		    } else {
			    return radian;
		    }
	    }

	    /**
	     * Rounds a number to the amount of decimals specified
	     *
	     * @param input to round
	     * @param decimals to round to
	     * @return the rounded number
	     */
	    public static double round(double input, int decimals) {
		    double p = Math.Pow(10, decimals);
		    return Math.Round(input * p) / p;
	    }

	    /**
	     * Calculates the linear interpolation between a and b with the given
	     * percent
	     *
	     * @param a
	     * @param b
	     * @param percent
	     * @return
	     */
	    public static double lerp(double a, double b, double percent) {
		    return (1 - percent) * a + percent * b;
	    }

	    /**
	     * Calculates the linear interpolation between a and b with the given
	     * percent
	     *
	     * @param a
	     * @param b
	     * @param percent
	     * @return
	     */
	    public static float lerp(float a, float b, float percent) {
		    return (1 - percent) * a + percent * b;
	    }

	    /**
	     * Calculates the linear interpolation between a and b with the given
	     * percent
	     *
	     * @param a
	     * @param b
	     * @param percent
	     * @return
	     */
	    public static int lerp(int a, int b, double percent) {
		    return (int) ((1 - percent) * a + percent * b);
	    }

        /**
	     * Calculates the value at x using linear interpolation
	     *
	     * @param x the X coord of the value to interpolate
	     * @param x1 the X coord of q0
	     * @param x2 the X coord of q1
	     * @param q0 the first known value (x1)
	     * @param q1 the second known value (x2)
	     * @return the interpolated value
	     */
	    public static double lerp(double x, double x1, double x2, double q0, double q1) {
		    return ((x2 - x) / (x2 - x1)) * q0 + ((x - x1) / (x2 - x1)) * q1;
	    }

        	/**
	     * Calculates the value at x,y using bilinear interpolation
	     *
	     * @param x the X coord of the value to interpolate
	     * @param y the Y coord of the value to interpolate
	     * @param q00 the first known value (x1, y1)
	     * @param q01 the second known value (x1, y2)
	     * @param q10 the third known value (x2, y1)
	     * @param q11 the fourth known value (x2, y2)
	     * @param x1 the X coord of q00 and q01
	     * @param x2 the X coord of q10 and q11
	     * @param y1 the Y coord of q00 and q10
	     * @param y2 the Y coord of q01 and q11
	     * @return the interpolated value
	     */
	    public static double biLerp(double x, double y, double q00, double q01,
			    double q10, double q11, double x1, double x2, double y1, double y2) {
		    double q0 = lerp(x, x1, x2, q00, q10);
		    double q1 = lerp(x, x1, x2, q01, q11);
		    return lerp(y, y1, y2, q0, q1);
	    }
	
	    /**
	     * Calculates the value at a target using bilinear interpolation
	     *
	     * @param target the vector of the value to interpolate
	     * @param q00 the first known value (known1.x, known1.y)
	     * @param q01 the second known value (known1.x, known2.y)
	     * @param q10 the third known value (known2.x, known1.y)
	     * @param q11 the fourth known value (known2.x, known2.y)
	     * @param known1 the X coord of q00 and q01 and the Y coord of q00 and q10
	     * @param known2 the X coord of q10 and q11 and the Y coord of q01 and q11
	     * @return the interpolated value
	     */
	    public static double biLerp(Vector2 target, double q00, double q01,
			    double q10, double q11, Vector2 known1, Vector2 known2) {
		        double q0 = lerp(target.x, known1.x, known2.x, q00, q10);
		        double q1 = lerp(target.x, known1.x, known2.x, q01, q11);
		        return lerp(target.y, known1.y, known2.y, q0, q1);
	        }

	        /**
	         * Calculates the value at x,y,z using trilinear interpolation
	         *
	         * @param x the X coord of the value to interpolate
	         * @param y the Y coord of the value to interpolate
	         * @param z the Z coord of the value to interpolate
	         * @param q000 the first known value (x1, y1, z1)
	         * @param q001 the second known value (x1, y2, z1)
	         * @param q010 the third known value (x1, y1, z2)
	         * @param q011 the fourth known value (x1, y2, z2)
	         * @param q100 the fifth known value (x2, y1, z1)
	         * @param q101 the sixth known value (x2, y2, z1)
	         * @param q110 the seventh known value (x2, y1, z2)
	         * @param q111 the eighth known value (x2, y2, z2)
	         * @param x1 the X coord of q000, q001, q010 and q011
	         * @param x2 the X coord of q100, q101, q110 and q111
	         * @param y1 the Y coord of q000, q010, q100 and q110
	         * @param y2 the Y coord of q001, q011, q101 and q111
	         * @param z1 the Z coord of q000, q001, q100 and q101
	         * @param z2 the Z coord of q010, q011, q110 and q111
	         * @return the interpolated value
	         */
	        public static double triLerp(double x, double y, double z, double q000, double q001,
			        double q010, double q011, double q100, double q101, double q110, double q111,
			        double x1, double x2, double y1, double y2, double z1, double z2) {
		        double q00 = lerp(x, x1, x2, q000, q100);
		        double q01 = lerp(x, x1, x2, q010, q110);
		        double q10 = lerp(x, x1, x2, q001, q101);
		        double q11 = lerp(x, x1, x2, q011, q111);
		        double q0 = lerp(y, y1, y2, q00, q10);
		        double q1 = lerp(y, y1, y2, q01, q11);
		        return lerp(z, z1, z2, q0, q1);
	        }
	
	        /**
	         * Calculates the value at target using trilinear interpolation
	         *
	         * @param target the vector of the value to interpolate
	         * @param q000 the first known value (known1.x, known1.y, known1.z)
	         * @param q001 the second known value (known1.x, known2.y, known1.z)
	         * @param q010 the third known value (known1.x, known1.y, known2.z)
	         * @param q011 the fourth known value (known1.x, known2.y, known2.z)
	         * @param q100 the fifth known value (known2.x, known1.y, known1.z)
	         * @param q101 the sixth known value (known2.x, known2.y, known1.z)
	         * @param q110 the seventh known value (known2.x, known1.y, known2.z)
	         * @param q111 the eighth known value (known2.x, known2.y, known2.z)
	         * @param known1 the X coord of q000, q001, q010 and q011, the Y coord of q000, q010, q100 and q110
	         * and the Z coord of q000, q001, q100 and q101
	         * @param known2 the X coord of q100, q101, q110 and q111, the Y coord of q001, q011, q101 and q111
	         * and the Z coord of q010, q011, q110 and q111
	         * @return the interpolated value
	         */
	        public static double triLerp(Vector3 target, double q000, double q001, double q010,
			        double q011, double q100, double q101, double q110, double q111, Vector3 known1, Vector3 known2) {
		        double q00 = lerp(target.x, known1.x, known2.x, q000, q100);
		        double q01 = lerp(target.x, known1.x, known2.x, q010, q110);
		        double q10 = lerp(target.x, known1.x, known2.x, q001, q101);
		        double q11 = lerp(target.x, known1.x, known2.x, q011, q111);
		        double q0  = lerp(target.y, known1.y, known2.y, q00, q10);
		        double q1  = lerp(target.y, known1.y, known2.y, q01, q11);
		        return lerp(target.z, known1.z, known2.z, q0, q1);
	        }


	    /**
	     * Calculates the linear interpolation between a and b with the given
	     * percent
	     *
	     * @param a
	     * @param b
	     * @param percent
	     * @return
	     *
	    public static Color lerp(Color a, Color b, double percent) {
		    int red = lerp(a.getRed(), b.getRed(), percent);
		    int blue = lerp(a.getBlue(), b.getBlue(), percent);
		    int green = lerp(a.getGreen(), b.getGreen(), percent);
		    int alpha = lerp(a.getAlpha(), b.getAlpha(), percent);
		    return new Color(red, green, blue, alpha);
	    }

	    public static Color blend(Color a, Color b) {
		    int red = lerp(a.getRed(), b.getRed(), (a.getAlpha()/255.0));
		    int blue = lerp(a.getBlue(), b.getBlue(), (a.getAlpha()/255.0));
		    int green = lerp(a.getGreen(), b.getGreen(), (a.getAlpha()/255.0));
		    int alpha = lerp(a.getAlpha(), b.getAlpha(), (a.getAlpha()/255.0));
		    return new Color(red, green, blue, alpha);
	    }*/

	    public static double clamp(double value, double low, double high) {
		    if (value < low) {
			    return low;
		    }
		    if (value > high) {
			    return high;
		    }
		    return value;
	    }

	    public static int clamp(int value, int low, int high) {
		    if (value < low) {
			    return low;
		    }
		    if (value > high) {
			    return high;
		    }
		    return value;
	    }

	    //Fast Math Implementation
	    public  static double cos( double x) {
		    return sin(x + (x > HALF_PI ? -THREE_PI_HALVES : HALF_PI));
	    }

	    public  static double sin(double x) {
		    x = sin_a * x * Math.Abs(x) + sin_b * x;
		    return sin_p * (x * Math.Abs(x) - x) + x;
	    }

	    public  static double tan( double x) {
		    return sin(x) / cos(x);
	    }

	    public  static double asin( double x) {
		    return x * (Math.Abs(x) * (Math.Abs(x) * asin_a + asin_b) + asin_c) + Math.Sign(x) * (asin_d - Math.Sqrt(1 - x * x));
	    }

	    public  static double acos( double x) {
		    return HALF_PI - asin(x);
	    }

	    public  static double atan( double x) {
		    return Math.Abs(x) < 1 ? x / (1 + atan_a * x * x) : Math.Sign(x) * HALF_PI - x / (x * x + atan_a);
	    }

	    public  static double sqrt( double x) {
		    return Math.Sqrt(x);
	    }

	    private const double sin_a = -4 / SQUARED_PI;

	    private const double sin_b = 4 / PI;

	    private const double sin_p = 9d / 40;

	    private  static double asin_a = -0.0481295276831013447d;

	    private  static double asin_b = -0.343835993947915197d;

	    private  static double asin_c = 0.962761848425913169d;

	    private  static double asin_d = 1.00138940860107040d;

	    private  static double atan_a = 0.280872d;

	    // Integer Maths

	    public static int floor(double x) {
		    int y = (int) x;
		    if (x < y) {
			    return y - 1;
		    }
		    return y;
	    }

	    public static int floor(float x) {
		    int y = (int) x;
		    if (x < y) {
			    return y - 1;
		    }
		    return y;
	    }

	    /**
	     * Gets the maximum byte value from two values
	     * 
	     * @param value1
	     * @param value2
	     * @return the maximum value
	     */
	    public static byte max(byte value1, byte value2) {
		    return value1 > value2 ? value1 : value2;
	    }

	    /**
	     * Rounds an integer up to the next power of 2.
	     *
	     * @param x
	     * @return the lowest power of 2 greater or equal to x
	     */
	    public static int roundUpPow2(int x) {
		    if (x <= 0) {
			    return 1;
		    } else if (x > 0x40000000) {
			    throw new ArgumentException("Rounding " + x + " to the next highest power of two would exceed the int range");
		    } else {
			    x--;
			    x |= x >> 1;
			    x |= x >> 2;
			    x |= x >> 4;
			    x |= x >> 8;
			    x |= x >> 16;
			    x++;
			    return x;
		    }
	    }

	    /**
	     * Calculates the mean of a set of values
	     *
	     * @param values to calculate the mean of
	     * @return the mean
	     */
	    public static int mean(params int[] values) {
		    int sum = 0;
		    foreach(int v in values) {
			    sum += v;
		    }
		    return sum/values.Length;
	    }

	    /**
	     * Calculates the mean of a set of values.
	     *
	     * @param values to calculate the mean of
	     */
	    public static double mean(params double[] values) {
		    double sum = 0;
		    foreach(double v in values) {
			    sum += v;
		    }
		    return sum/values.Length;
	    }
    }
}