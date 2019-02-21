using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;


namespace TrackerLibrary.DataAccess
{
	public interface IDataConnection
	{
		PersonModel CreatePerson(PersonModel model);
		PrizeModel CreatePrize(PrizeModel model);
		TeamModel CreateTeam(TeamModel model);
		List<PersonModel> GetPerson_All();
		List<TeamModel> GetTeam_All();
	}
}
