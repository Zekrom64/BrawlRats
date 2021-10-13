using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Box2DSharp.Collision;
using Box2DSharp.Common;

namespace BrawlRats.Util {

	/// <summary>
	/// Extension methods for vectors.
	/// </summary>
	public static class VectorExtensions {

		/// <summary>
		/// Reduces the absolute value of the vector, clamping at zero.
		/// </summary>
		/// <param name="v">Vector to diminish</param>
		/// <param name="amt">Amount to diminish by</param>
		/// <returns>Diminished vector</returns>
		public static Vector2 Diminish(this Vector2 v, Vector2 amt) => new(MathUtil.Diminish(v.X, amt.X), MathUtil.Diminish(v.Y, amt.Y));

		/// <summary>
		/// Computes the minimum of two vectors.
		/// </summary>
		/// <param name="v1">First vector</param>
		/// <param name="v2">Second vector</param>
		/// <returns>Minimum vector value</returns>
		public static Vector2 Min(this Vector2 v1, Vector2 v2) => new(Math.Min(v1.X, v2.X), Math.Min(v1.Y, v2.Y));

		/// <summary>
		/// Computes the maximum of two vectors.
		/// </summary>
		/// <param name="v1">First vector</param>
		/// <param name="v2">Second vector</param>
		/// <returns>Maximum vector value</returns>
		public static Vector2 Max(this Vector2 v1, Vector2 v2) => new(Math.Max(v1.X, v2.X), Math.Max(v1.Y, v2.Y));

		/// <summary>
		/// Rotates this vector about the origin.
		/// </summary>
		/// <param name="v">Vector to rotate</param>
		/// <param name="rot">Rotation to apply</param>
		/// <returns>Rotated vector</returns>
		public static Vector2 RotateAboutOrigin(this Vector2 v, Rotation rot) => new() {
			X = v.X * rot.Cos - v.Y * rot.Sin,
			Y = v.X * rot.Sin + v.Y * rot.Cos
		};

		public static Vector2 Interpolate(this Vector2 v1, Vector2 v2, float alpha = 0.5f) => new() {
			X = MathUtil.Interpolate(v1.X, v2.X, alpha),
			Y = MathUtil.Interpolate(v1.Y, v2.Y, alpha)
		};

	}

	/// <summary>
	/// Extension methods for arrays.
	/// </summary>
	public static class ArrayExtensions {

		/// <summary>
		/// Enumerates each item in the array using the <see cref="Array.ForEach{T}(T[], Action{T})"/> method.
		/// </summary>
		/// <typeparam name="T">Array element type</typeparam>
		/// <param name="array">Array to enumerate</param>
		/// <param name="fn">Enumeration function</param>
		public static void ForEach<T>(this T[] array, Action<T> fn) => Array.ForEach(array, fn);

	}

	/// <summary>
	/// Extension methods for physics types.
	/// </summary>
	public static class PhysicsExtensions {

		/// <summary>
		/// Offsets this AABB by the given amount.
		/// </summary>
		/// <param name="aabb">AABB</param>
		/// <param name="amt">Amount to offset</param>
		/// <returns>Offset AABB</returns>
		public static AABB Offset(this AABB aabb, Vector2 amt) => new() { LowerBound = aabb.LowerBound + amt, UpperBound = aabb.UpperBound + amt };

		/// <summary>
		/// Adds a radian angle to this rotation.
		/// </summary>
		/// <param name="rot">Rotation to add to</param>
		/// <param name="radians">Radian angle to add</param>
		/// <returns>Summed rotation</returns>
		public static Rotation Add(this Rotation rot, float radians) => new(MathUtil.ClampRadians(rot.Angle + radians));

		/// <summary>
		/// Adds two rotations together.
		/// </summary>
		/// <param name="rot1">First rotation</param>
		/// <param name="rot2">Second rotation</param>
		/// <returns>Summed rotation</returns>
		public static Rotation Add(this Rotation rot1, Rotation rot2) => rot1.Add(rot2.Angle);

	}
}
