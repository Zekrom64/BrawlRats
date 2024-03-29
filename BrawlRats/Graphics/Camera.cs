﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract.Core.Numerics;
using BrawlRats.Util;
using BrawlRats.Content;
using Box2D.NetStandard.Collision;
using System.Runtime.InteropServices;
using BrawlRats.Physics;

namespace BrawlRats.Graphics {

	/// <summary>
	/// A camera view is a rectangle defining the area seen by a 'camera' ie. the displayed view.
	/// </summary>
	public struct CameraView : IEquatable<CameraView> {

		/// <summary>
		/// The aspect ratio of this camera view.
		/// </summary>
		public float AspectRatio => Area.Size.X / Area.Size.Y;

		/// <summary>
		/// The rectangle defining the view of the camera.
		/// </summary>
		public Rectf Area;

		/// <summary>
		/// Creates a new camera view with the given area.
		/// </summary>
		/// <param name="area">Camera view area</param>
		public CameraView(Rectf area) { Area = area; }

		/// <summary>
		/// Creates a new camera view at the given position with the given size.
		/// </summary>
		/// <param name="position">View position</param>
		/// <param name="size">View size</param>
		public CameraView(Vector2 position, Vector2 size) { Area = new(position.X, position.Y, size.X, size.Y); }

		/// <summary>
		/// Creates a new camera view at the given position with the given size.
		/// </summary>
		/// <param name="x">View X coordinate</param>
		/// <param name="y">View Y coordinate</param>
		/// <param name="width">View width</param>
		/// <param name="height">View height</param>
		public CameraView(float x, float y, float width, float height) { Area = new(x, y, width, height); }

		public static implicit operator CameraView(Rectf area) => new() { Area = area };
		public static implicit operator CameraView(AABB aabb) => new() {
			Area = new() {
				Position = aabb.LowerBound,
				Size = aabb.UpperBound - aabb.LowerBound
			}
		};

		public static implicit operator Matrix4x4(CameraView c) =>
			Matrix4x4.CreateOrthographic(c.Area.Size.X, c.Area.Size.Y, 0, 1) *
			Matrix4x4.CreateTranslation(c.Area.Position.X, c.Area.Position.Y, 0);

		/// <summary>
		/// Interpolates between two different camera views.
		/// </summary>
		/// <param name="a">First camera view</param>
		/// <param name="b">Second camera view</param>
		/// <param name="alpha">Alpha factor determining interpolation position</param>
		/// <returns>Interpolated camera view</returns>
		public static CameraView Interpolate(CameraView a, CameraView b, float alpha = 0.5f) {
			return new Rectf() {
				Position = a.Area.Position.Interpolate(b.Area.Position, alpha),
				Size = a.Area.Size.Interpolate(b.Area.Size, alpha)
			};
		}

		/// <summary>
		/// Adjusts this camera view to fit the required minimum size.
		/// </summary>
		/// <param name="minSize">The required minimum size</param>
		public void AdjustForMinSize(Vector2 minSize) {
			if (minSize.X > Area.Size.X) {
				Area.Position.X -= (minSize.X - Area.Size.X) * 0.5f;
				Area.Size.X = minSize.X;
			}
			if (minSize.Y > Area.Size.Y) {
				Area.Position.Y -= (minSize.Y - Area.Size.Y) * 0.5f;
				Area.Size.Y = minSize.Y;
			}
		}

		/// <summary>
		/// Corrects this camera view to have the requested aspect ratio. If the current
		/// aspect ratio is larger (wider) than the requested ratio, the height is increased
		/// to fit the requested ratio, else if it is smaller (taller) the width is increased.
		/// </summary>
		/// <param name="ratio">The requested aspect ratio</param>
		public void CorrectForAspectRatio(float ratio) {
			float aspect = AspectRatio;
			if (aspect > ratio) {
				// Scene is wider than the required ratio, correct height
				Area.Size.Y = Area.Size.X * (1.0f / ratio);
			} else if (aspect < ratio) {
				// Scene is taller than the required ratio, correct width
				Area.Size.X = Area.Size.Y * ratio;
			}
		}


		public bool Equals(CameraView view) =>
			Area.Position == view.Area.Position &&
			Area.Size == view.Area.Size;

		public override bool Equals(object? obj) => obj is CameraView view && Equals(view);

		public override int GetHashCode() => Area.GetHashCode();

		public static bool operator ==(CameraView v1, CameraView v2) => v1.Equals(v2);
		public static bool operator !=(CameraView v1, CameraView v2) => !v1.Equals(v2);

	}

	public struct CameraMove {

		/// <summary>
		/// The target camera view.
		/// </summary>
		public CameraView Target;

		/// <summary>
		/// The remaining amount of time in the pan.
		/// </summary>
		public float RemainingTime;

		/// <summary>
		/// If the pan is done, ie. there is no remaining time left in the pan.
		/// </summary>
		public bool IsDone => RemainingTime <= 0;

		public CameraMove(CameraView target, float time) {
			Target = target;
			RemainingTime = time;
		}

		public bool DoMove(ref CameraView view, float delta) {
			// If stepped over the remaining time, set to the target and clear time
			if (delta > RemainingTime) {
				RemainingTime = 0;
				view = Target;
				return true;
			} else {
				// Else interpolate by scale of delta within remaining
				float alpha = RemainingTime / delta;
				view = CameraView.Interpolate(Target, view, alpha);
				// Subtract from remaining
				RemainingTime -= delta;
				return false;
			}
		}

	}

	/// <summary>
	/// Enumeration of camera modes.
	/// </summary>
	public enum CameraMode {
		/// <summary>
		/// The camera does not follow anything automatically, although
		/// it may pan between views.
		/// </summary>
		Fixed,
		/// <summary>
		/// The camera will follow a targeted entity.
		/// </summary>
		Follow,
		/// <summary>
		/// The camera will contain a group of entities.
		/// </summary>
		Contain
	}

	/// <summary>
	/// A camera controller manages camera panning and tracking within a set of constraints. The controller
	/// behaves differently depending on the <see cref="Mode"/> it is in:
	/// <list type="bullet">
	/// <item><see cref="CameraMode.Fixed"/> - The controller will only update based on <see cref="CurrentPan"/>.</item>
	/// <item><see cref="CameraMode.Follow"/> - The controller will follow a specific entity (<see cref="TrackedEntity"/>),
	/// targeting their bounding box.</item>
	/// <item><see cref="CameraMode.Contain"/> - The controller will target a view based on the minimum bounding box containing
	/// the bounding boxes of every entity in <see cref="ContainedEntities"/>.</item>
	/// </list>
	/// The current camera view will always be constrained by <see cref="AspectRatio"/> and <see cref="MinSize"/>. Any panning
	/// automatically generated by non-fixed modes will use a panning time defined by <see cref="TrackingMoveTime"/>.
	/// </summary>
	public class CameraController {

		/// <summary>
		/// The required aspect ratio of the final camera view.
		/// </summary>
		public float AspectRatio { get; set; }

		/// <summary>
		/// The required minimum size of the final camera view.
		/// </summary>
		public Vector2 MinSize { get; set; }

		/// <summary>
		/// The current mode of the camerea.
		/// </summary>
		public CameraMode Mode { get; set; }

		/// <summary>
		/// The current camera view.
		/// </summary>
		public CameraView CurrentView;

		/// <summary>
		/// The current movement for the camera.
		/// </summary>
		public CameraMove CurrentMove;

		public Entity? TrackedEntity { get; set; }

		private readonly List<Entity> containedEntities = new();

		/// <summary>
		/// The time that should be used when tracking different entities. Setting this to zero effectively disables
		/// smooth camera movement and will immediately snap to the tracked view.
		/// </summary>
		public float TrackingMoveTime { get; set; } = 0.5f;

		private void UpdateFollow() {
			if (TrackedEntity != null) CurrentMove.Target = TrackedEntity.BoundingBox;
		}

		private void UpdateContain() {
			if (containedEntities.Count > 0) {
				AABB bb = containedEntities[0].BoundingBox;
				for (int i = 1; i < containedEntities.Count; i++) bb.Combine(containedEntities[i].BoundingBox);
				CurrentMove.Target = bb;
			}
		}

		public void UpdateCamera(float delta) {
			if (Mode != CameraMode.Follow) TrackedEntity = null;
			else if (Mode != CameraMode.Contain && containedEntities.Count > 0) containedEntities.Clear();
			switch(Mode) {
				case CameraMode.Fixed: break;
				case CameraMode.Follow:
					UpdateFollow();
					break;
				case CameraMode.Contain:
					UpdateContain();
					break;
			}
			CurrentMove.DoMove(ref CurrentView, delta);
			CurrentView.AdjustForMinSize(MinSize);
			CurrentView.CorrectForAspectRatio(AspectRatio);
		}

	}

}
