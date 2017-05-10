using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace SqlRun
{
    public class SqlProvider
    {
        //static DbContext Context;
        private Options options;
        //Server server;
        SqlCommand server;

        public SqlProvider(Options options)
        {
            this.options = options;
        }
        public void ExecuteSqlCommand(string script)
        {
            string[] sqlLines = script.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            List<string> sqlAll = SplitGo(sqlLines);

            for (int i = 0; i < sqlAll.Count; i++)
            {
                string sql = sqlAll[i];
                if (string.IsNullOrEmpty(sql) || sql.Equals("\r\n") || sql.Equals("\n")) continue;

                //int count = Context.Database.ExecuteSqlCommand(sql);
                server.CommandText = sql;
                int count = server.ExecuteNonQuery();
                Console.WriteLine("return {0} - {1}", count, (sqlAll.Count > 0 ? " Part - " + i.ToString() : ""));
            }

            //server.ConnectionContext.ExecuteNonQuery(script);            
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
            


            //Context = new DbContext(connectionString);

            SqlConnection conn = new SqlConnection(options.ConnectionString);
            server = new SqlCommand();
            server.Connection = conn;
            conn.Open();
            //server = new Server(new ServerConnection(conn));
        }
    }
}

