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

		protected override void PrepareScene(SceneNewOldStreet scene) {
			scene.VFX.BackgroundColor = Vector3.Zero;
			scene.VFX.Opacity = 0;
		}

		protected override IEnumerator<IChoreographyAction<SceneNewOldStreet>> RunScene(SceneNewOldStreet scene) {
			yield return FadeIn(2);
		}

	}

	public class SceneNewOldStreet : Scene {

		public SceneNewOldStreet() {
			Choreographer = new SceneChoreographerNewOldStreet(this);
		}

	}
}
