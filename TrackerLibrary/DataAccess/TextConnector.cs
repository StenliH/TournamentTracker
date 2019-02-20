using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.DataAccess.TextHelpers;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
	public class TextConnector : IDataConnection
	{
		private const string PrizesFile = "PrizeModels.csv";
		private const string PeopleFile = "PersonModels.csv";

		/// <summary>
		/// Saves a new person to the text file.
		/// </summary>
		/// <param name="model">The person information.</param>
		/// <returns></returns>
		public PersonModel CreatePerson(PersonModel model)
		{
			// Load file
			// convert to List<PersonModel>
			List<PersonModel> people = PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();

			// get id
			int currentId = 1;
			if (people.Any())
				currentId = people.OrderByDescending(x => x.Id).First().Id + 1;

			model.Id = currentId;

			// add record to the List<PersonModel>
			people.Add(model);

			// convert to text
			// save to file
			people.SaveToFile(PeopleFile);

			return model;
		}

		/// <summary>
		/// Saves a new prize to the text file.
		/// </summary>
		/// <param name="model">The prize information.</param>
		/// <returns>The prize information, including the unique identifier.</returns>
		public PrizeModel CreatePrize(PrizeModel model)
		{
			// Load the text file
			// Convert the text to List<PrizeModel>
			List<PrizeModel> prizes = PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();

			// Find the ID
			int currentId = 1;
			if (prizes.Any())
				currentId = prizes.OrderByDescending(x => x.Id).First().Id + 1;

			model.Id = currentId;

			// Add the new record with the new ID (max + 1)
			prizes.Add(model);

			// Convert the prizes to List<string>
			// Save the List<string> to the text file
			prizes.SaveToFile(PrizesFile);

			return model;
		}

		public TeamModel CreateTeam(TeamModel model)
		{
			throw new NotImplementedException();
		}

		public void AssignPeopleToTeam(TeamModel model)
		{
			throw new NotImplementedException();
		}

		public List<PersonModel> GetPerson_All()
		{
			return PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();
		}
	}

}
