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

	public abstract class Scene : IUpdateable {

		[NotNull]
		public ISceneChoreographer Choreographer { get; protected set; }

		public Physics Physics { get; } = new();

		public readonly List<Entity> Entities = new();

		public bool IsPlaying { get; set; } = true;

		public readonly SceneVFX VFX = new();

		public virtual void Initialize() {
			Choreographer.Initialize();
		}

		public void Update(UpdatePhase phase, float delta) {
			switch(phase) {
				case UpdatePhase.LogicChoreography:
					Choreographer.StepLogic(delta);
					break;
				case UpdatePhase.PhysicsStep:
				case UpdatePhase.PhysicsCollide:
					Physics.Update(phase, delta);
					break;
				case UpdatePhase.LogicEntities:
					foreach (Entity e in Entities) e.Update(phase, delta);
					break;
			}
		}
	}

}
