using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerLibrary.Models
{
	public class TeamModel
	{
		/// <summary>
		/// List of persons who are members of this team.
		/// </summary>
		public List<PersonModel> TeamMembers { get; set; } = new List<PersonModel>();

		/// <summary>
		/// Name of the team.
		/// </summary>
		public string TeamName { get; set; }
	}
}
