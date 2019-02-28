using TrackerLibrary.Models;

namespace TrackerUI
{
	public interface IPrizeRequester
	{
		void GivePrize(PrizeModel model);
	}
}
