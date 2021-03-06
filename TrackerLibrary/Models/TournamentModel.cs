﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
	public class TournamentModel
	{
		/// <summary>
		/// The unique identifier for a tournament.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Name of the tournament.
		/// </summary>
		public string TournamentName { get; set; }

		/// <summary>
		/// Entry fee for the tournament.
		/// </summary>
		public decimal EntryFee { get; set; }

		/// <summary>
		/// List of all teams in the tournament.
		/// </summary>
		public List<TeamModel> EnteredTeams { get; set; } = new List<TeamModel>();

		/// <summary>
		/// List of prizes given to the winners of the tournament.
		/// </summary>
		public List<PrizeModel> Prizes { get; set; } = new List<PrizeModel>();

		/// <summary>
		/// List of all rounds in the tournament. One round consists of duel games - matchups.
		/// </summary>
		public List<List<MatchupModel>> Rounds { get; set; } = new List<List<MatchupModel>>();
	}
}
