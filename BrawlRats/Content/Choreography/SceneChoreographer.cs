using System;
using System.Collections.Generic;

namespace BrawlRats.Content.Choreography {

	public interface ISceneChoreographer {

		public void Initialize();

		public void StepLogic(float delta);

	}

	public abstract class SceneChoreographer<S> : ISceneChoreographer where S : Scene {

		private readonly IEnumerator<IChoreographyAction<S>> actions;
		private IChoreographyAction<S> currentAction;

		public bool IsFinished { get; set; }

		public S Scene { get; }

		public SceneChoreographer(S scene) {
			Scene = scene;
			actions = RunScene(scene);
		}

		protected abstract void PrepareScene(S scene);

		protected abstract IEnumerator<IChoreographyAction<S>> RunScene(S scene);

		public void Initialize() => PrepareScene(Scene);

		public void StepLogic(float delta) {
			if (!IsFinished) {
				if (currentAction != null && currentAction.TryStep(Scene, delta)) currentAction = null;
				if (currentAction == null) {
					IsFinished = !actions.MoveNext();
					if (!IsFinished) currentAction = actions.Current;
				}
			}
		}


		protected static WaitFor<S> Wait(float seconds) => ChoreographyActions<S>.Wait(seconds);

		protected static WaitFor<S> WaitUntil(Predicate<S> condition, float timeout) => ChoreographyActions<S>.WaitUntil(condition, timeout);

		protected static Sequence<S> Sequence(IEnumerator<IChoreographyAction<S>> actions) => ChoreographyActions<S>.Sequence(actions);

		protected static RepeatWhile<S> RepeatWhile(Predicate<S> condition, IChoreographyAction<S> action) => ChoreographyActions<S>.RepeatWhile(condition, action);

		protected static Group<S> Group(params IChoreographyAction<S>[] actions) => ChoreographyActions<S>.Group(actions);

		protected static Group<S> Group(IEnumerable<IChoreographyAction<S>> actions) => ChoreographyActions<S>.Group(actions);

		protected static Fade<S> FadeIn(float period) => ChoreographyActions<S>.FadeIn(period);

		protected static Fade<S> FadeOut(float period) => ChoreographyActions<S>.FadeOut(period);

	}

}
