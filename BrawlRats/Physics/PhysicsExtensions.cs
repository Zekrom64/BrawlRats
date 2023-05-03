using System.Numerics;
using Box2D.NetStandard.Collision;
using Tesseract.Core.Numerics;

namespace BrawlRats.Physics {

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
		public static AABB Offset(this AABB aabb, Vector2 amt) => new(aabb.LowerBound + amt, aabb.UpperBound + amt);

	}

}
