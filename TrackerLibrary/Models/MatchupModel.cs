using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
	public class MatchupModel
	{
		/// <summary>
		/// The unique identifier for a matchup.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// The id number of tournament to which this matchup belongs to.
		/// </summary>
		public int TournamentId { get; set; }

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
