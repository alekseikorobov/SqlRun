using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace SqlRun
{
    class Options
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

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
