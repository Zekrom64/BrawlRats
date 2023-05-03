using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using BrawlRats.Physics;

namespace BrawlRats.Content.Characters {
	
	public class CharacterDef {
		
		/// <summary>
		/// The size of the character, used to make their dynamic collision box.
		/// </summary>
		public Vector2 Size;

		/// <summary>
		/// The "base" faction that this character is built to be part of. The true faction of
		/// a character may be overridden during gameplay.
		/// </summary>
		public FactionID BaseFaction = FactionID.None;

		/// <summary>
		/// The damage stats for the character.
		/// </summary>
		public DamageStats DamageStats = new();

	}

	public abstract class Character : Entity {

		public CharacterDef Def { get; }

		/// <summary>
		/// The faction override to apply to the character during gameplay. If null the base
		/// faction from the character definition is used.
		/// </summary>
		public FactionID? FactionOverride { get; set; }

		/// <summary>
		/// The current faction this character belongs to. Factions determine what groups are allowed
		/// to damage each other.
		/// </summary>
		public FactionID Faction {
			get {
				if (FactionOverride != null) return FactionOverride.Value;
				else return Def.BaseFaction;
			}
		}

		public Character(CharacterDef def) {
			Def = def;
			DamageStats = def.DamageStats.Clone();
		}

		public override bool CanBeHit(HitInfo info) {
			// Only characters in different factions can hit each other
			if (info.Hitter is Character c) {
				return Faction != c.Faction;
			} else return true;
		}

	}
}
