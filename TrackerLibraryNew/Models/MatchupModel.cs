using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibraryNew.Models
{
	class MatchupModel
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
