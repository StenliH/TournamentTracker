using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;


namespace TrackerLibrary
{
	public static class TournamentLogic
	{
		// TODO - go through
		public static void CreateRounds(TournamentModel tournament)
		{
			// create matchups from first round and put them in the list
			List<TeamModel> randomizedTeams = RandomizeList(tournament.EnteredTeams);
			int rounds = FindNumberOfRounds(randomizedTeams.Count);
			int byes = NumberOfByes(randomizedTeams.Count, rounds);

			tournament.Rounds.Add(CreateFirstRound(randomizedTeams, byes));

			CreateOtherRounds(tournament, rounds);
		}

		private static void CreateOtherRounds(TournamentModel model, int numberOfRounds)
		{
			int round = 2;
			List<MatchupModel> previousRound = model.Rounds[0];
			List<MatchupModel> currRound = new List<MatchupModel>();
			MatchupModel currMatchup = new MatchupModel();

			while (round <= numberOfRounds)
			{
				foreach (MatchupModel match in previousRound)
				{
					currMatchup.Entries.Add(new MatchupEntryModel { ParentMatchup = match });

					if (currMatchup.Entries.Count > 1)
					{
						currMatchup.MatchupRound = round;
						currRound.Add(currMatchup);
						currMatchup = new MatchupModel();
					}
				}

				model.Rounds.Add(currRound);
				previousRound = currRound;

				currRound = new List<MatchupModel>();
				round++;

			}
		}

		private static List<MatchupModel> CreateFirstRound(List<TeamModel> realTeams, int numberOfByes)
		{
			List<MatchupModel> output = new List<MatchupModel>();
			MatchupModel current = new MatchupModel();

			foreach (var team in realTeams)
			{
				current.Entries.Add(new MatchupEntryModel { TeamCompeting = team });

				if (numberOfByes > 0 || current.Entries.Count > 1)
				{
					current.MatchupRound = 1;
					output.Add(current);
					current = new MatchupModel();

					if (numberOfByes > 0)
						numberOfByes--;
				}
			}

			return output;
		}

		private static int FindNumberOfRounds(int teamCount)
		{
			int output = 1;
			int val = 2;

			while (val < teamCount)
			{
				output += 1;
				val *= 2;
			}

			return output;
		}

		private static int NumberOfByes(int realTeamsCount, int numberOfRounds)
		{
			int output = 0;
			int totalTeams = 1;

			for (int i = 0; i < numberOfRounds; i++)
			{
				totalTeams *= 2;
			}

			output = totalTeams - realTeamsCount;

			return output;
		}

		private static List<TeamModel> RandomizeList(List<TeamModel> list)
		{
			List<TeamModel> output = new List<TeamModel>(list);

			Random random = new Random();

			for (int i = 0; i < output.Count; i++)
			{
				var r = random.Next(0, output.Count);

				var value = output[i];
				output[i] = output[r];
				output[r] = value;
			}

			return output;
		}
	}
}
