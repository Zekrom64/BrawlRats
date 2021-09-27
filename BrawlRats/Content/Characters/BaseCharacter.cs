using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

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

	}

	public abstract class BaseCharacter : Entity {

		[NotNull]
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

		public BaseCharacter([NotNull] CharacterDef def) {
			Def = def;
		}

	}
}
