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
			List<MatchupModel> matchups = GlobalConfig.MatchupFile.FullFilePath().LoadFile().ConvertToMatchupModels();

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

				if (cols[4].Length > 0)
				{
					string[] prizesIds = cols[4].Split('|');

					foreach (var id in prizesIds)
					{
						model.Prizes.Add(prizes.Where(x => x.Id == int.Parse(id)).First());
					} 
				}

				// Capture information model.Rounds
				string[] rounds = cols[5].Split('|');

				foreach (string round in rounds)
				{
					string[] msText = round.Split('^');
					List<MatchupModel> ms = new List<MatchupModel>();

					foreach (string matchupModelTextId in msText)
					{
						ms.Add(matchups.Where(x => x.Id == int.Parse(matchupModelTextId)).First());
					}

					model.Rounds.Add(ms);
				}

				output.Add(model);
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
				lines.Add($"{ t.Id },{ t.TournamentName },{ t.EntryFee },{ ConvertTeamListToString(t.EnteredTeams) },{ ConvertPrizeListToString(t.Prizes) },{ ConvertRoundListToString(t.Rounds) }");
			}

			File.WriteAllLines(fileName.FullFilePath(), lines);
		}

		public static void SaveRoundsToFile(this TournamentModel model, string matchupFileName, string matchupEntryFileName)
		{
			// loop through each round
			// loop through each matchup
			// get the id for the new matchup and save the record
			// loop through each entry, get the id and save it

			foreach (List<MatchupModel> round in model.Rounds)
			{
				foreach (MatchupModel matchup in round)
				{
					// load all of the matchups from file
					// get the top id and add one
					// store the id
					// save the matchup record
					matchup.SaveMatchupToFile(matchupFileName, matchupEntryFileName);

					
				}
			}
		}

		private static void SaveMatchupToFile(this MatchupModel matchup, string matchupFileName, string matchupEntryFileName)
		{
			List<MatchupModel> matchups = GlobalConfig.MatchupFile.FullFilePath().LoadFile().ConvertToMatchupModels();

			int currentId = 1;

			if (matchups.Count > 0)
			{
				currentId = matchups.OrderByDescending(x => x.Id).First().Id + 1;
			}

			matchup.Id = currentId;

			matchups.Add(matchup);

			foreach (MatchupEntryModel entry in matchup.Entries)
			{
				entry.SaveEntryToFile(matchupEntryFileName);
			}

			// save to file
			List<string> lines = new List<string>();

			foreach (MatchupModel m in matchups)
			{
				string winner = "";
				if (m.Winner != null)
				{
					winner = m.Winner.Id.ToString();
				}

				lines.Add($"{ m.Id },{ ConvertMatchupEntryListToString(m.Entries) },{ winner },{ m.MatchupRound }");
			}

			File.WriteAllLines(GlobalConfig.MatchupFile.FullFilePath(), lines);
		}

		public static void UpdateMatchupToFile(this MatchupModel matchup)
		{
			List<MatchupModel> matchups = GlobalConfig.MatchupFile.FullFilePath().LoadFile().ConvertToMatchupModels();

			MatchupModel oldMatchup = new MatchupModel();
			foreach (MatchupModel m in matchups)
			{
				if (m.Id == matchup.Id)
				{
					oldMatchup = m;
				}
			}

			matchups.Remove(oldMatchup);

			matchups.Add(matchup);

			foreach (MatchupEntryModel entry in matchup.Entries)
			{
				entry.UpdateEntryToFile();
			}

			// save to file
			List<string> lines = new List<string>();

			foreach (MatchupModel m in matchups)
			{
				string winner = "";
				if (m.Winner != null)
				{
					winner = m.Winner.Id.ToString();
				}

				lines.Add($"{ m.Id },{ ConvertMatchupEntryListToString(m.Entries) },{ winner },{ m.MatchupRound }");
			}

			File.WriteAllLines(GlobalConfig.MatchupFile.FullFilePath(), lines);
		}

		public static void UpdateEntryToFile(this MatchupEntryModel entry)
		{
			List<MatchupEntryModel> entries = GlobalConfig.MatchupEntryFile.FullFilePath().LoadFile().ConvertToMatchupEntryModels();
			MatchupEntryModel oldEntry = new MatchupEntryModel();

			foreach (MatchupEntryModel e in entries)
			{
				if (e.Id == entry.Id)
				{
					oldEntry = e;
				}
			}

			entries.Remove(oldEntry);

			entries.Add(entry);

			List<string> lines = new List<string>();

			foreach (MatchupEntryModel e in entries)
			{
				string parent = "";

				if (e.ParentMatchup != null)
				{
					parent = e.ParentMatchup.Id.ToString();
				}

				string teamCompeting = "";
				if (e.TeamCompeting != null)
				{
					teamCompeting = e.TeamCompeting.Id.ToString();
				}

				lines.Add($"{ e.Id },{ teamCompeting },{ e.Score },{ parent }");
			}

			File.WriteAllLines(GlobalConfig.MatchupEntryFile.FullFilePath(), lines);
		}

		private static void SaveEntryToFile(this MatchupEntryModel entry, string matchupEntryFileName)
		{
			List<MatchupEntryModel> entries = GlobalConfig.MatchupEntryFile.FullFilePath().LoadFile().ConvertToMatchupEntryModels();

			int currentId = 1;

			if (entries.Count > 0)
			{
				currentId = entries.OrderByDescending(x => x.Id).First().Id + 1;
			}

			entry.Id = currentId;
			entries.Add(entry);

			List<string> lines = new List<string>();

			foreach (MatchupEntryModel e in entries)
			{
				string parent = "";

				if (e.ParentMatchup != null)
				{
					parent = e.ParentMatchup.Id.ToString();
				}

				string teamCompeting = "";
				if (e.TeamCompeting != null)
				{
					teamCompeting = e.TeamCompeting.Id.ToString();
				}

				lines.Add($"{ e.Id },{ teamCompeting },{ e.Score },{ parent }");
			}

			File.WriteAllLines(GlobalConfig.MatchupEntryFile.FullFilePath(), lines);
		}

		public static List<MatchupEntryModel> ConvertToMatchupEntryModels(this List<string> lines)
		{
			// id=0, TeamCompeting=1, Score=2, ParentMatchup=3
			List<MatchupEntryModel> output = new List<MatchupEntryModel>();

			foreach (string line in lines)
			{
				string[] cols = line.Split(',');

				MatchupEntryModel me = new MatchupEntryModel();
				me.Id = int.Parse(cols[0]);

				int teamCompeting = 0;
				if (int.TryParse(cols[1], out teamCompeting))
				{
					me.TeamCompeting = LookupTeamById(teamCompeting);
				}

				me.Score = double.Parse(cols[2]);

				int parentId = 0;
				if (int.TryParse(cols[3], out parentId))
				{
					me.ParentMatchup = LookupMatchupById(parentId); 
				}
				else
				{
					me.ParentMatchup = null;
				}

				output.Add(me);
			}

			return output;
		}

		private static MatchupModel LookupMatchupById(int id)
		{
			List<string> matchups = GlobalConfig.MatchupFile.FullFilePath().LoadFile();

			foreach (string matchup in matchups)
			{
				string[] cols = matchup.Split(',');
				if (cols[0] == id.ToString())
				{
					List<string> matchingMatchups = new List<string>();
					matchingMatchups.Add(matchup);
					return matchingMatchups.ConvertToMatchupModels().First();
				}
			}

			return null;
		}

		private static TeamModel LookupTeamById(int id)
		{
			List<string> teams = GlobalConfig.TeamsFile.FullFilePath().LoadFile();

			foreach (string team in teams)
			{
				string[] cols = team.Split(',');
				if (cols[0] == id.ToString())
				{
					List<string> matchingTeams = new List<string>();
					matchingTeams.Add(team);
					return matchingTeams.ConvertToTeamModels(GlobalConfig.PeopleFile).First();
				}
			}

			return null;
		}

		private static List<MatchupEntryModel> ConvertStringToMatchupEntryModels(string input)
		{
			string[] ids = input.Split('|');
			List<MatchupEntryModel> output = new List<MatchupEntryModel>();
			List<string> entries = GlobalConfig.MatchupEntryFile.FullFilePath().LoadFile();
			List<string> matchingEntries = new List<string>();

			foreach (string id in ids)
			{
				foreach (string entry in entries)
				{
					string[] cols = entry.Split(',');

					if (cols[0] == id)
					{
						matchingEntries.Add(entry);
					}
				}
			}

			output = matchingEntries.ConvertToMatchupEntryModels();

			return output;
		}

		public static List<MatchupModel> ConvertToMatchupModels(this List<string> lines)
		{
			// id=0, entries=1(pipe delimited by id), winner=2, matchupRound=3
			List<MatchupModel> output = new List<MatchupModel>();

			foreach (string line in lines)
			{
				string[] cols = line.Split(',');

				MatchupModel p = new MatchupModel();
				p.Id = int.Parse(cols[0]);
				p.Entries = ConvertStringToMatchupEntryModels(cols[1]);
				if (cols[2].Length == 0)
				{
					p.Winner = null;
				}
				else
				{
					p.Winner = LookupTeamById(int.Parse(cols[2]));
				}
				p.MatchupRound = int.Parse(cols[3]);

				output.Add(p);
			}

			return output;
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
			output = output.Remove(output.Length - 1);

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

		private static string ConvertMatchupEntryListToString(List<MatchupEntryModel> entries)
		{
			string output = "";

			if (entries.Count == 0)
				return "";

			foreach (MatchupEntryModel e in entries)
			{
				output += $"{ e.Id }|";
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
			output = output.Remove(output.Length - 1);

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
			output = output.Remove(output.Length - 1);

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
			output = output.Remove(output.Length - 1);

			return output;
		}

	}

		
}
