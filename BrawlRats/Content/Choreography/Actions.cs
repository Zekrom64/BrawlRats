using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlRats.Content.Choreography {

	public interface IChoreographyAction<S> where S : Scene {

		public bool TryStep(S scene, float delta);

	}

	public static class ChoreographyActions<S> where S : Scene {

		public static WaitFor<S> Wait(float seconds) => new(seconds);

		public static WaitFor<S> WaitUntil(Predicate<S> condition, float timeout = float.PositiveInfinity) => new(timeout, condition);

		public static Sequence<S> Sequence(IEnumerator<IChoreographyAction<S>> actions) => new(actions);

		public static RepeatWhile<S> RepeatWhile(Predicate<S> condition, IChoreographyAction<S> action) => new(condition, action);

		public static Group<S> Group(params IChoreographyAction<S>[] actions) => new(new List<IChoreographyAction<S>>(actions));

		public static Group<S> Group(IEnumerable<IChoreographyAction<S>> actions) => new(new List<IChoreographyAction<S>>(actions));

		public static Fade<S> FadeIn(float period) => new(true, 1.0f / period);

		public static Fade<S> FadeOut(float period) => new(false, 1.0f / period);

	}
	
	public class WaitFor<S> : IChoreographyAction<S> where S : Scene {

		public readonly float Delay;

		public float Accumulator { get; set; } = 0;

		public Predicate<S> BreakCondition { get; }

		public WaitFor(float seconds, Predicate<S> breakCond = null) {
			Delay = seconds;
			BreakCondition = breakCond;
		}

		public bool TryStep(S scene, float delta) {
			if ((BreakCondition?.Invoke(scene)).GetValueOrDefault(false)) return true;
			Accumulator += delta;
			return Accumulator >= Delay;
		}

	}

	public class Sequence<S> : IChoreographyAction<S> where S : Scene {

		private readonly IEnumerator<IChoreographyAction<S>> actions;
		private IChoreographyAction<S> current;

		public Sequence(IEnumerator<IChoreographyAction<S>> actions) {
			this.actions = actions;
		}

		public bool TryStep(S scene, float delta) {
			if (current != null && current.TryStep(scene, delta)) current = null;
			if (current == null) {
				if (!actions.MoveNext()) return true;
				else current = actions.Current;
			}
			return false;
		}

	}

	public class RepeatWhile<S> : IChoreographyAction<S> where S : Scene {

		public Predicate<S> Condition { get; }

		public IChoreographyAction<S> Action { get; }

		public RepeatWhile(Predicate<S> cond, IChoreographyAction<S> action) {
			Condition = cond;
			Action = action;
		}

		public bool TryStep(S scene, float delta) {
			if (!Condition(scene)) return true;
			Action.TryStep(scene, delta);
			return false;
		}
	}

	public class Group<S> : IChoreographyAction<S> where S : Scene {

		public IReadOnlyList<IChoreographyAction<S>> Actions;

		public Group(IReadOnlyList<IChoreographyAction<S>> actions) {
			Actions = actions;
		}

		public bool TryStep(S scene, float delta) {
			bool stepping = true;
			foreach (IChoreographyAction<S> action in Actions) if (!action.TryStep(scene, delta)) stepping = false;
			return stepping;
		}

	}

	public class Fade<S> : IChoreographyAction<S> where S : Scene {

		public bool Direction { get; }

		public float FadeStep { get; }

		public Fade(bool direction, float step) {
			Direction = direction;
			FadeStep = step;
		}

		public bool TryStep(S scene, float delta) {
			if (Direction) {
				return (scene.VFX.Opacity += FadeStep) == 1;
			} else {
				return (scene.VFX.Opacity -= FadeStep) == 0;
			}
		}
	}

}
