using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess.TextHelpers
{
	public static class TextConnectorProcessor
	{
		public static string FullFilePath(this string fileName) // PrizeModels.csv
		{
			return $"{ ConfigurationManager.AppSettings["filePath"] }\\{ fileName }";
		}

		public static List<string> LoadFile(this string file)
		{
			if (!File.Exists(file))
			{
				return new List<string>();
			}

			return File.ReadAllLines(file).ToList();
		}

		public static List<PersonModel> ConvertToPersonModels(this List<string> lines)
		{
			List<PersonModel> output = new List<PersonModel>();

			foreach (string line in lines)
			{
				string[] cols = line.Split(',');

				PersonModel p = new PersonModel();

				p.Id = int.Parse(cols[0]);
				p.FirstName = cols[1];
				p.LastName = cols[2];
				p.EmailAddress = cols[3];
				p.CellphoneNumber = cols[4];

				output.Add(p);
			}

			return output;
		}

		public static List<PrizeModel> ConvertToPrizeModels(this List<string> lines)
		{
			List<PrizeModel> output = new List<PrizeModel>();

			foreach (string line in lines)
			{
				string[] cols = line.Split(',');

				PrizeModel p = new PrizeModel();
				p.Id = int.Parse(cols[0]);
				p.PlaceNumber = int.Parse(cols[1]);
				p.PlaceName = cols[2];
				p.PrizeAmount = decimal.Parse(cols[3]);
				p.PrizePercentage = double.Parse(cols[4]);

				output.Add(p);
			}

			return output;
		}

		public static List<TeamModel> ConvertToTeamModels(this List<string> lines, string peopleFileName)
		{
			List<TeamModel> output = new List<TeamModel>();

			// structure of TeamModel in TeamsFile:
			// Id, TeamName, PersonId|PersonId|..|..|....

			foreach (string line in lines)
			{
				string[] cols = line.Split(',');

				TeamModel t = new TeamModel();
				List<PersonModel> people = peopleFileName.FullFilePath().LoadFile().ConvertToPersonModels();

				t.Id = int.Parse(cols[0]);
				t.TeamName = cols[1];
				string[] PersonIds = cols[2].Split('|');

				foreach (string id in PersonIds)
				{
					t.TeamMembers.Add(people.Where(x => x.Id == int.Parse(id)).First());
				}

				output.Add(t);
			}

			return output;
		}

		public static List<TournamentModel> ConvertToTournamentModels(
			this List<string> lines, 
			string teamsFileName, 
			string peopleFileName, 
			string prizesFileName)
		{
			// id,tournamentName,entryFee,enteredTeams,Prizes,Rounds
			// 2,DeathMatch,20,(2|5|3),(1|2|3|4),(id^id^id|id^id^id|id^id^id)

			List<TournamentModel> output = new List<TournamentModel>();
			List<TeamModel> teams = teamsFileName.FullFilePath().LoadFile().ConvertToTeamModels(peopleFileName);
			List<PrizeModel> prizes = prizesFileName.FullFilePath().LoadFile().ConvertToPrizeModels();

			foreach (var line in lines)
			{
				string[] cols = line.Split(',');

				TournamentModel model = new TournamentModel();
				model.Id = int.Parse(cols[0]);
				model.TournamentName = cols[1];
				model.EntryFee = decimal.Parse(cols[2]);

				string[] teamsIds = cols[3].Split('|');
				foreach (var id in teamsIds)
				{
					model.EnteredTeams.Add(teams.Where(x => x.Id == int.Parse(id)).First());
				}

				string[] prizesIds = cols[4].Split('|');
				foreach (var id in prizesIds)
				{
					model.Prizes.Add(prizes.Where(x => x.Id == int.Parse(id)).First());
				}

				// TODO - capture model.Rounds
			}

			return output;
		}

		public static void SaveToFile(this List<PersonModel> persons, string fileName)
		{
			List<string> lines = new List<string>();

			foreach (PersonModel p in persons)
			{
				lines.Add($"{ p.Id },{ p.FirstName },{ p.LastName },{ p.EmailAddress },{ p.CellphoneNumber }");
			}

			File.WriteAllLines(fileName.FullFilePath(), lines);
		}

		public static void SaveToFile(this List<PrizeModel> prizes, string fileName)
		{
			List<string> lines = new List<string>();

			foreach (PrizeModel p in prizes)
			{
				lines.Add($"{ p.Id },{ p.PlaceNumber },{ p.PlaceName },{ p.PrizeAmount },{ p.PrizePercentage }");
			}

			File.WriteAllLines(fileName.FullFilePath(), lines);
		}

		public static void SaveToFile(this List<TeamModel> teams, string fileName)
		{
			List<string> lines = new List<string>();

			foreach (TeamModel t in teams)
			{
				string teamMembers = ConvertPersonListToString(t.TeamMembers);

				lines.Add($"{ t.Id },{ t.TeamName },{ teamMembers }");
			}

			File.WriteAllLines(fileName.FullFilePath(), lines);
		}

		public static void SaveToFile(this List<TournamentModel> tournaments, string fileName)
		{
			List<string> lines = new List<string>();

			foreach (var t in tournaments)
			{
				lines.Add($@"{ t.Id },
					{ t.TournamentName },
					{ t.EntryFee },
					{ ConvertTeamListToString(t.EnteredTeams) },
					{ ConvertPrizeListToString(t.Prizes) },
					{ ConvertRoundListToString(t.Rounds) }");
			}

			File.WriteAllLines(fileName.FullFilePath(), lines);
		}

		private static string ConvertTeamListToString(List<TeamModel> teams)
		{
			string output = "";

			if (teams.Count == 0)
				return "";

			foreach (var team in teams)
			{
				output += $"{ team.Id }|";
			}
			output.Remove(output.Length - 1);

			return output;
		}

		private static string ConvertPersonListToString(List<PersonModel> people)
		{
			string output = "";

			if (people.Count == 0)
				return "";

			foreach (PersonModel person in people)
			{
				output += $"{ person.Id }|";
			}
			output = output.Remove(output.Length - 1);

			return output;
		}

		private static string ConvertPrizeListToString(List<PrizeModel> prizes)
		{
			string output = "";

			if (prizes.Count == 0)
				return "";

			foreach (var prize in prizes)
			{
				output += $"{ prize.Id }|";
			}
			output.Remove(output.Length - 1);

			return output;
		}

		private static string ConvertRoundListToString(List<List<MatchupModel>> rounds)
		{
			string output = "";

			if (rounds.Count == 0)
				return "";

			foreach (var round in rounds)
			{
				output += $"{ ConvertMatchupListToString(round) }|";
			}
			output.Remove(output.Length - 1);

			return output;
		}

		private static string ConvertMatchupListToString(List<MatchupModel> matchups)
		{
			string output = "";

			if (matchups.Count == 0)
				return "";

			foreach (var matchup in matchups)
			{
				output += $"{ matchup.Id }^";
			}
			output.Remove(output.Length - 1);

			return output;
		}

	}
}
