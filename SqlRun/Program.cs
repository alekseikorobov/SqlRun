using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
//using EntityFramework;
using CommandLine;
using CommandLine.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Migrations.History;
using System.Data.Entity.Core.EntityClient;

namespace SqlRun
{
    class Program
    {
        static DbContext Context;
        static Options options;
        static void Main(string[] args)
        {
            //args = new string[6];
            //args[0] = "-s";
            //args[1] = "rumskapd29";
            //
            //args[2] = "-d";
            //args[3] = "KDB_Production";
            //
            //args[4] = "-p";
            //args[5] = @"\\tfs.ru.kworld.kpmg.com\Builds\Japps\2017.04\SQL\kdb";
            
            bool IsDirectory = true;
            bool IsFromFile = false;
            try
            {
                options = new Options();
                if(args.Length == 0) { 
                    Console.WriteLine(options.GetUsage());
                    Console.Write("Run sql with param to default? (y/n): ");
                    string res = Console.ReadLine();
                    if(res != "y")
                        return;
                }

                if (Parser.Default.ParseArguments(args, options))
                {
                    options.Verbose = true;
                }
                if (!string.IsNullOrEmpty(options.Path))
                {
                    options.Path = options.Path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    //char lastChar = options.Path[options.Path.Length - 1];
                    if (File.Exists(options.Path))
                    {
                        IsDirectory = false;
                    }
                    else
                    {
                        if (Directory.Exists(options.Path))
                        {
                            options.Patern = "*.sql";
                        }
                        else
                        {
                            int index = options.Path.LastIndexOf(Path.AltDirectorySeparatorChar);
                            if (index != -1)
                                options.Patern = options.Path.Substring(index);
                            else
                                options.Patern = "*";
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(options.File))
                {
                    IsFromFile = true;
                }
                else
                {
                    options.Path = Environment.CurrentDirectory;
                    options.Patern = "*.sql";
                }

                //test(args); return;

                InitConection();

                if (IsFromFile)
                {
                    foreach (var file in File.ReadAllLines(options.File))
                    {
                        if (!File.Exists(file))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("File {0} not exists", file);
                            continue;
                        }
                        Console.ResetColor();
                        ActionFile(file);
                    }
                }
                else if (IsDirectory)
                {
                    //System.Diagnostics.Debugger.Launch();
                    //string str = System.Web.HttpContext.Current.Server.MapPath(options.Path);
                    //string s = Path.GetFullPath(options.Path);
                    options.Path = CleanFileName(options.Path);
                    //Console.WriteLine("options.Path - {0}", options.Path);
                    var d = new DirectoryInfo(options.Path);
                    var files = d.GetFiles(options.Patern);

                    if (files.Length == 0)
                    {
                        var newPath = Path.Combine(options.Path, "sql");
                        if (Directory.Exists(newPath))
                        {
                            d = new DirectoryInfo(newPath);
                            files = d.GetFiles(options.Patern);
                        }
                    }

                    foreach (var file in files.OrderBy(c =>
                     {
                         int i = int.MaxValue;
                         if (Regex.IsMatch(c.Name, "^\\d+"))
                             i = int.Parse(Regex.Match(c.Name, "^\\d+").Value);
                         return i;
                     }))
                    {
                        ActionFile(file.FullName);
                    }
                }
                else
                {
                    ActionFile(options.Path);
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.ResetColor();
                Console.WriteLine("end");
                Console.Read();
            }
            Console.WriteLine("end");
        }

        private static string CleanFileName(string fileName)
        {            
            //return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
            return fileName.Replace("\"", "");
        }

        private static void test(string[] args)
        {
            Console.WriteLine("-----------------------------");
            Console.WriteLine("args:");
            foreach (var item in args)
            {
                Console.WriteLine("\t{0}", item);
            }

            Console.WriteLine("DataBase - {0}", options.DataBase);
            Console.WriteLine("File - {0}", options.File);
            Console.WriteLine("Patern - {0}", options.Patern);
            Console.WriteLine("Path - {0}", options.Path);
            Console.WriteLine("Source - {0}", options.Source);
            Console.WriteLine("Verbose - {0}", options.Verbose);

            Console.WriteLine("-----------------------------");
        }

        private static void InitConection()
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
            Context = new DbContext(connectionString);
            //MyDbConfiguration.BuildConnectionString(
        }

        private static void ActionFile(string file)
        {
            try
            {
                string[] sqlLines = File.ReadAllLines(file);

                List<string> sqlAll = SplitGo(sqlLines);

                for (int i = 0; i < sqlAll.Count; i++)
                {
                    string sql = sqlAll[i];
                    if (string.IsNullOrEmpty(sql) || sql.Equals("\r\n") || sql.Equals("\n")) continue;
                    string s = Context.Database.Connection.Database;
                    
                    int count = Context.Database.ExecuteSqlCommand(sql);
                    Console.WriteLine("count {1} - {0} {2}", Path.GetFileNameWithoutExtension(file), count, (sqlAll.Count>0?" Part - " + i.ToString():""));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " " + file);
            }
            Console.ResetColor();
        }

        private static List<string> SplitGo(string[] sqlLines)
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
    }

    //public class MyDbConfiguration : DbConfiguration
    //{
    //    public MyDbConfiguration()
    //    {
    //        SetDefaultConnectionFactory(new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0"));
    //        SetProviderServices("System.Data.SqlClient", System.Data.Entity.SqlServer.SqlProviderServices.Instance);
    //        SetProviderServices("System.Data.SqlServerCe.4.0", System.Data.Entity.SqlServer.SqlProviderServices.Instance);
            
    //    }
    //    public static string BuildConnectionString(string sqlBuilder)
    //    {

    //        // Initialize the connection string builder for the
    //        // underlying provider.
            
    //        EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();

    //        //Set the provider name.
    //        entityBuilder.Provider = "System.Data.SqlClient";

    //        // Set the provider-specific connection string.
    //        entityBuilder.ProviderConnectionString = sqlBuilder;

    //        // Set the Metadata location.
    //        entityBuilder.Metadata = @"res://*/DatabaseModel.csdl|res://*/DatabaseModel.ssdl|res://*/DatabaseModel.msl";

    //        return entityBuilder.ToString();

    //    }
    //}
}
