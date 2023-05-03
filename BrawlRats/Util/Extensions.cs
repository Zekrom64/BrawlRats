using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Box2D.NetStandard.Common;
using BrawlRats.Physics;

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
		/// Rotates this vector about the origin.
		/// </summary>
		/// <param name="v">Vector to rotate</param>
		/// <param name="rot">Rotation to apply</param>
		/// <returns>Rotated vector</returns>
		public static Vector2 RotateAboutOrigin(this Vector2 v, float rot) {
			float c = MathF.Cos(rot), s = MathF.Sin(rot);
			return new Vector2(v.X * c - v.Y * s, v.X * s + v.Y * c);
		}

		public static Vector2 Interpolate(this Vector2 v1, Vector2 v2, float alpha = 0.5f) => new() {
			X = MathUtil.Interpolate(v1.X, v2.X, alpha),
			Y = MathUtil.Interpolate(v1.Y, v2.Y, alpha)
		};

	}

}
