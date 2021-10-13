using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract.Core.Math;
using BrawlRats.Util;
using BrawlRats.Content;
using Box2DSharp.Collision;

namespace BrawlRats.Graphics {
	
	public struct CameraView {

		public float AspectRatio => Area.Size.X / Area.Size.Y;

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

		public static CameraView Interpolate(CameraView a, CameraView b, float alpha = 0.5f) {
			return new Rectf() {
				Position = a.Area.Position.Interpolate(b.Area.Position, alpha),
				Size = a.Area.Size.Interpolate(b.Area.Size, alpha)
			};
		}

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

	public struct CameraPan {

		public CameraView Target;

		public float RemainingTime;

		public bool IsDone => RemainingTime <= 0;

		public CameraPan(CameraView target, float time) {
			Target = target;
			RemainingTime = time;
		}

		public bool DoPan(ref CameraView view, float delta) {
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

	public class CameraController : IUpdateable {

		public float AspectRatio { get; set; }

		public Vector2 MinSize { get; set; }

		public CameraMode Mode { get; set; }

		public CameraView CurrentView;

		public CameraPan CurrentPan;

		public Entity TrackedEntity { get; set; }

		private readonly HashSet<Entity> containedEntities = new();
		public ICollection<Entity> ContainedEntities => containedEntities;

		private void UpdateFollow() {
			CurrentPan.Target = TrackedEntity.Physics.AABB;
		}

		private void UpdateContain() {
			AABB bb = default;
			foreach (Entity e in containedEntities) bb.Combine(e.Physics.AABB);
			CurrentPan.Target = bb;
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
			CurrentPan.DoPan(ref CurrentView, delta);
			CurrentView.AdjustForMinSize(MinSize);
			CurrentView.CorrectForAspectRatio(AspectRatio);
		}

		public void Update(UpdatePhase phase, float delta) {
			switch(phase) {
				case UpdatePhase.RenderSetup:
					UpdateCamera(delta);
					break;
			}
		}
	}

}
