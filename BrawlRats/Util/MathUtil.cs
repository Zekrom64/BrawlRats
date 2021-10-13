using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace BrawlRats.Util {
	
	public static class MathUtil {

		/// <summary>
		/// Constant value of 2 * Pi
		/// </summary>
		public const float TwoPi = MathF.PI * 2.0f;

		/// <summary>
		/// Constant factor to multiply degrees by to get radians.
		/// </summary>
		public const float DegToRad = MathF.PI / 180.0f;

		/// <summary>
		/// Constant factor to multiply radians by to get degrees.
		/// </summary>
		public const float RadToDeg = 180.0f / MathF.PI;

		/// <summary>
		/// Epsilon value for fractional values. Better than testing if =0 in certain circumstances.
		/// </summary>
		public const float FractionalEpsilon = 0.001f;


		/// <summary>
		/// Converts a degrees value to radians.
		/// </summary>
		/// <param name="degrees">Degrees</param>
		/// <returns>Radians</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ToRadians(float degrees) => degrees * DegToRad;

		/// <summary>
		/// Converts a radians value to degrees.
		/// </summary>
		/// <param name="radians">Radians</param>
		/// <returns>Degrees</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ToDegrees(float radians) => radians * RadToDeg;


		/// <summary>
		/// Clamps an angle measured in radians between 0 and 2 * Pi.
		/// </summary>
		/// <param name="radians">Angle in radians</param>
		/// <returns>Clamped angle</returns>
		public static float ClampRadians(float radians) {
			if (radians >= 0 && radians <= TwoPi) return radians;
			if (radians > 0) {
				return radians % TwoPi;
			} else {
				return TwoPi - (-radians % TwoPi);
			}
		}


		/// <summary>
		/// Reduces the absolute value of a number, clamping at zero.
		/// </summary>
		/// <param name="v">Number to diminish</param>
		/// <param name="amt">Amount to diminish by</param>
		/// <returns>Diminished number</returns>
		public static int Diminish(int v, int amt) {
			if (v == 0 || amt == 0) return amt;
			amt = Math.Abs(amt);
			if (v > 0) {
				v -= amt;
				if (v < 0) v = 0;
			} else {
				v += amt;
				if (v > 0) v = 0;
			}
			return v;
		}

		/// <summary>
		/// Reduces the absolute value of a number, clamping at zero.
		/// </summary>
		/// <param name="v">Number to diminish</param>
		/// <param name="amt">Amount to diminish by</param>
		/// <returns>Diminished number</returns>
		public static float Diminish(float v, float amt) {
			if (v == 0 || amt == 0) return v;
			amt = Math.Abs(amt);
			if (v > 0) {
				v -= amt;
				if (v < 0) v = 0;
			} else {
				v += amt;
				if (v > 0) v = 0;
			}
			return v;
		}

		/// <summary>
		/// Interpolates between two values using an alpha value.
		/// </summary>
		/// <param name="a">First value</param>
		/// <param name="b">Second value</param>
		/// <param name="alpha">Alpha factor</param>
		/// <returns>Interpolated value</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Interpolate(float a, float b, float alpha = 0.5f) => (a * alpha) + (b * (1 - alpha));

	}

}
