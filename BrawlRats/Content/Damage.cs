using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BrawlRats.Content {

	/// <summary>
	/// Enumeration of types of damage.
	/// </summary>
	public enum DamageType {
		/// <summary>
		/// No-damage type, used as a placeholder to indicate no damage.
		/// </summary>
		None = 0,

		// 5E-styled damage types

		Slashing,
		Piercing,
		Bludgeoning,
		Acid,
		Fire,
		Cold,
		Radiant,
		Lightning,
		Thunder,
		Force,
		Psychic
	}

	/// <summary>
	/// Damage information.
	/// </summary>
	public struct Damage {

		/// <summary>
		/// Damage value for no damage, set to default.
		/// </summary>
		public static readonly Damage None = default;

		/// <summary>
		/// The type of damage.
		/// </summary>
		public DamageType Type;

		/// <summary>
		/// The amount of damage.
		/// </summary>
		public float Amount;

		/// <summary>
		/// A directional force to apply as part of the damage.
		/// </summary>
		public Vector2 Force;

		public static implicit operator bool(Damage dmg) => dmg.Type != DamageType.None;

	}

	/// <summary>
	/// Per-object statistics related to damage management.
	/// </summary>
	public class DamageStats {

		/// <summary>
		/// The maximum amount of health.
		/// </summary>
		public float MaxHealth { get; init; }

		private float health;
		/// <summary>
		/// The current amount of health.
		/// </summary>
		public float Health { 
			get => health;
			set {
				if (value < 0) health = 0;
				else health = value;
			}
		}

		/// <summary>
		/// If the statistics indicate death (ie. health is 0).
		/// </summary>
		public bool IsDead => Health == 0;


		private Dictionary<DamageType, float> dmgmods = null;
		/// <summary>
		/// The set of damage modifiers indexed by damage type.
		/// </summary>
		public IDictionary<DamageType, float> DamageMods {
			get {
				if (dmgmods == null) dmgmods = new();
				return dmgmods;
			}
		}

		/// <summary>
		/// Gets the damage mod for a given type of damage, returning 1.0 if
		/// no mod is specified.
		/// </summary>
		/// <param name="type">The damage type</param>
		/// <returns>The modifier for the damage type</returns>
		public float GetDamageMod(DamageType type) {
			if (type == DamageType.None) return 0;
			else if (dmgmods == null) return 1;
			else if (dmgmods.TryGetValue(type, out float mod)) return mod;
			else return 1;
		}


		/// <summary>
		/// Tests if these statistics can take the given damage. This is equivalent
		/// to testing if the total damage that would be applied is 0 using the normal
		/// damage computation.
		/// </summary>
		/// <param name="dmg">Damage to test</param>
		/// <returns>If the damage can be taken</returns>
		public bool CanTakeDamage(Damage dmg) => (GetDamageMod(dmg.Type) * dmg.Amount) == 0;

		/// <summary>
		/// Applies damage to the statistics, decrementing health to the 
		/// </summary>
		/// <param name="dmg"></param>
		public void DoDamage(Damage dmg) {
			float mod = GetDamageMod(dmg.Type);
			Health -= dmg.Amount * mod;
		}


		/// <summary>
		/// Clones the damage statistics.
		/// </summary>
		/// <returns>Cloned damage statistics</returns>
		public DamageStats Clone() {
			return new DamageStats() {
				MaxHealth = MaxHealth,
				health = health,
				dmgmods = dmgmods
			};
		}

	}

	/// <summary>
	/// Hitbox information.
	/// </summary>
	public struct HitboxInfo {

		/// <summary>
		/// The damage to apply if hit by this hitbox.
		/// </summary>
		public Damage Damage;

		/// <summary>
		/// A force to apply to objects hit by this hitbox.
		/// </summary>
		public Vector2 DamageForce;

	}

	/// <summary>
	/// Enumeration of hurtbox IDs.
	/// </summary>
	public enum HurtboxID {
		/// <summary>
		/// Undefined hurtbox used when the specific type of hurtbox is not known.
		/// </summary>
		Undefined = 0,

		Head,
		Torso,
		LeftArm,
		RightArm,
		LeftLeg,
		RightLeg
	}

	/// <summary>
	/// Hurtbox information.
	/// </summary>
	public struct HurtboxInfo {

		/// <summary>
		/// The ID of the hurtbox.
		/// </summary>
		public HurtboxID ID;

	}

}
