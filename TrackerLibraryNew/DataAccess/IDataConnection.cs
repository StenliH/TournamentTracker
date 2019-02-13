using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibraryNew.Models;


namespace TrackerLibraryNew.DataAccess
{
	public interface IDataConnection
	{
		PrizeModel CreatePrize(PrizeModel model);

	}
}
