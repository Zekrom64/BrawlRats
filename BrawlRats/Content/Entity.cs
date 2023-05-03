using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Box2D.NetStandard.Collision;
using BrawlRats.Physics;
using BrawlRats.Util;

namespace BrawlRats.Content {

	public abstract class Entity : IPhysicsController {

		// IPhysicsController

		public virtual void OnCollision(IPhysicsController other) { }

		public virtual void OnStopCollision(IPhysicsController other) { }

		public virtual void OnCompoundHit(IPhysicsController hitter, IPhysicsController hurtee) { }

		public virtual void OnStopCompoundHit(IPhysicsController hitter, IPhysicsController hurtee) { }

		public virtual void OnHit(HitInfo info) => DamageStats?.DoDamage(info.Hitbox.Damage);

		public virtual void OnStopHit(HitInfo info) { }

		public virtual bool CanBeHit(HitInfo info) => true;

		// Entity

		/// <summary>
		/// The scene that the entity is inside of.
		/// </summary>
		public Scene? Scene { get; internal set; }

		/// <summary>
		/// If the entity is loaded into a scene, allowing it to be simulated
		/// and drawn. Entities may exist outside of a scene but should not
		/// be used as their state will be invalid.
		/// </summary>
		public bool IsLoaded => Scene != null;

		private AABB boundingBox = default;

		public AABB BoundingBox {
			get => Physics?.AABB ?? boundingBox;
			set => boundingBox = value;
		}

		public Vector2 Position {
			get => Physics?.Body?.Position?? default;
			set => Physics?.Body?.SetTransform(value, Physics.Body.GetAngle());
		}

		public PhysicsBody? Physics { get; protected init; }

		public DamageStats? DamageStats { get; protected init; }


		protected abstract void DoLogic(float delta);

		public virtual void Update(float delta) {
			DoLogic(delta);
		}

	}

}
