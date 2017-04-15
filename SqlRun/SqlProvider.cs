using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Migrations.History;
using System.Data.Entity.Core.EntityClient;

namespace SqlRun
{
	public class SqlProvider
	{
		static DbContext Context;
		public SqlProvider ()
		{
			
		}
		public void ExecuteSqlCommand(string text)
		{
			string[] sqlLines = text.Split (new string[]{"\r\n","\n"});

			List<string> sqlAll = SplitGo(sqlLines);

			for (int i = 0; i < sqlAll.Count; i++)
			{
				string sql = sqlAll[i];
				if (string.IsNullOrEmpty(sql) || sql.Equals("\r\n") || sql.Equals("\n")) continue;

				int count = Context.Database.ExecuteSqlCommand(sql);
				Console.WriteLine("return {0} - {1}", count, (sqlAll.Count>0?" Part - " + i.ToString():""));
			}
		}
		private List<string> SplitGo(string[] sqlLines)
		{
			List<string> sqlAll = new List<string>();
			List<int> seporatorGO = new List<int>();
			int startIndex = 0;
			int nowIndex = 0;
			int coment = 0;
			for (int i = 0; i < sqlLines.Length; i++)
			{
				string line = sqlLines[i];
				line = Regex.Replace(line, @"/\*(.*)\*/", "");
				line = Regex.Replace(line, @"(.*?/\*).*", "$1");
				line = Regex.Replace(line, @".*?(\*/.*)", "$1");
				if (line.IndexOf("/*") != -1)
				{
					coment++;
				}
				if (line.IndexOf("*/") != -1)
				{
					coment--;
				}
				if (coment == 0 && Regex.IsMatch(line, "^( +|)GO( +|)$", RegexOptions.IgnoreCase))
				{
					seporatorGO.Add(i);
				}
			}
			do
			{
				int endIndex = seporatorGO.Count > nowIndex ? seporatorGO[nowIndex] : sqlLines.Length;

				StringBuilder sqlPart = new StringBuilder();

				for (int i = startIndex; i < endIndex; i++)
				{
					sqlPart.AppendLine(sqlLines[i]);
				}
				sqlAll.Add(sqlPart.ToString());
				startIndex = endIndex + 1;

				nowIndex++;
			} while (nowIndex <= seporatorGO.Count);

			return sqlAll;
		}
		public void InitConection()
		{
			//System.Diagnostics.Debugger.Launch();
			var connectionString = "";
			if (ConfigurationManager.ConnectionStrings != null 
				&& ConfigurationManager.ConnectionStrings.Count>0
				&& ConfigurationManager.ConnectionStrings["Default"] != null)
				connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;

			if (string.IsNullOrEmpty(connectionString))
			{
				if (!string.IsNullOrEmpty(options.DataBase) && !string.IsNullOrEmpty(options.Source))
				{
					connectionString = $"data source={options.Source};initial catalog={options.DataBase};integrated security=True;application name={options.DataBase};MultipleActiveResultSets=True";
				}
				else
				{
					throw new Exception("Не указано подключение, ни в конфиге ни в параметрах");
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(options.DataBase))
				{
					connectionString = Regex.Replace(connectionString, @"initial catalog\=(.*?);", $"initial catalog={options.DataBase};");
					connectionString = Regex.Replace(connectionString, @"application name\=(.*?);", $"initial catalog={options.DataBase};");
				}
				if (!string.IsNullOrEmpty(options.Source))
				{
					connectionString = Regex.Replace(connectionString, @"data source\=(.*?);", $"data source={options.Source};");
				}
			}
			Console.WriteLine("connectionString - {0}", connectionString);
			//SqlConnection conn = new SqlConnection(sqlConnectionString);
			//Server server = new Server(new ServerConnection(conn));
			//server.ConnectionContext.ExecuteNonQuery(script);

			Context = new DbContext(connectionString);
		}
	}
}

