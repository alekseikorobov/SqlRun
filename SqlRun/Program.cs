﻿using System;
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


namespace SqlRun
{
    class Program
    {
		static SqlProvider SqlProvider; 
        
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

				SqlProvider = new SqlProvider();
				SqlProvider.InitConection ();

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

        private static void ActionFile(string file)
        {
            try
            {
				Console.WriteLine("file - {1}", Path.GetFileNameWithoutExtension(file));
				SqlProvider.ExecuteSqlCommand(File.ReadAllText(file));

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " " + file);
            }
            Console.ResetColor();
        }

        
    }
}
