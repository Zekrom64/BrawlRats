using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Box2DSharp.Collision;
using Box2DSharp.Collision.Collider;
using Box2DSharp.Collision.Shapes;
using Box2DSharp.Common;
using Box2DSharp.Dynamics;
using Box2DSharp.Dynamics.Contacts;
using BrawlRats.Util;
using Tesseract.Core.Math;

namespace BrawlRats.Content {

	public static class Shapes {

		public static PolygonShape Box(float width, float height, Vector2 center = default, float angle = 0) {
			var shape = new PolygonShape();
			shape.SetAsBox(width, height, center, MathUtil.ToRadians(angle));
			return shape;
		}

		public static PolygonShape Box(float width, float height, float angle = 0) {
			var shape = new PolygonShape();
			shape.SetAsBox(width, height, Vector2.Zero, MathUtil.ToRadians(angle));
			return shape;
		}
		
		public static PolygonShape RegularPolygon(float radius, int sides, Vector2 center = default, float angle = 0) {
			if (sides < 3) throw new ArgumentException("Polygon must have at least 3 sides");
			float step = MathUtil.TwoPi / sides;
			Vector2[] points = new Vector2[sides];
			for(int i = 0; i < sides; i++) {
				points[i] = center + new Vector2(MathF.Sin(angle), MathF.Cos(angle)) * radius;
				angle += step;
			}
			var shape = new PolygonShape();
			shape.Set(points);
			return shape;
		}

		public static CircleShape Circle(float radius) => new() { Radius = radius };

		public static ChainShape Chain(Vector2[] points) {
			if (points.Length < 3) throw new ArgumentException("Chain must have at least 3 points");
			var shape = new ChainShape();
			shape.CreateChain(points[1..^1], points.Length - 2, points[0], points[^0]);
			return shape;
		}

		public static ChainShape Chain(float segmentLength, int segments) {
			if (segments < 2) throw new ArgumentException("Chain must have at least 2 segments");
			Vector2[] points = new Vector2[segments + 1];
			points[0] = Vector2.Zero;
			for (int i = 0; i < segments; i++) points[i + 1] = points[i] - new Vector2(0, segmentLength);
			return Chain(points);
		}

		public static ChainShape ChainLoop(Vector2[] points) {
			if (points.Length < 3) throw new ArgumentException("Chain must have at least 3 points");
			var shape = new ChainShape();
			shape.CreateLoop(points);
			return shape;
		}

		public static ChainShape ChainLoop(float radius, int segments, float angle = 0) {
			if (segments < 3) throw new ArgumentException("Chain loop must have at least 3 segments");
			Vector2[] points = new Vector2[segments];
			float step = MathUtil.TwoPi / segments;
			for(int i = 0; i < segments; i++) {
				points[i] = new Vector2(MathF.Sin(angle), MathF.Cos(angle)) * radius;
				angle += step;
			}
			return ChainLoop(points);
		}

		public static EdgeShape Edge(Vector2 v1, Vector2 v2) {
			EdgeShape shape = new();
			shape.SetTwoSided(v1, v2);
			return shape;
		}

	}

	/// <summary>
	/// Bitmask of collision categories.
	/// </summary>
	[Flags]
	public enum CollisionMask : ushort {
		/// <summary>
		/// Scene terrain category.
		/// </summary>
		SceneTerrain = 0x0001,

		/// <summary>
		/// Player category.
		/// </summary>
		Players = 0x0002,
		/// <summary>
		/// Object category.
		/// </summary>
		Objects = 0x0004
	}

	/// <summary>
	/// Enumeration of collision groups
	/// </summary>
	public enum CollisionGroup : short {
		Default = 0
	}

	/// <summary>
	/// Physics fixture definition.
	/// </summary>
	public class PhysicsFixtureDef {

		/// <summary>
		/// The density of the fixture.
		/// </summary>
		public float Density = 1.0f;

		// Default collides as an object
		/// <summary>
		/// The bitmask of collision categories this fixture belongs to.
		/// </summary>
		public CollisionMask CollisionCategory = CollisionMask.Objects;

		// Default collides with objects and terrain
		/// <summary>
		/// The bitmask of collision categories this fixture will collide with.
		/// </summary>
		public CollisionMask CollisionMask = CollisionMask.SceneTerrain | CollisionMask.Objects;

		// Default collision group (no group specificity)
		/// <summary>
		/// The collision group this fixture is a part of.
		/// </summary>
		public CollisionGroup CollisionGroup = CollisionGroup.Default;

		/// <summary>
		/// If the fixture is a sensor, ie. it only detects collision and does not apply it.
		/// </summary>
		public bool IsSensor = false;

		// "Bounciness", default no bounce
		/// <summary>
		/// The restitution, or "bounciness" of the fixture.
		/// </summary>
		public float Restitution = 0.0f;

		/// <summary>
		/// The shape of the fixture.
		/// </summary>
		public Shape Shape;

		// Coefficient of friction, default regular friction
		/// <summary>
		/// The coefficient of friction to apply.
		/// </summary>
		public float Friction = 1.0f;

		// Threshold in speed above which restitution is applied
		/// <summary>
		/// The velocity threshold above which to apply <see cref="Restitution"/> to collisions.
		/// </summary>
		public float RestitutionThreshold = 0.0f;

		public static implicit operator FixtureDef([NotNull] PhysicsFixtureDef def) => new() {
			Density = def.Density,
			Filter = new() {
				CategoryBits = (ushort)def.CollisionCategory,
				MaskBits = (ushort)def.CollisionMask,
				GroupIndex = (short)def.CollisionGroup
			},
			IsSensor = def.IsSensor,
			Restitution = def.Restitution,
			Shape = def.Shape,
			Friction = def.Friction,
			RestitutionThreshold = def.RestitutionThreshold
		};

	}

	/// <summary>
	/// Physics body definition.
	/// </summary>
	public class PhysicsBodyDef {

		/// <summary>
		/// The list of fixtures that are part of this body.
		/// </summary>
		[NotNull]
		public PhysicsFixtureDef[] Fixtures;

		/// <summary>
		/// The initial angle of the body.
		/// </summary>
		public float InitialAngle = 0;

		/// <summary>
		/// Angular dampening to apply to the body.
		/// </summary>
		public float AngularDampening = 0.0f;

		/// <summary>
		/// The initial angular velocity of the body.
		/// </summary>
		public float InitialAngularVelocity = 0.0f;

		/// <summary>
		/// The type of body.
		/// </summary>
		public BodyType BodyType = BodyType.DynamicBody;

		/// <summary>
		/// If the body is a "bullet" which should be simulated more carefully for collisions.
		/// </summary>
		public bool IsBullet = false;

		/// <summary>
		/// If the rotation of a body is fixed, ie. it cannot rotate.
		/// </summary>
		public bool HasFixedRotation = true;

		/// <summary>
		/// Dampening to apply to linear motion.
		/// </summary>
		public float LinearDampening = 0.0f;

		/// <summary>
		/// The initial velocity of the body.
		/// </summary>
		public Vector2 InitialVelocity = Vector2.Zero;

		/// <summary>
		/// The initial position of the body.
		/// </summary>
		public Vector2 InitialPosition = Vector2.Zero;

		/// <summary>
		/// If the body has physics enabled.
		/// </summary>
		public bool Enabled = true;

		/// <summary>
		/// If the body is allowed to enter a "sleep" state if it is not moving.
		/// </summary>
		public bool AllowSleep = true;

		/// <summary>
		/// If the body is currently "awake".
		/// </summary>
		public bool Awake = true;

		/// <summary>
		/// Scale applied to gravity for the body.
		/// </summary>
		public float GravityScale = 1.0f;

		/// <summary>
		/// The collidable group of hitboxes.
		/// </summary>
		[MaybeNull]
		public ICollidable<HitboxInfo> Hitboxes = null;

		/// <summary>
		/// The collidable group of hurtboxes.
		/// </summary>
		[MaybeNull]
		public ICollidable<HurtboxInfo> Hurtboxes = null;

		public static implicit operator BodyDef([NotNull] PhysicsBodyDef def) => new() {
			Angle = def.InitialAngle,
			AngularDamping = def.AngularDampening,
			AngularVelocity = def.InitialAngularVelocity,
			BodyType = def.BodyType,
			Bullet = def.IsBullet,
			FixedRotation = def.HasFixedRotation,
			LinearDamping = def.LinearDampening,
			LinearVelocity = def.InitialVelocity,
			Position = def.InitialPosition,
			Enabled = def.Enabled,
			AllowSleep = def.AllowSleep,
			Awake = def.Awake,
			GravityScale = def.GravityScale
		};

	}

	/// <summary>
	/// Record for hit information.
	/// </summary>
	public record HitInfo {

		/// <summary>
		/// The controller doing the hitting.
		/// </summary>
		public IPhysicsController Hitter { get; init; }

		/// <summary>
		/// The hitbox that is hitting.
		/// </summary>
		public HitboxInfo Hitbox { get; init; }

		/// <summary>
		/// The controller being hit.
		/// </summary>
		public IPhysicsController Hurtee { get; init; }

		/// <summary>
		/// The hurtbax that is being hit.
		/// </summary>
		public HurtboxInfo Hurtbox { get; init; }

	}

	/// <summary>
	/// A physics controller manages how a physics body interacts with other bodies.
	/// </summary>
	public interface IPhysicsController {

		/// <summary>
		/// Called whenever the regular body controlled by this controller collides with another controller.
		/// </summary>
		/// <param name="other">Other controller</param>
		public void OnCollision(IPhysicsController other);

		/// <summary>
		/// Called whenever the regular body controlled by this controller stops colliding with another controller.
		/// </summary>
		/// <param name="other">Other controller</param>
		public void OnStopCollision(IPhysicsController other);

		/// <summary>
		/// Called whenever a hit is detected between this controller and another, but is only
		/// invoked for the first collision. Any subsequent overlap between colliders is ignored.
		/// </summary>
		/// <param name="hitter">The object causing the hit</param>
		/// <param name="hurtee">The object being hit</param>
		public void OnCompoundHit(IPhysicsController hitter, IPhysicsController hurtee);

		/// <summary>
		/// Called when a compound hit has ended.
		/// </summary>
		/// <param name="hitter">The object causing the hit</param>
		/// <param name="hurtee">The object being hit</param>
		public void OnStopCompoundHit(IPhysicsController hitter, IPhysicsController hurtee);

		/// <summary>
		/// Called whenever a hit is detected between this controller and another. This controller
		/// may be either the hitter or hurtee. This will be called for every intersection of collider,
		/// see <see cref="OnCompoundHit(IPhysicsController)"/> for detection only between whole bodies.
		/// </summary>
		/// <param name="info">Hit information</param>
		public void OnHit(HitInfo info);

		/// <summary>
		/// Called when an individual hit has ended.
		/// </summary>
		/// <param name="info"></param>
		public void OnStopHit(HitInfo info);

		/// <summary>
		/// Tests if the object can be hit by the given hit information. This is only tested on the
		/// hurtee side, but controls the hit for both entities.
		/// </summary>
		/// <param name="info">Hit information</param>
		/// <returns>If this object can be hit using the given information</returns>
		public bool CanBeHit(HitInfo info) => true;

	}

	/// <summary>
	/// A body is an object that can have physics applied to it. A body is composed of
	/// fixtures which determine the geometry that interacts physically. A body
	/// can also contain hitboxes and hurtboxes that can be tested for overlap.
	/// </summary>
	public class PhysicsBody : IDisposable {

		/// <summary>
		/// The physics this body belongs to.
		/// </summary>
		public Physics Physics { get; }

		/// <summary>
		/// The body associated with this instance.
		/// </summary>
		public Body Body { get; }

		private readonly List<Fixture> fixtures = new();
		/// <summary>
		/// The list of fixtures attached to this body.
		/// </summary>
		public IReadOnlyList<Fixture> Fixtures => fixtures;

		/// <summary>
		/// The controller that is controlling this body.
		/// </summary>
		public IPhysicsController Controller { get; }

		/// <summary>
		/// The collidable for this bodies hitbox.
		/// </summary>
		public ICollidable<HitboxInfo> Hitboxes { get; }

		/// <summary>
		/// The collidable for this bodies hurtbox.
		/// </summary>
		public ICollidable<HurtboxInfo> Hurtboxes { get; }

		/// <summary>
		/// The scalar inerta of this body.
		/// </summary>
		public float Intertia => Body.Inertia;

		/// <summary>
		/// The linear dampening applied to this body.
		/// </summary>
		public float LinearDampening {
			get => Body.LinearDamping;
			set => Body.LinearDamping = value;
		}

		/// <summary>
		/// The angular dampening applied to this body.
		/// </summary>
		public float AngularDampening {
			get => Body.AngularVelocity;
			set => Body.AngularDamping = value;
		}

		/// <summary>
		/// The mass of this body.
		/// </summary>
		public float Mass => Body.Mass;

		/// <summary>
		/// The type of this body.
		/// </summary>
		public BodyType BodyType => Body.BodyType;

		/// <summary>
		/// The linear velocity of this body.
		/// </summary>
		public Vector2 Velocity {
			get => Body.LinearVelocity;
			set => Body.SetLinearVelocity(value);
		}

		/// <summary>
		/// The angular velocity of this body.
		/// </summary>
		public float AngularVelocity {
			get => Body.AngularVelocity;
			set => Body.SetAngularVelocity(value);
		}

		/// <summary>
		/// If the body is allowed to sleep.
		/// </summary>
		public bool IsSleepingAllowed {
			get => Body.IsSleepingAllowed;
			set => Body.IsSleepingAllowed = value;
		}

		/// <summary>
		/// If the body is awake.
		/// </summary>
		public bool IsAwake {
			get => Body.IsAwake;
			set => Body.IsAwake = value;
		}

		/// <summary>
		/// If dynamic physics are enabled for this body.
		/// </summary>
		public bool IsEnabled {
			get => Body.IsEnabled;
			set => Body.IsEnabled = value;
		}
		
		/// <summary>
		/// If the body has a fixed rotation.
		/// </summary>
		public bool IsFixedRotation {
			get => Body.IsFixedRotation;
			set => Body.IsFixedRotation = value;
		}

		/// <summary>
		/// If the body is considered a "bullet" with more precise stepping.
		/// </summary>
		public bool IsBullet {
			get => Body.IsBullet;
			set => Body.IsBullet = value;
		}

		/// <summary>
		/// The rotational angle of the body.
		/// </summary>
		public float Angle {
			get => Body.GetAngle();
			set {
				Body.SetTransform(Position, value);
				aabbFlag = true;
			}
		}

		/// <summary>
		/// The position of the body.
		/// </summary>
		public Vector2 Position {
			get => Body.GetPosition();
			set => Body.SetTransform(value, Angle);
		}

		/// <summary>
		/// The transform of the body.
		/// </summary>
		public Transform Transform {
			get => Body.GetTransform();
			set {
				Body.SetTransform(value.Position, value.Rotation.Angle);
				aabbFlag = true;
			}
		}

		/// <summary>
		/// The mass data of the body.
		/// </summary>
		public MassData MassData {
			get {
				Body.GetMassData(out MassData data);
				return data;
			}
			set => Body.SetMassData(value);
		}

		public PhysicsBody([NotNull] Physics phys, [NotNull] PhysicsBodyDef def, [NotNull] IPhysicsController controller) {
			Physics = phys;
			Controller = controller;
			phys.bodies.Add(this);

			Body = phys.World.CreateBody(def);
			Body.UserData = this; // Userdata stores a reference to the containing structure
			foreach (var fixdef in def.Fixtures) fixtures.Add(Body.CreateFixture(fixdef));
		}

		public void Dispose() {
			GC.SuppressFinalize(this);
			Physics.World.DestroyBody(Body);
			Physics.bodies.Remove(this);
		}

		/// <summary>
		/// Adds a fixture to this body.
		/// </summary>
		/// <param name="def">Fixture definition</param>
		/// <returns>The added fixture</returns>
		public Fixture AddFixture([NotNull] PhysicsFixtureDef def) {
			Fixture fix = Body.CreateFixture(def);
			fixtures.Add(fix);
			aabbFlag = true;
			return fix;
		}
		
		/// <summary>
		/// Removes a fixture from this body.
		/// </summary>
		/// <param name="fix">Fixture to remove</param>
		public void RemoveFixture([NotNull] Fixture fix) {
			fixtures.Remove(fix);
			Body.DestroyFixture(fix);
			aabbFlag = true;
		}

		/// <summary>
		/// Applies an angular impulse to this body.
		/// </summary>
		/// <param name="impulse">Angular impulse</param>
		/// <param name="wake">If the impulse should wake up this body</param>
		public void ApplyAngularImpulse(float impulse, bool wake = true) => Body.ApplyAngularImpulse(impulse, wake);

		/// <summary>
		/// Applies a force to this body at the given point.
		/// </summary>
		/// <param name="force">Force to apply</param>
		/// <param name="point">Point to apply force at</param>
		/// <param name="wake">If the impulse should wake up this body</param>
		public void ApplyForce(in Vector2 force, in Vector2 point, bool wake = true) => Body.ApplyForce(force, point, wake);

		/// <summary>
		/// Applies a force to this body at its center.
		/// </summary>
		/// <param name="force">Force to apply</param>
		/// <param name="wake">If the impulse should wake up this body</param>
		public void ApplyForce(in Vector2 force, bool wake = true) => Body.ApplyForceToCenter(force, wake);

		// Compound hit record
		private struct CompoundHit {

			// Hitter & hurtee
			public IPhysicsController Hitter;
			public IPhysicsController Hurtee;

			// The number of hits contributing to this compound
			public int Count;

		}

		internal readonly List<HitInfo> newhits = new();
		private readonly HashSet<HitInfo> hits = new();
		private readonly List<CompoundHit> compoundHits = new();

		private void DecrementCompound(IPhysicsController hitter, IPhysicsController hurtee) {
			// If compound hit exists
			for(int i = 0; i < compoundHits.Count; i++) {
				CompoundHit hit = compoundHits[i];
				if (hit.Hitter == hitter && hit.Hurtee == hurtee) {
					// Decrement count and remove if zero
					if (--hit.Count <= 0) {
						compoundHits.RemoveAt(i);
						Controller.OnStopCompoundHit(hitter, hurtee);
					}
					break;
				}
			}
		}

		private void IncrementCompound(IPhysicsController hitter, IPhysicsController hurtee) {
			// If compound hit already exists increment count
			for(int i = 0; i < compoundHits.Count; i++) {
				CompoundHit hit = compoundHits[i];
				if (hit.Hitter == hitter && hit.Hurtee == hurtee) {
					hit.Count++;
					return;
				}
			}
			// Else add a new one
			compoundHits.Add(new CompoundHit() {
				Hitter = hitter,
				Hurtee = hurtee
			});
			Controller.OnCompoundHit(hitter, hurtee);
		}

		internal void UpdateHits() {
			// Remove any old hits
			hits.RemoveWhere(info => {
				if (!newhits.Contains(info)) {
					DecrementCompound(info.Hitter, info.Hurtee);
					Controller.OnStopHit(info);
					return true;
				} else return false;
			});
			// Add any new hits
			foreach(HitInfo hit in newhits) {
				if (!hits.Contains(hit)) {
					IncrementCompound(hit.Hitter, hit.Hurtee);
					Controller.OnHit(hit);
					hits.Add(hit);
				}
			}
		}

		public AABB ComputeAABB() {
			AABB bb = default;
			foreach(Fixture fixture in Fixtures) {
				fixture.Shape.ComputeAABB(out AABB shapeBB, Transform, 0);
				bb.Combine(shapeBB);
			}
			return bb;
		}

		private bool aabbFlag = true;
		private AABB cachedAABB;
		private float cachedBBAngle;

		public AABB AABB {
			get {
				// If rotatable and angle changes MUST update AABB
				if (!IsFixedRotation) {
					if (cachedBBAngle != Angle) aabbFlag = true;
				}
				// Only recompute if required
				if (aabbFlag) {
					cachedAABB = ComputeAABB();
					cachedBBAngle = Angle;
					aabbFlag = false;
					return cachedAABB;
				// Else only update based on positional changes
				} else {
					Vector2 size = cachedAABB.GetExtents();
					return new AABB(Position, size);
				}
			}
		}

	}

	/// <summary>
	/// A physics instance manages a world of bodies with the effects of physics applied to them.
	/// </summary>
	public class Physics : IContactListener {

		/// <summary>
		/// The time step to use for physics simulation.
		/// </summary>
		public const float TimeStep = 1.0f / 100.0f;

		/// <summary>
		/// The number of velocity iterations to apply when stepping the physics simulation.
		/// </summary>
		public const int VelocityIterations = 8;

		/// <summary>
		/// The number of position iterations to apply when stepping the physics simulation.
		/// </summary>
		public const int PositionIterations = 3;

		/// <summary>
		/// The world this physics instance controls.
		/// </summary>
		public World World { get; } = new();

		// Time stepping accumulator, used to make sure physics is stepped in constant time intervals
		private float timeAccum = 0;

		// The list of bodies in this physics world
		internal readonly List<PhysicsBody> bodies = new();

		public Physics() {
			World.SetContactListener(this);
		}

		private static void TestHit(PhysicsBody hitter, PhysicsBody hurtee) {
			if (hitter.Hitboxes != null && hurtee.Hurtboxes != null) {
				hitter.Hitboxes.GatherIntersects(hurtee.Hurtboxes, (HitboxInfo hit, HurtboxInfo hurt) => {
					HitInfo info = new() {
						Hitter = hitter.Controller,
						Hitbox = hit,
						Hurtee = hurtee.Controller,
						Hurtbox = hurt
					};
					// If hurtee can be hit
					if (hurtee.Controller.CanBeHit(info)) {
						// Add hits to each
						hitter.newhits.Add(info);
						hurtee.newhits.Add(info);
						// Add force to hurtee
						Vector2 force = hit.DamageForce;
						if (force.X != 0 && force.Y != 0) hurtee.ApplyForce(force);
					}
				}, hitter.Transform, hurtee.Transform);
			}
		}

		public void Update(UpdatePhase phase, float delta) {
			switch (phase) {
				case UpdatePhase.PhysicsStep:
					timeAccum += delta;
					// Step world using acquired time
					while (timeAccum > TimeStep) {
						World.Step(TimeStep, VelocityIterations, PositionIterations);
						timeAccum -= TimeStep;
					}
					break;
				case UpdatePhase.PhysicsCollide:
					// Reset the hit lists for each body
					foreach (PhysicsBody body in bodies) body.newhits.Clear();
					// Iterate each pair of bodies
					for(int i = 0; i < bodies.Count - 1; i++) {
						PhysicsBody body1 = bodies[i];
						for(int j = i + 1; j < bodies.Count; j++) {
							PhysicsBody body2 = bodies[j];
							// Test for hits from the first body to the second
							TestHit(body1, body2);
							// Test for hits from the second body to the first
							TestHit(body2, body1);
						}
					}
					// Update hit lists for each body
					foreach (PhysicsBody body in bodies) body.UpdateHits();
					break;
				default:
					break;
			}
		}

		public void BeginContact(Contact contact) {
			PhysicsBody body1 = (PhysicsBody)contact.FixtureA.Body.UserData;
			PhysicsBody body2 = (PhysicsBody)contact.FixtureB.Body.UserData;
			body1.Controller.OnCollision(body2.Controller);
			body2.Controller.OnCollision(body1.Controller);
		}

		public void EndContact(Contact contact) {
			PhysicsBody body1 = (PhysicsBody)contact.FixtureA.Body.UserData;
			PhysicsBody body2 = (PhysicsBody)contact.FixtureB.Body.UserData;
			body1.Controller.OnStopCollision(body2.Controller);
			body2.Controller.OnStopCollision(body1.Controller);
		}

		public void PreSolve(Contact contact, in Manifold oldManifold) { }

		public void PostSolve(Contact contact, in ContactImpulse impulse) { }

	}
}
