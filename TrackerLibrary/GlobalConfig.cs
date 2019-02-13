using System.Collections.Generic;
using TrackerLibrary.DataAccess;
using System.Configuration;

namespace TrackerLibrary
{
	public static class GlobalConfig
	{
		public static IDataConnection Connection { get; private set; }

		public static void InitializeConnections(DatabaseType db)
		{
			switch (db)
			{
				case DatabaseType.Sql:
					// TODO - Set up the SQL Connector properly
					SqlConnector sql = new SqlConnector();
					Connection = sql;
					break;
				case DatabaseType.TextFile:
					// TODO - Create the Text Connection
					TextConnector text = new TextConnector();
					Connection = text;
					break;
				default:
					break;
			}

			//if (db == DatabaseType.Sql)
			//{
			//	// TODO - Set up the SQL Connector properly
			//	SqlConnector sql = new SqlConnector();
			//	Connections = (IDataConnection)sql;
			//}
			//else if (db == DatabaseType.TextFile)
			//{
			//	// TODO - Create the Text Connection
			//	TextConnector text = new TextConnector();
			//	Connections = (IDataConnection)text;
			//}
		}

		public static string CnnString(string name)
		{
			return ConfigurationManager.ConnectionStrings[name].ConnectionString;
		}
	}
}
