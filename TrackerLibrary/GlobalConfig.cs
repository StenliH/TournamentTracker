using System.Collections.Generic;
using TrackerLibrary.DataAccess;
using System.Configuration;

namespace TrackerLibrary
{
	public static class GlobalConfig
	{
		public const string PrizesFile = "PrizeModels.csv";
		public const string PeopleFile = "PersonModels.csv";
		public const string TeamsFile = "TeamModels.csv";
		public const string TournamentsFile = "Tournaments.csv";
		public const string MatchupFile = "MatchupModels.csv";
		public const string MatchupEntryFile = "MatchupEntryModels.csv";

		public static IDataConnection Connection { get; private set; }

		public static void InitializeConnections(DatabaseType db)
		{
			switch (db)
			{
				case DatabaseType.Sql:
					SqlConnector sql = new SqlConnector();
					Connection = sql;
					break;
				case DatabaseType.TextFile:
					TextConnector text = new TextConnector();
					Connection = text;
					break;
				default:
					break;
			}

		}

		public static string CnnString(string name)
		{
			return ConfigurationManager.ConnectionStrings[name].ConnectionString;
		}
	}
}
