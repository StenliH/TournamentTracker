using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
	public class TeamModel
	{
		/// <summary>
		/// Unique database identifier of the team.
		/// </summary>
		public int Id { get; set; }

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
