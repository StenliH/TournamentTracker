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
				var p = new DynamicParameters();
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
				var p = new DynamicParameters();
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
				var p = new DynamicParameters();
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

		public void AssignPeopleToTeam(TeamModel model)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(sqlDatabaseName)))
			{
			}
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
					var p = new DynamicParameters();
					p.Add("@TeamId", team.Id);

					team.TeamMembers = connection.Query<PersonModel>("dbo.spTeamMembers_GetByTeam", p, commandType: CommandType.StoredProcedure).ToList();
				}
			}

			return output;
		}
	}
}
