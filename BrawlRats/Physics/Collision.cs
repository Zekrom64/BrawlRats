using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Box2D.NetStandard.Collision;
using Box2D.NetStandard.Collision.Shapes;
using Box2D.NetStandard.Common;
using BrawlRats.Util;
using Tesseract.Core.Collections;

namespace BrawlRats.Physics {

	/// <summary>
	/// Bitmask of collision categories.
	/// </summary>
	[Flags]
	public enum CollisionMask : ushort {
		/// <summary>
		/// Scene terrain category.
		/// </summary>
		SceneTerrain = 0x0001,

		/// <summary>
		/// Player category.
		/// </summary>
		Players = 0x0002,
		/// <summary>
		/// Object category.
		/// </summary>
		Objects = 0x0004
	}

	/// <summary>
	/// Enumeration of collision groups
	/// </summary>
	public enum CollisionGroup : short {
		Default = 0
	}

	/// <summary>
	/// A collider is a single instance that can detect overlap with other colliders.
	/// </summary>
	/// <typeparam name="T">Data type stored with collider</typeparam>
	public struct Collider<T> {

		/// <summary>
		/// The shape of the collider.
		/// </summary>
		public Shape Shape;

		/// <summary>
		/// A transform specifying the orientation of this collider's shape relative to what it is attached to.
		/// </summary>
		public Transform Transform;

		/// <summary>
		/// The data associated with the collider.
		/// </summary>
		public T Data;
	}

	/// <summary>
	/// A collidable object stores a group of colliders and additional information.
	/// </summary>
	/// <typeparam name="T">Collider data type</typeparam>
	public interface ICollidable<T> {

		/// <summary>
		/// The AABB containing all the colliders in this object.
		/// </summary>
		public AABB AABB { get; }

		/// <summary>
		/// A collection of all the colliders in this object.
		/// </summary>
		public IReadOnlyCollection<Collider<T>> Colliders { get; }

		/// <summary>
		/// Tests if any colliders in this object overlap with those in another object.
		/// </summary>
		/// <typeparam name="T2">Second collider data type</typeparam>
		/// <param name="other">Collider to test against</param>
		/// <param name="t1">Transform to apply to this collider</param>
		/// <param name="t2">Transform to apply to the other collider</param>
		/// <returns></returns>
		public bool Intersects<T2>(ICollidable<T2> other, Transform t1 = default, Transform t2 = default);

		/// <summary>
		/// Gathers the overlaps between colliders in this object and those in another collidable.
		/// </summary>
		/// <typeparam name="T2">Second collider data type</typeparam>
		/// <param name="other">Collider to test against</param>
		/// <param name="consumer">Consumer to invoke for each found intersection</param>
		/// <param name="t1">Transform to apply to this collider</param>
		/// <param name="t2">Transform to apply to the other collider</param>
		/// <returns></returns>
		public bool GatherIntersects<T2>(ICollidable<T2> other, Action<T, T2> consumer, Transform t1 = default, Transform t2 = default);

	}

	/// <summary>
	/// A collider list stores a list of colliders.
	/// </summary>
	/// <typeparam name="T">Collider data type</typeparam>
	public class ColliderList<T> : ICollection<Collider<T>>, ICollidable<T> {

		private readonly List<Collider<T>> colliders = new();

		private bool recomputeAABB;
		private AABB aabb;

		public AABB AABB {
			get {
				if (recomputeAABB) {
					aabb = default;
					foreach(var c in colliders) {
						c.Shape.ComputeAABB(out AABB cbb, c.Transform, 0);
						aabb.Combine(cbb);
					}
					recomputeAABB = false;
				}
				return aabb;
			}
		}

		public IReadOnlyCollection<Collider<T>> Colliders => colliders;

		private bool CanCollide<T2>(ICollidable<T2> other, Transform t1 = default, Transform t2 = default) {
			throw new NotImplementedException();
			/*
			// If either count is zero there can never be a collision
			if (colliders.Count == 0 || other.Colliders.Count == 0) return false;
			// AABBs must overlap for collider lists to overlap
			AABB bb1, bb2;

			// Adjust AABB for potential rotation
			float angle1 = MathUtil.ClampRadians(t1.Rotation.Angle);
			float angle2 = MathUtil.ClampRadians(t2.Rotation.Angle);
			if (angle1 > MathUtil.FractionalEpsilon) {
				Vector2 size = AABB.GetExtents();
				Shapes.Box(size.X, size.Y, 0).ComputeAABB(out bb1, t1, 0);
			} else bb1 = AABB.Offset(t1.Position);
			if (angle2 > MathUtil.FractionalEpsilon) {
				Vector2 size = other.AABB.GetExtents();
				Shapes.Box(size.X, size.Y, 0).ComputeAABB(out bb2, t2, 0);
			} else bb2 = AABB.Offset(t2.Position);
			return CollisionUtils.TestOverlap(bb1, bb2);
			*/
		}

		public bool Intersects<T2>(ICollidable<T2> other, Transform t1 = default, Transform t2 = default) {
			throw new NotImplementedException();
			/*
			if (!CanCollide(other, t1, t2)) return false;

			float angle1 = MathUtil.ClampRadians(t1.Rotation.Angle);
			float angle2 = MathUtil.ClampRadians(t2.Rotation.Angle);
			// Perform collision checks
			foreach (Collider<T> c1 in this) {
				Transform tc1 = c1.Transform;
				if (angle1 > MathUtil.FractionalEpsilon) {
					tc1.Rotation.Add(t1.Rotation);
					tc1.Position = tc1.Position.RotateAboutOrigin(t1.Rotation);
				}
				foreach(Collider<T2> c2 in other.Colliders) {
					Transform tc2 = c2.Transform;
					if (angle2 > MathUtil.FractionalEpsilon) {
						tc2.Rotation.Add(t2.Rotation);
						tc2.Position = tc2.Position.RotateAboutOrigin(t2.Rotation);
					}
					if (CollisionUtils.TestOverlap(c1.Shape, 0, c2.Shape, 0, tc1, tc2, null)) return true;
				}
			}
			return false;
			*/
		}

		public bool GatherIntersects<T2>(ICollidable<T2> other, Action<T, T2> consumer, Transform t1 = default, Transform t2 = default) {
			throw new NotImplementedException();
			/*
			if (!CanCollide(other, t1, t2)) return false;

			bool collision = false;
			float angle1 = MathUtil.ClampRadians(t1.Rotation.Angle);
			float angle2 = MathUtil.ClampRadians(t2.Rotation.Angle);
			// Perform collision checks
			foreach (Collider<T> c1 in this) {
				Transform tc1 = c1.Transform;
				if (angle1 > MathUtil.FractionalEpsilon) {
					tc1.Rotation.Add(t1.Rotation);
					tc1.Position = tc1.Position.RotateAboutOrigin(t1.Rotation);
				}
				foreach (Collider<T2> c2 in other.Colliders) {
					Transform tc2 = c2.Transform;
					if (angle2 > MathUtil.FractionalEpsilon) {
						tc2.Rotation.Add(t2.Rotation);
						tc2.Position = tc2.Position.RotateAboutOrigin(t2.Rotation);
					}
					if (CollisionUtils.TestOverlap(c1.Shape, 0, c2.Shape, 0, tc1, tc2, null)) {
						consumer(c1.Data, c2.Data);
						collision = true;
					}
				}
			}

			return collision;
			*/
		}

		// ICollection<Collider<T>>

		public int Count => colliders.Count;

		public bool IsReadOnly => false;

		public void Add(Collider<T> item) {
			colliders.Add(item);
			recomputeAABB = true;
		}

		public void Clear() {
			colliders.Clear();
			aabb = default;
			recomputeAABB = false;
		}

		public bool Contains(Collider<T> item) => colliders.Contains(item);

		public void CopyTo(Collider<T>[] array, int arrayIndex) => colliders.CopyTo(array, arrayIndex);

		public bool Remove(Collider<T> item) {
			if (colliders.Remove(item)) {
				recomputeAABB = true;
				return true;
			} else return false;
		}

		public IEnumerator<Collider<T>> GetEnumerator() => colliders.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => colliders.GetEnumerator();
	}

	/// <summary>
	/// An animated collider list internally stores a list of collidables corresponding to each
	/// frame of an animation, but externally exposes the collidable indexed by the current frame.
	/// </summary>
	/// <typeparam name="T">Collider data type</typeparam>
	public class AnimatedColliderList<T> : ICollidable<T> {

		public readonly List<ICollidable<T>> Frames = new();

		private int currentFrame;
		public int CurrentFrame {
			get {
				if (currentFrame >= Frames.Count) currentFrame = Frames.Count - 1;
				return currentFrame;
			}
			set {
				currentFrame = value;
				if (currentFrame >= Frames.Count) currentFrame = Frames.Count - 1;
				if (currentFrame < 0) currentFrame = 0;
			}
		}

		public AABB AABB {
			get {
				if (Frames.Count == 0) return default;
				else return Frames[CurrentFrame].AABB;
			}
		}

		public IReadOnlyCollection<Collider<T>> Colliders {
			get {
				if (Frames.Count == 0) return Collection<Collider<T>>.EmptyList;
				else return Frames[CurrentFrame].Colliders;
			}
		}

		public bool Intersects<T2>(ICollidable<T2> other, Transform t1 = default, Transform t2 = default) =>
			Frames[CurrentFrame].Intersects(other, t1, t2);

		public bool GatherIntersects<T2>(ICollidable<T2> other, Action<T, T2> consumer, Transform t1 = default, Transform t2 = default) =>
			Frames[CurrentFrame].GatherIntersects(other, consumer, t1, t2);

	}

}
