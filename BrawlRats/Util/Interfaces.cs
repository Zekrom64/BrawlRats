using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BrawlRats.Util {
	
	public enum UpdatePhase {
		PhysicsCollide,
		PhysicsStep,
		//PhysicsBounds

		LogicChoreography,
		LogicEntities,

		RenderSetup,
		RenderDraw
	}

	public interface IUpdateable {

		public void Update(UpdatePhase phase, float delta);

	}

	public enum DrawLayer {

	}

	public interface IDrawable {

		public void Draw(DrawLayer layer);

	}

}
