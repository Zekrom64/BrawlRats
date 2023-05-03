using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrawlRats.Content.Choreography;

namespace BrawlRats.Content.Scenes {

	public class SceneChoreographerNewOldStreet : SceneChoreographer<SceneNewOldStreet> {

		public SceneChoreographerNewOldStreet(SceneNewOldStreet scene) : base(scene) { }

		protected override void PrepareScene() {
			Scene.VFX.BackgroundColor = Vector3.Zero;
			Scene.VFX.Opacity = 0;
		}

		protected override IEnumerable<IChoreographyAction<SceneNewOldStreet>> RunScene() {
			yield return FadeIn(2);
		}

	}

	public class SceneNewOldStreet : Scene {

		public SceneNewOldStreet() {
			Choreographer = new SceneChoreographerNewOldStreet(this);
		}

	}
}
