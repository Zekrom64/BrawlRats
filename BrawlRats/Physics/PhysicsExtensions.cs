using System.Numerics;
using Box2D.NetStandard.Collision;
using Tesseract.Core.Numerics;

namespace BrawlRats.Physics {

	/// <summary>
	/// Extension methods for physics types.
	/// </summary>
	public static class PhysicsExtensions {

		/// <summary>
		/// Combines two bounding boxes together, producing a bounding box that holds both.
		/// </summary>
		/// <param name="a">First bounding box</param>
		/// <param name="b">Second bounding box</param>
		/// <returns>Combined bounding box</returns>
		public static AABB Combine(this AABB a, AABB b) => new(a.LowerBound.Min(b.LowerBound), a.UpperBound.Max(b.UpperBound));

		/// <summary>
		/// Offsets this AABB by the given amount.
		/// </summary>
		/// <param name="aabb">AABB</param>
		/// <param name="amt">Amount to offset</param>
		/// <returns>Offset AABB</returns>
		public static AABB Offset(this AABB aabb, Vector2 amt) => new(aabb.LowerBound + amt, aabb.UpperBound + amt);

	}

}
