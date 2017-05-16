using System;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.Configuration;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.Collections.Generic;

namespace SqlRun
{
    public class SqlProvider
    {
        //static DbContext Context;
        private Options options;
        //Microsoft.SqlServer.Management.Smo.Server server;
        System.Data.SqlClient.SqlCommand server;

        public SqlProvider(Options options)
        {
            this.options = options;
        }
        public void ExecuteSqlCommand(string script)
        {
            //int count = server.ConnectionContext.ExecuteNonQuery(script);
            //Console.WriteLine("return {0}", count, "");

            TSql100Parser t = new TSql100Parser(true);

            using (TextReader open = new StringReader(script))
            {
                IList<ParseError> errors;
                TSqlFragment frag = t.Parse(open, out errors);

                var s = frag as TSqlScript;
                int part = 0;
                foreach (var item in s.Batches)
                {
                    SqlScriptGeneratorOptions opt = new SqlScriptGeneratorOptions();
                    Sql100ScriptGenerator gen = new Sql100ScriptGenerator();
                    string sql;
                    gen.GenerateScript(item, out sql);

                    server.CommandText = sql;
                    int count = server.ExecuteNonQuery();

                    //int count = server.ConnectionContext.ExecuteNonQuery(sql);

                    Console.WriteLine("return {0} - {1}", count, (s.Batches.Count > 0 ? " Part - " + part.ToString() : ""));
                }
            }
        }
        //public void ExecuteSqlCommand(string script)
        //{
        //    string[] sqlLines = script.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

        //    List<string> sqlAll = SplitGo(sqlLines);

        //    for (int i = 0; i < sqlAll.Count; i++)
        //    {
        //        string sql = sqlAll[i];
        //        if (string.IsNullOrEmpty(sql) || sql.Equals("\r\n") || sql.Equals("\n")) continue;

        //        //int count = Context.Database.ExecuteSqlCommand(sql);
        //        //server.CommandText = sql;
        //        int count = server.ExecuteNonQuery();
        //        Console.WriteLine("return {0} - {1}", count, (sqlAll.Count > 0 ? " Part - " + i.ToString() : ""));
        //    }

        //    //server.ConnectionContext.ExecuteNonQuery(script);            
        //}
        //private List<string> SplitGo(string[] sqlLines)
        //{
        //    List<string> sqlAll = new List<string>();
        //    List<int> seporatorGO = new List<int>();
        //    int startIndex = 0;
        //    int nowIndex = 0;
        //    int coment = 0;
        //    for (int i = 0; i < sqlLines.Length; i++)
        //    {
        //        string line = sqlLines[i];
        //        line = Regex.Replace(line, @"/\*(.*)\*/", "");
        //        line = Regex.Replace(line, @"(.*?/\*).*", "$1");
        //        line = Regex.Replace(line, @".*?(\*/.*)", "$1");
        //        if (line.IndexOf("/*") != -1)
        //        {
        //            coment++;
        //        }
        //        if (line.IndexOf("*/") != -1)
        //        {
        //            coment--;
        //        }
        //        if (coment == 0 && Regex.IsMatch(line, "^( +|)GO( +|)$", RegexOptions.IgnoreCase))
        //        {
        //            seporatorGO.Add(i);
        //        }
        //    }
        //    do
        //    {
        //        int endIndex = seporatorGO.Count > nowIndex ? seporatorGO[nowIndex] : sqlLines.Length;

        //        StringBuilder sqlPart = new StringBuilder();

        //        for (int i = startIndex; i < endIndex; i++)
        //        {
        //            sqlPart.AppendLine(sqlLines[i]);
        //        }
        //        sqlAll.Add(sqlPart.ToString());
        //        startIndex = endIndex + 1;

        //        nowIndex++;
        //    } while (nowIndex <= seporatorGO.Count);

        //    return sqlAll;
        //}
        public void InitConection()
        {
            //System.Diagnostics.Debugger.Launch();

            //Context = new DbContext(connectionString);

            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(ConnectionString());
            server = new System.Data.SqlClient.SqlCommand();
            server.Connection = conn;
            conn.Open();

            //server = new Server(new ServerConnection(conn));
            //server.ConnectionContext.Connect();
        }

        public string ConnectionString()
        {

            var connectionString = "";
            if (ConfigurationManager.ConnectionStrings != null
                && ConfigurationManager.ConnectionStrings.Count > 0
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
            //Console.WriteLine("connectionString - {0}", connectionString);

            return connectionString;

        }
    }
}

