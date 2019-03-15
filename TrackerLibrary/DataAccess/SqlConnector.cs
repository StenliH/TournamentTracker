using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
	public class SqlConnector : IDataConnection
	{
		private const string sqlDatabaseName = "Tournaments";

		/// <summary>
		/// Saves a new person to a database.
		/// </summary>
		/// <param name="model">The information about person.</param>
		/// <returns></returns>
		public PersonModel CreatePerson(PersonModel model)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(sqlDatabaseName)))
			{
				DynamicParameters p = new DynamicParameters();
				p.Add("@FirstName", model.FirstName);
				p.Add("@LastName", model.LastName);
				p.Add("@EmailAddress", model.EmailAddress);
				p.Add("@CellphoneNumber", model.CellphoneNumber);
				p.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

				connection.Execute("dbo.spPeople_Insert", p, commandType: CommandType.StoredProcedure);

				model.Id = p.Get<int>("@id");
			}

			return model;
		}

		/// <summary>
		/// Saves a new prize to a database.
		/// </summary>
		/// <param name="model">The prize information.</param>
		/// <returns>The prize information, including the unique identifier.</returns>
		public PrizeModel CreatePrize(PrizeModel model)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(sqlDatabaseName)))
			{
				DynamicParameters p = new DynamicParameters();
				p.Add("@PlaceNumber", model.PlaceNumber);
				p.Add("@PlaceName", model.PlaceName);
				p.Add("@PrizeAmount", model.PrizeAmount);
				p.Add("@PrizePercentage", model.PrizePercentage);
				p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

				connection.Execute("dbo.spPrizes_Insert", p, commandType: CommandType.StoredProcedure);

				model.Id = p.Get<int>("@id");

				return model;
			}
		}

		public TeamModel CreateTeam(TeamModel model)
		{
			// save Team name into "dbo.Teams" and get back its id
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(sqlDatabaseName)))
			{
				// inserting to dbo.Teams table and getting id of the team
				DynamicParameters p = new DynamicParameters();
				p.Add("@TeamName", model.TeamName);
				p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

				connection.Execute("dbo.spTeams_Insert", p, commandType: CommandType.StoredProcedure);

				model.Id = p.Get<int>("@id");

				// assigning people to teams in dbo.TeamMembers
				p = new DynamicParameters();
				p.Add("@TeamId", model.Id);

				foreach (var member in model.TeamMembers)
				{
					p.Add("@PersonId", member.Id);

					connection.Execute("dbo.spTeamMembers_Insert", p, commandType: CommandType.StoredProcedure);
				}

				return model;
			}
		}

		public void CreateTournament(TournamentModel model)
		{

			using(IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(sqlDatabaseName)))
			{
				// store Tournament to the database
				SaveTournament(connection, model);

				// assign Teams to the tournament
				SaveTournamentEntries(connection, model);

				//assign prizes to the tournament
				SaveTournamentPrizes(connection, model);

				// TODO save - matchups
				SaveTournamentRounds(connection, model);

			}
		}

		private void SaveTournamentRounds(IDbConnection connection, TournamentModel model)
		{
			// loop through rounds
				// loop through matchups
					// save the matchup
					// loop through the entries and save them
			
			foreach (List<MatchupModel> round in model.Rounds)
			{
				foreach (MatchupModel matchup in round)
				{
					DynamicParameters p = new DynamicParameters();
					p.Add("@TournamentId", model.Id);
					p.Add("@MatchupRound", matchup.MatchupRound);
					p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

					connection.Execute("dbo.spMatchups_Insert", p, commandType: CommandType.StoredProcedure);
					matchup.Id = p.Get<int>("@id");

					foreach (MatchupEntryModel entry in matchup.Entries)
					{
						p = new DynamicParameters();
						p.Add("@MatchupId", matchup.Id);
						if (entry.ParentMatchup == null)
						{
							p.Add("@ParentMatchupId", null);
						}
						else
						{
							p.Add("@ParentMatchupId", entry.ParentMatchup.Id);
						}

						if (entry.TeamCompeting == null)
						{
							p.Add("@TeamCompetingId", null);
						}
						else
						{
							p.Add("@TeamCompetingId", entry.TeamCompeting.Id);
						}

						p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

						connection.Execute("dbo.spMatchupEntries_Insert", p, commandType: CommandType.StoredProcedure);
						entry.Id = p.Get<int>("@id");
					}
				}
			}
		}

		private void SaveTournamentPrizes(IDbConnection connection, TournamentModel model)
		{
			foreach (var prize in model.Prizes)
			{
				DynamicParameters p = new DynamicParameters();
				p.Add("@TournamentId", model.Id);
				p.Add("@PrizeId", prize.Id);
				p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

				connection.Execute("dbo.spTournamentPrizes_Insert", p, commandType: CommandType.StoredProcedure);
			}
		}

		private void SaveTournamentEntries(IDbConnection connection, TournamentModel model)
		{
			foreach (var team in model.EnteredTeams)
			{
				DynamicParameters p = new DynamicParameters();
				p.Add("@TournamentId", model.Id);
				p.Add("@TeamId", team.Id);
				p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

				connection.Execute("dbo.spTournamentEntries_Insert", p, commandType: CommandType.StoredProcedure);
			}
		}

		private void SaveTournament(IDbConnection connection, TournamentModel model)
		{
			DynamicParameters p = new DynamicParameters();
			p.Add("@TournamentName", model.TournamentName);
			p.Add("@EntryFee", model.EntryFee);
			// TODO - delete? -> p.Add("@Active", model.Active);
			p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

			connection.Execute("dbo.spTournaments_Insert", p, commandType: CommandType.StoredProcedure);

			model.Id = p.Get<int>("@id");
		}

		public List<PersonModel> GetPerson_All()
		{
			List<PersonModel> output;

			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(sqlDatabaseName)))
			{
				output = connection.Query<PersonModel>("dbo.spPeople_GetAll").ToList();
			}

			return output;
		}

		public List<TeamModel> GetTeam_All()
		{
			List<TeamModel> output = new List<TeamModel>();

			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(sqlDatabaseName)))
			{
				output = connection.Query<TeamModel>("dbo.spTeams_GetAll").ToList();

				foreach (var team in output)
				{
					DynamicParameters p = new DynamicParameters();
					p.Add("@TeamId", team.Id);

					team.TeamMembers = connection.Query<PersonModel>("dbo.spTeamMembers_GetByTeam", p, commandType: CommandType.StoredProcedure).ToList();
				}
			}

			return output;
		}

		public List<TournamentModel> GetTournament_All()
		{
			List<TournamentModel> output = new List<TournamentModel>();

			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(sqlDatabaseName)))
			{
				output = connection.Query<TournamentModel>("dbo.spTournaments_GetAll").ToList();



				foreach (TournamentModel t in output)
				{
					// Populate prizes
					DynamicParameters p = new DynamicParameters();
					p.Add("@TournamentId", t.Id);
					t.Prizes = connection.Query<PrizeModel>("dbo.spPrizes_GetByTournament", p, commandType: CommandType.StoredProcedure).ToList();


					// Populate Teams
					t.EnteredTeams = connection.Query<TeamModel>("dbo.spTeams_GetByTournament", p, commandType: CommandType.StoredProcedure).ToList();

					foreach (TeamModel team in t.EnteredTeams)
					{
						p = new DynamicParameters();
						p.Add("@TeamId", team.Id);
						team.TeamMembers = connection.Query<PersonModel>("dbo.spTeamMembers_GetByTeam", p, commandType: CommandType.StoredProcedure).ToList();
					}


					// Populate Rounds

					p = new DynamicParameters();
					p.Add("@TournamentId", t.Id);
					List<MatchupModel> matchups = connection.Query<MatchupModel>("dbo.spMatchups_GetByTournament", p, commandType: CommandType.StoredProcedure).ToList();

					foreach (MatchupModel matchup in matchups)
					{
						// populate winner
						if (matchup.WinnerId > 0)
						{
							matchup.Winner = t.EnteredTeams.Where(x => x.Id == matchup.WinnerId).First();
						}

						// populate entries
						p = new DynamicParameters();
						p.Add("@MatchupId", matchup.Id);
						matchup.Entries = connection.Query<MatchupEntryModel>("dbo.spMatchupEntries_GetByMatchup", p, commandType: CommandType.StoredProcedure).ToList();

						foreach (MatchupEntryModel me in matchup.Entries)
						{
							if (me.TeamCompetingId > 0)
							{
								me.TeamCompeting = t.EnteredTeams.Where(x => x.Id == me.TeamCompetingId).First();
							}

							if (me.ParentMatchupId > 0)
							{
								me.ParentMatchup = matchups.Where(x => x.Id == me.ParentMatchupId).First();
							}
						}
					}

					int numberOfRounds = matchups.OrderByDescending(x => x.MatchupRound).First().MatchupRound;
					for (int i = 0; i < numberOfRounds; i++)
					{
						t.Rounds.Add(new List<MatchupModel>());
					}

					foreach (MatchupModel matchup in matchups)
					{
						t.Rounds[matchup.MatchupRound - 1].Add(matchup);
					}

				}

			}

			return output;
		}

		public void UpdateMatchup(MatchupModel model)
		{
			//dbo.spMatchups_Update @id, @WinnerId
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(sqlDatabaseName)))
			{
				DynamicParameters p = new DynamicParameters();

				if (model.Winner != null)
				{
					p.Add("@id", model.Id);
					p.Add("@WinnerId", model.Winner.Id);

					connection.Execute("dbo.spMatchups_Update", p, commandType: CommandType.StoredProcedure); 
				}

				//spMatchupEntries_Update @id, @TeamCompetingId, @Score
				foreach (MatchupEntryModel me in model.Entries)
				{
					if (me.TeamCompeting != null)
					{
						p = new DynamicParameters();
						p.Add("@id", me.Id);
						p.Add("@TeamCompetingId", me.TeamCompeting.Id);
						p.Add("@Score", me.Score);

						connection.Execute("dbo.spMatchupEntries_Update", p, commandType: CommandType.StoredProcedure);  
					}
				}
			}

		}
	}
}
