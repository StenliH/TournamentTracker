using System.Collections.Generic;

namespace TrackerLibrary.Models
{
	public class MatchupModel
	{
		/// <summary>
		/// List of teams involved in this matchup.
		/// </summary>
		public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();

		/// <summary>
		/// Team ho won this round.
		/// </summary>
		public TeamModel Winner { get; set; }

		/// <summary>
		/// Number of round in which this matchup belongs.
		/// </summary>
		public int MatchupRound { get; set; }
	}
}
