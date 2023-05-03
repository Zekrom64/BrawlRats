using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract.Core.Collections;

namespace BrawlRats.Content {
	
	/// <summary>
	/// Enumeration of faction IDs.
	/// </summary>
	public enum FactionID {
		/// <summary>
		/// None/unaligned faction.
		/// </summary>
		None = 0,
		
		// "Generic" factions for arbitrary teams

		/// <summary>
		/// Generic faction #1.
		/// </summary>
		Generic1,
		/// <summary>
		/// Generic faction #2.
		/// </summary>
		Generic2,
		/// <summary>
		/// Generic faction #3.
		/// </summary>
		Generic3,
		/// <summary>
		/// Generic faction #4.
		/// </summary>
		Generic4,

		// "Official" factions for story use

		/// <summary>
		/// The party of sprawl rats.
		/// </summary>
		SprawlRats,
		/// <summary>
		/// The Malbulgian court.
		/// </summary>
		Malbulgians,
		/// <summary>
		/// O.N.I. forces.
		/// </summary>
		ONI
	}

	/// <summary>
	/// Additional information for a specific faction.
	/// </summary>
	public class FactionInfo {

		/// <summary>
		/// The faction this information is for.
		/// </summary>
		public required FactionID Faction { get; init; }

		/// <summary>
		/// The name of this instance of the faction.
		/// </summary>
		public required string Name { get; init; }

		/// <summary>
		/// The base set of enemies for the faction.
		/// </summary>
		public IReadOnlyList<FactionID> BaseEnemies { get; init; } = Collection<FactionID>.EmptyList;

	}

	/// <summary>
	/// Standard faction information.
	/// </summary>
	public static class Factions {

		/// <summary>
		/// Standard information for sprawl rats.
		/// </summary>
		public static readonly FactionInfo SprawlRats = new() {
			Faction = FactionID.SprawlRats,
			Name = "Sprawl Rats"
		};

		/// <summary>
		/// Standard information for Malbulgians.
		/// </summary>
		public static readonly FactionInfo Malbulgians = new() {
			Faction = FactionID.Malbulgians,
			Name = "The Malbulgian Court",
			BaseEnemies = new FactionID[] { FactionID.ONI }
		};

		/// <summary>
		/// Standard information for O.N.I. forces.
		/// </summary>
		public static readonly FactionInfo ONI = new() {
			Faction = FactionID.ONI,
			Name = "O.N.I. Forces",
			BaseEnemies = new FactionID[] { FactionID.Malbulgians }
		};

	}

	/// <summary>
	/// A context which specifies what factions exist and any special relations between them for this case.
	/// </summary>
	public class FactionContext {

		/// <summary>
		/// The list of factions in this context.
		/// </summary>
		public required IReadOnlyList<FactionInfo> Factions { get; init; }

		/// <summary>
		/// Truces which override any conflicts between factions for this scene.
		/// </summary>
		public IReadOnlyList<(FactionID, FactionID)> Truces { get; init; } = Collection<(FactionID, FactionID)>.EmptyList;

		/// <summary>
		/// Additional conflicts between factions for this scene.
		/// </summary>
		public IReadOnlyList<(FactionID, FactionID)> Conflicts { get; init; } = Collection<(FactionID, FactionID)>.EmptyList;

	}

}
