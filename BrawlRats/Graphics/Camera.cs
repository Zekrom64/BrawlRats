using System;
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

namespace BrawlRats.Graphics {
	
	/// <summary>
	/// A camera view describes the 
	/// </summary>
	public struct CameraView {

		/// <summary>
		/// The aspect ratio of the camera view
		/// </summary>
		public float AspectRatio => Area.Size.X / Area.Size.Y;

		/// <summary>
		/// The screen area the camera view covers
		/// </summary>
		public Rectf Area;

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
		/// Interpolates between two different camera views based on a scaling factor, where
		/// 0 corresponds to the first view and 1 corresponds to the second view.
		/// </summary>
		/// <param name="a">First view to interpolate</param>
		/// <param name="b">Second view to interpolate</param>
		/// <param name="alpha">The interpolation factor</param>
		/// <returns>The interpolated view between the two</returns>
		public static CameraView Interpolate(CameraView a, CameraView b, float alpha = 0.5f) {
			return new Rectf() {
				Position = a.Area.Position.Interpolate(b.Area.Position, alpha),
				Size = a.Area.Size.Interpolate(b.Area.Size, alpha)
			};
		}

		/// <summary>
		/// Adjusts this camera view to fit the given minimum size, centered
		/// on the original view.
		/// </summary>
		/// <param name="minSize">The minimum view size</param>
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
		/// Corrects this camera view to match the given aspect ratio.
		/// </summary>
		/// <param name="ratio">Requested aspect ratio</param>
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

	}

	public struct CameraMove {

		public CameraView Target;

		public float RemainingTime;

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

	public class CameraController {

		public float AspectRatio { get; set; }

		public Vector2 MinSize { get; set; }

		public CameraMode Mode { get; set; }

		public CameraView CurrentView;

		public CameraMove CurrentMove;

		public Entity? TrackedEntity { get; set; }

		private readonly List<Entity> containedEntities = new();
		public ICollection<Entity> ContainedEntities => containedEntities;

		private void UpdateFollow() {
			CurrentMove.Target = TrackedEntity.Physics.AABB;
		}

		private void UpdateContain() {
			if (containedEntities.Count > 0) {
				
			}
		}

		private void UpdateCamera(float delta) {
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
