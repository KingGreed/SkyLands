using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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