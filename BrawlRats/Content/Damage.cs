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

		/// <summary>
		/// Slashing damage; done by swords or cutting implements.
		/// </summary>
		Slashing,
		/// <summary>
		/// Piercing damage; done by bullets, spears, darts, etc.
		/// </summary>
		Piercing,
		/// <summary>
		/// Bludgeoning damage; done by clubs or blunt objects.
		/// </summary>
		Bludgeoning,
		/// <summary>
		/// Acid damage.
		/// </summary>
		Acid,
		/// <summary>
		/// Fire damage; done directly by fire or by excessive heat.
		/// </summary>
		Fire,
		/// <summary>
		/// Cold damage; done by extreme cold.
		/// </summary>
		Cold,
		/// <summary>
		/// Radiant damage; done by bright light or radiation.
		/// </summary>
		Radiant,
		/// <summary>
		/// Lighting damage; done by electrical shock.
		/// </summary>
		Lightning,
		/// <summary>
		/// Thunder damage; done by concussive blast waves.
		/// </summary>
		Thunder,
		/// <summary>
		/// Force damage; done by raw force such as being crushed.
		/// </summary>
		Force,
		/// <summary>
		/// Psychic damage; mental trauma that manifests as physical harm.
		/// </summary>
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


		private Dictionary<DamageType, float>? dmgmods = null;
		/// <summary>
		/// The set of damage modifiers indexed by damage type.
		/// </summary>
		public IDictionary<DamageType, float> DamageMods {
			get {
				dmgmods ??= new();
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

		// Body parts

		/// <summary>
		/// The head body part.
		/// </summary>
		Head,
		/// <summary>
		/// The torso body part.
		/// </summary>
		Torso,
		/// <summary>
		/// The left arm body part.
		/// </summary>
		LeftArm,
		/// <summary>
		/// The right arm body part.
		/// </summary>
		RightArm,
		/// <summary>
		/// The left leg body part.
		/// </summary>
		LeftLeg,
		/// <summary>
		/// The right leg body part.
		/// </summary>
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
