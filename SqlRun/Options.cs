using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using System.Configuration;
using System.Text.RegularExpressions;

namespace SqlRun
{
    public class Options
    {
        //[Option('r', "read", Required = false, HelpText = "Input file to be processed.")]
        //public string InputFile { get; set; }

        //[Option('v', "verbose", DefaultValue = false, HelpText = "Prints all messages to standard output.")]
        //public bool Verbose { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [Option('p', "path", Required = false, HelpText = "Path or dir from sql files. Using regex for file name")]
        public string Path { get; set; }

        [Option('f', "file", Required = false, HelpText = "Read path from file")]
        public string File { get; set; }

        [Option('s', "server", Required = false, HelpText = "Db connection server host")]
        public string Source { get; set; }

        [Option('d', "db", Required = false, HelpText = "Db connection database name")]
        public string DataBase { get; set; }
        public string Patern { get; set; }
        public bool Verbose { get; set; }

        [Option('t', "test", Required = false, HelpText = "Db connection database name")]
        public bool IsTest { get; set; }
        public string ConnectionString
        {
            get
            {
                var connectionString = "";
                if (ConfigurationManager.ConnectionStrings != null
                    && ConfigurationManager.ConnectionStrings.Count > 0
                    && ConfigurationManager.ConnectionStrings["Default"] != null)
                    connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    if (!string.IsNullOrEmpty(DataBase) && !string.IsNullOrEmpty(Source))
                    {
                        connectionString = $"data source={Source};initial catalog={DataBase};integrated security=True;application name={DataBase};MultipleActiveResultSets=True";
                    }
                    else
                    {
                        throw new Exception("Не указано подключение, ни в конфиге ни в параметрах");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(DataBase))
                    {
                        connectionString = Regex.Replace(connectionString, @"initial catalog\=(.*?);", $"initial catalog={DataBase};");
                        connectionString = Regex.Replace(connectionString, @"application name\=(.*?);", $"initial catalog={DataBase};");
                    }
                    if (!string.IsNullOrEmpty(Source))
                    {
                        connectionString = Regex.Replace(connectionString, @"data source\=(.*?);", $"data source={Source};");
                    }
                }
                //Console.WriteLine("connectionString - {0}", connectionString);

                return connectionString;
            }
        }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
