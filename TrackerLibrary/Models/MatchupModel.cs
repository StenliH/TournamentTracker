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
		// TODO - do I need this??

		/// <summary>
		/// List of teams involved in this matchup.
		/// </summary>
		public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();

		/// <summary>
		/// The ID from the database that will be used to identify the winner.
		/// </summary>
		public int WinnerId	{ get; set; }

		/// <summary>
		/// Team ho won this round.
		/// </summary>
		private TeamModel _Winner;
		public TeamModel Winner
		{
			get { return _Winner; }
			set
			{
				_Winner = value;
				if (_Winner != null)
				{
				WinnerId = _Winner.Id;
				}
			}
		}

		/// <summary>
		/// Number of round in which this matchup belongs.
		/// </summary>
		public int MatchupRound { get; set; }

		/// <summary>
		/// Name of matchup to be displayed in a list of matchups on TournamentViewerForm.
		/// </summary>
		public string DisplayName
		{
			get
			{
				string output = "";
				foreach (MatchupEntryModel me in Entries)
				{
					if (me.TeamCompeting != null)
					{
						if (output.Length == 0)
						{
							output = me.TeamCompeting.TeamName;
						}
						else
						{
							output += $" vs. { me.TeamCompeting.TeamName }";
						} 
					}
					else
					{
						output = "?? vs. ?? (not yet determined)";
						break;
					}
				}

				return output;
			}
		}
	}
}
