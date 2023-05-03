using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Box2D.NetStandard.Collision;
using Box2D.NetStandard.Collision.Shapes;
using Box2D.NetStandard.Common;
using Box2D.NetStandard.Dynamics;
using Box2D.NetStandard.Dynamics.Bodies;
using Box2D.NetStandard.Dynamics.Contacts;
using Box2D.NetStandard.Dynamics.Fixtures;
using Box2D.NetStandard.Dynamics.World;
using Box2D.NetStandard.Dynamics.World.Callbacks;
using BrawlRats.Content;
using Tesseract.Core.Numerics;

namespace BrawlRats.Physics {

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
		public required Shape Shape { get; init; }

		// Coefficient of friction, default regular friction
		/// <summary>
		/// The coefficient of friction to apply.
		/// </summary>
		public float Friction = 1.0f;

		public static implicit operator FixtureDef(PhysicsFixtureDef def) => new() {
			density = def.Density,
			filter = new() {
				categoryBits = (ushort)def.CollisionCategory,
				maskBits = (ushort)def.CollisionMask,
				groupIndex = (short)def.CollisionGroup
			},
			isSensor = def.IsSensor,
			restitution = def.Restitution,
			shape = def.Shape,
			friction = def.Friction
		};

	}

	/// <summary>
	/// Physics body definition.
	/// </summary>
	public class PhysicsBodyDef {

		/// <summary>
		/// The list of fixtures that are part of this body.
		/// </summary>
		public required PhysicsFixtureDef[] Fixtures { get; init; }

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
		public BodyType BodyType = BodyType.Dynamic;

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
		public ICollidable<HitboxInfo>? Hitboxes = null;

		/// <summary>
		/// The collidable group of hurtboxes.
		/// </summary>
		public ICollidable<HurtboxInfo>? Hurtboxes = null;

		public static implicit operator BodyDef(PhysicsBodyDef def) => new() {
			angle = def.InitialAngle,
			angularDamping = def.AngularDampening,
			angularVelocity = def.InitialAngularVelocity,
			type = def.BodyType,
			bullet = def.IsBullet,
			fixedRotation = def.HasFixedRotation,
			linearDamping = def.LinearDampening,
			linearVelocity = def.InitialVelocity,
			position = def.InitialPosition,
			enabled = def.Enabled,
			allowSleep = def.AllowSleep,
			awake = def.Awake,
			gravityScale = def.GravityScale
		};

	}

	/// <summary>
	/// Record for hit information.
	/// </summary>
	public record HitInfo {

		/// <summary>
		/// The controller doing the hitting.
		/// </summary>
		public required IPhysicsController Hitter { get; init; }

		/// <summary>
		/// The hitbox that is hitting.
		/// </summary>
		public required HitboxInfo Hitbox { get; init; }

		/// <summary>
		/// The controller being hit.
		/// </summary>
		public required IPhysicsController Hurtee { get; init; }

		/// <summary>
		/// The hurtbax that is being hit.
		/// </summary>
		public required HurtboxInfo Hurtbox { get; init; }

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
		/// The physics world this body belongs to.
		/// </summary>
		public PhysicsWorld Physics { get; }

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
		public ICollidable<HitboxInfo>? Hitboxes { get; }

		/// <summary>
		/// The collidable for this bodies hurtbox.
		/// </summary>
		public ICollidable<HurtboxInfo>? Hurtboxes { get; }

		/// <summary>
		/// The scalar inerta of this body.
		/// </summary>
		public float Intertia => Body.GetInertia();

		/// <summary>
		/// The linear dampening applied to this body.
		/// </summary>
		public float LinearDampening {
			get => Body.GetLinearDamping();
			set => Body.SetLinearDampling(value);
		}

		/// <summary>
		/// The angular dampening applied to this body.
		/// </summary>
		public float AngularDampening {
			get => Body.GetAngularVelocity();
			set => Body.SetAngularDamping(value);
		}

		/// <summary>
		/// The mass of this body.
		/// </summary>
		public float Mass => Body.GetMass();

		/// <summary>
		/// The type of this body.
		/// </summary>
		public BodyType BodyType => Body.Type();

		/// <summary>
		/// The linear velocity of this body.
		/// </summary>
		public Vector2 Velocity {
			get => Body.GetLinearVelocity();
			set => Body.SetLinearVelocity(value);
		}

		/// <summary>
		/// The angular velocity of this body.
		/// </summary>
		public float AngularVelocity {
			get => Body.GetAngularVelocity();
			set => Body.SetAngularVelocity(value);
		}

		/// <summary>
		/// If the body is allowed to sleep.
		/// </summary>
		public bool IsSleepingAllowed {
			get => Body.IsSleepingAllowed();
			set => Body.SetSleepingAllowed(value);
		}

		/// <summary>
		/// If the body is awake.
		/// </summary>
		public bool IsAwake {
			get => Body.IsAwake();
			set => Body.SetAwake(value);
		}

		/// <summary>
		/// If dynamic physics are enabled for this body.
		/// </summary>
		public bool IsEnabled {
			get => Body.IsEnabled();
			set => Body.SetEnabled(value);
		}
		
		/// <summary>
		/// If the body has a fixed rotation.
		/// </summary>
		public bool IsFixedRotation {
			get => Body.IsFixedRotation();
			set => Body.SetFixedRotation(value);
		}

		/// <summary>
		/// If the body is considered a "bullet" with more precise stepping.
		/// </summary>
		public bool IsBullet {
			get => Body.IsBullet();
			set => Body.SetBullet(value);
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
				Body.SetTransform(value.p, value.GetAngle());
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

		public PhysicsBody(PhysicsWorld phys, PhysicsBodyDef def, IPhysicsController controller) {
			Physics = phys;
			Controller = controller;
			phys.bodies.Add(this);

			Body = phys.World.CreateBody(def);
			Body.SetUserData(this);
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
	public class PhysicsWorld : ContactListener {

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

		public PhysicsWorld() {
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

		public void Update(float delta) {
			timeAccum += delta;
			// Step world using acquired time
			while (timeAccum > TimeStep) {
				World.Step(TimeStep, VelocityIterations, PositionIterations);
				timeAccum -= TimeStep;
			}
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
		}

		public override void BeginContact(in Contact contact) {
			PhysicsBody body1 = (PhysicsBody)contact.FixtureA.Body.UserData;
			PhysicsBody body2 = (PhysicsBody)contact.FixtureB.Body.UserData;
			body1.Controller.OnCollision(body2.Controller);
			body2.Controller.OnCollision(body1.Controller);
		}

		public override void EndContact(in Contact contact) {
			PhysicsBody body1 = (PhysicsBody)contact.FixtureA.Body.UserData;
			PhysicsBody body2 = (PhysicsBody)contact.FixtureB.Body.UserData;
			body1.Controller.OnStopCollision(body2.Controller);
			body2.Controller.OnStopCollision(body1.Controller);
		}

		public override void PreSolve(in Contact contact, in Manifold oldManifold) { }

		public override void PostSolve(in Contact contact, in ContactImpulse impulse) { }

	}
}
