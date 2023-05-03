using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BrawlRats.Util;
using BrawlRats.Content.Choreography;
using BrawlRats.Physics;

namespace BrawlRats.Content {

	public class SceneVFX {

		public Vector3 BackgroundColor = Vector3.Zero;

		private float opacity;
		public float Opacity {
			get => opacity;
			set {
				if (value < 0) opacity = 0;
				else if (value > 1) opacity = 1;
				else opacity = value;
			}
		}

	}

	public abstract class Scene {

		public required ISceneChoreographer Choreographer { get; init; }

		public PhysicsWorld Physics { get; } = new();

		public readonly List<Entity> Entities = new();

		public bool IsPlaying { get; set; } = true;

		public readonly SceneVFX VFX = new();

		public virtual void Initialize() {
			Choreographer.Initialize();
		}

		public void Update(float delta) {
			Choreographer.StepLogic(delta);
			Physics.Update(delta);
			foreach (Entity e in Entities) e.Update(delta);
		}
	}

}
