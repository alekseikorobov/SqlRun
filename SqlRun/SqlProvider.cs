using System;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;

namespace SqlRun
{
    public class SqlProvider
    {
        //static DbContext Context;
        private readonly Options _options;
        //Microsoft.SqlServer.Management.Smo.Server server;
        System.Data.SqlClient.SqlCommand _server;

        public SqlProvider(Options options)
        {
            _options = options;
        }
        //public void ExecuteSqlCommand(string script)
        //{
        //    //int count = server.ConnectionContext.ExecuteNonQuery(script);
        //    //Console.WriteLine("return {0}", count, "");

        //    TSql100Parser t = new TSql100Parser(true);

        //    using (TextReader open = new StringReader(script))
        //    {
        //        IList<ParseError> errors;
        //        TSqlFragment frag = t.Parse(open, out errors);

        //        var s = frag as TSqlScript;
        //        int part = 0;
        //        foreach (var item in s.Batches)
        //        {
        //            SqlScriptGeneratorOptions opt = new SqlScriptGeneratorOptions();
        //            //opt.AlignClauseBodies
        //            Sql100ScriptGenerator gen = new Sql100ScriptGenerator(opt);
        //            string sql;
        //            gen.GenerateScript(item, out sql);

        //            server.CommandText = sql;
        //            int count = server.ExecuteNonQuery();

        //            //int count = server.ConnectionContext.ExecuteNonQuery(sql);

        //            Console.WriteLine("return {0} - {1}", count, (s.Batches.Count > 0 ? " Part - " + part.ToString() : ""));
        //        }
        //    }
        //}
        public void ExecuteSqlCommand(string script)
        {
            string[] sqlLines = script.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            List<string> sqlAll = this.SplitGo(sqlLines);

            for (int i = 0; i < sqlAll.Count; i++)
            {
                string sql = sqlAll[i];
                try
                {
                    if (string.IsNullOrEmpty(sql) || sql.Equals("\r\n") || sql.Equals("\n")) continue;

                    _server.CommandText = sql;
                    _server.CommandTimeout = 0;
                    int count = _server.ExecuteNonQuery();
                    Console.WriteLine("return {0} - {1}", count, (sqlAll.Count > 0 ? " Part - " + i : ""));
                }
                catch (Exception ex)
                {
                    throw new Exception($"Not correct line '{i}', sql='{sql}'", ex);
                }
            }
        }
        private List<string> SplitGo(IReadOnlyList<string> sqlLines)
        {
            List<string> sqlAll = new List<string>();
            List<int> separatorGo = new List<int>();
            int startIndex = 0;
            int nowIndex = 0;
            int comment = 0;
            for (int i = 0; i < sqlLines.Count; i++)
            {
                string line = sqlLines[i];
                line = Regex.Replace(line, @"/\*(.*)\*/", "");
                line = Regex.Replace(line, @"(.*?/\*).*", "$1");
                line = Regex.Replace(line, @".*?(\*/.*)", "$1");
                if (line.IndexOf("/*", StringComparison.Ordinal) != -1)
                {
                    comment++;
                }
                if (line.IndexOf("*/", StringComparison.Ordinal) != -1)
                {
                    comment--;
                }
                if (comment == 0 && Regex.IsMatch(line, "^([\t ]+|)GO([\t ]+|[\t ]+--.*|--.*|)$", RegexOptions.IgnoreCase))
                {
                    separatorGo.Add(i);
                }
            }
            do
            {
                int endIndex = separatorGo.Count > nowIndex ? separatorGo[nowIndex] : sqlLines.Count;

                StringBuilder sqlPart = new StringBuilder();

                for (int i = startIndex; i < endIndex; i++)
                {
                    sqlPart.AppendLine(sqlLines[i]);
                }
                sqlAll.Add(sqlPart.ToString());
                startIndex = endIndex + 1;

                nowIndex++;
            } while (nowIndex <= separatorGo.Count);

            return sqlAll;
        }
        public void InitConnection()
        {
            //System.Diagnostics.Debugger.Launch();

            //Context = new DbContext(connectionString);

            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(this.ConnectionString());
            _server = new System.Data.SqlClient.SqlCommand { Connection = conn };
            conn.Open();

            //server = new Server(new ServerConnection(conn));
            //server.ConnectionContext.Connect();
        }

        public string ConnectionString()
        {

            string connectionString = "";
            if (ConfigurationManager.ConnectionStrings != null
                && ConfigurationManager.ConnectionStrings.Count > 0
                && ConfigurationManager.ConnectionStrings["Default"] != null)
                connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                if (!string.IsNullOrEmpty(_options.DataBase) && !string.IsNullOrEmpty(_options.Source))
                {
                    connectionString = $"data source={_options.Source};initial catalog={_options.DataBase};integrated security=True;application name={_options.DataBase};MultipleActiveResultSets=True";
                }
                else
                {
                    throw new Exception("Не указано подключение, ни в конфиге ни в параметрах");
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(_options.DataBase))
                {
                    connectionString = Regex.Replace(connectionString, @"initial catalog\=(.*?);", $"initial catalog={_options.DataBase};");
                    connectionString = Regex.Replace(connectionString, @"application name\=(.*?);", $"initial catalog={_options.DataBase};");
                }
                if (!string.IsNullOrEmpty(_options.Source))
                {
                    connectionString = Regex.Replace(connectionString, @"data source\=(.*?);", $"data source={_options.Source};");
                }
            }
            //Console.WriteLine("connectionString - {0}", connectionString);

            return connectionString;

        }
    }
}

