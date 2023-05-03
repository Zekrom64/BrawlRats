using System;
using System.Collections.Generic;

namespace BrawlRats.Content.Choreography {

	/// <summary>
	/// Non-generic interface for scene choreographers. Instances should be derived
	/// from the abstract class <see cref="SceneChoreographer{S}"/> instead which
	/// implements the actual logic.
	/// </summary>
	public interface ISceneChoreographer {

		/// <summary>
		/// Initializes the choreographer.
		/// </summary>
		public void Initialize();

		/// <summary>
		/// Steps the choreography logic forward using the given timestep.
		/// </summary>
		/// <param name="delta">Amount of time to step forward</param>
		public void StepLogic(float delta);

	}

	/// <summary>
	/// A scene choreographer manages choreographic actions for a particular scene (intro,
	/// outro, intermissions, events).
	/// </summary>
	/// <typeparam name="S">The scene to perform choreography for</typeparam>
	public abstract class SceneChoreographer<S> : ISceneChoreographer where S : Scene {

		private readonly IEnumerator<IChoreographyAction<S>> actions;
		private IChoreographyAction<S> currentAction;

		/// <summary>
		/// If the choreographer is finished, indicating the scene has ended.
		/// </summary>
		public bool IsFinished { get; set; }

		/// <summary>
		/// The scene the choreographer manages.
		/// </summary>
		public S Scene { get; }

		/// <summary>
		/// Creates a new choreographer for the given scene.
		/// </summary>
		/// <param name="scene">Scene to choreograph</param>
		public SceneChoreographer(S scene) {
			Scene = scene;
			actions = RunScene().GetEnumerator();
		}

		/// <summary>
		/// Called once before the scene is run to initialize choreographer's <see cref="Scene"/> 
		/// to an initial state (such as character/entity placement or VFX).
		/// </summary>
		protected abstract void PrepareScene();

		/// <summary>
		/// Runs choreography for this scene by generating an enumerable value describing
		/// a list of choreography actions to perform. It is highly recommended to implement
		/// this using yield return statements.
		/// </summary>
		/// <returns>Enumeration of scene choreography actions</returns>
		protected abstract IEnumerable<IChoreographyAction<S>> RunScene();

		public void Initialize() => PrepareScene();

		public void StepLogic(float delta) {
			if (!IsFinished) {
				if (currentAction != null && currentAction.TryStep(Scene, delta)) currentAction = null;
				if (currentAction == null) {
					IsFinished = !actions.MoveNext();
					if (!IsFinished) currentAction = actions.Current;
				}
			}
		}


		/// <summary>
		/// Creates an action to wait for the given amount of time.
		/// </summary>
		/// <param name="seconds">Number of seconds to wait</param>
		/// <returns>Wait action</returns>
		protected static WaitFor<S> Wait(float seconds) => ChoreographyActions<S>.Wait(seconds);

		/// <summary>
		/// Creates an action to wait until either the given condition is true, or a timeout occurs.
		/// </summary>
		/// <param name="condition">Condition to wait for</param>
		/// <param name="timeout">Timeout value</param>
		/// <returns>Wait until action</returns>
		protected static WaitFor<S> WaitUntil(Predicate<S> condition, float timeout) => ChoreographyActions<S>.WaitUntil(condition, timeout);

		/// <summary>
		/// Creates a compound action that will execute a sequence of other actions.
		/// </summary>
		/// <param name="actions">Actions to perform in sequence</param>
		/// <returns>Sequence action</returns>
		protected static Sequence<S> Sequence(IEnumerator<IChoreographyAction<S>> actions) => ChoreographyActions<S>.Sequence(actions);

		/// <summary>
		/// Creates a repeating action that will continue while a condition is true.
		/// </summary>
		/// <param name="condition">Condition to repeat while true</param>
		/// <param name="action">Action to repeat</param>
		/// <returns>Repeat while action</returns>
		protected static RepeatWhile<S> RepeatWhile(Predicate<S> condition, IChoreographyAction<S> action) => ChoreographyActions<S>.RepeatWhile(condition, action);

		/// <summary>
		/// Creates a compound action that will execute a group of actions simultaneously, waiting until they are all complete.
		/// </summary>
		/// <param name="actions">Actions to perform as a group</param>
		/// <returns>Group action</returns>
		protected static Group<S> Group(params IChoreographyAction<S>[] actions) => ChoreographyActions<S>.Group(actions);

		/// <summary>
		/// Creates a compound action that will execute a group of actions simultaneously, waiting until they are all complete.
		/// </summary>
		/// <param name="actions">Actions to perform as a group</param>
		/// <returns>Group action</returns>
		protected static Group<S> Group(IEnumerable<IChoreographyAction<S>> actions) => ChoreographyActions<S>.Group(actions);

		/// <summary>
		/// Creates an action that will use VFX to fade the scene in over the given period.
		/// </summary>
		/// <param name="period">Fade in period</param>
		/// <returns>Fade in action</returns>
		protected static Fade<S> FadeIn(float period) => ChoreographyActions<S>.FadeIn(period);

		/// <summary>
		/// Creates an action that will use VFX to fade the scene out over the given period.
		/// </summary>
		/// <param name="period">Fade out period</param>
		/// <returns>Fade out action</returns>
		protected static Fade<S> FadeOut(float period) => ChoreographyActions<S>.FadeOut(period);

	}

}
