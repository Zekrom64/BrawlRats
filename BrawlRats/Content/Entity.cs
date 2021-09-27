using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using BrawlRats.Util;

namespace BrawlRats.Content {

	public abstract class Entity : IUpdateable, IPhysicsController {

		// IPhysicsController

		public virtual void OnCollision(IPhysicsController other) { }

		public virtual void OnStopCollision(IPhysicsController other) { }

		public virtual void OnHit(HitInfo info) => DamageStats?.DoDamage(info.Hitbox.Damage);

		public virtual bool CanBeHit(HitInfo info) => true;

		// Entity

		public Vector2 Position {
			get => Physics.Body.GetPosition();
			set => Physics.Body.SetTransform(value, Physics.Body.GetAngle());
		}

		[NotNull]
		public PhysicsBody Physics { get; protected init; }

		[MaybeNull]
		public DamageStats DamageStats { get; protected init; }


		public virtual void Update(UpdatePhase phase, float delta) {
			
		}

	}

}
