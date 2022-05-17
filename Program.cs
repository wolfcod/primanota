using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using CommandLine;

namespace pn
{
    public class Options
    {
        [Option('v', "verbore", Required = false, HelpText = ("Set output to verbose messges."))]
        public bool Verbose { get; set; }

        [Option('i', "input", Required = true, HelpText = ("Input File."))]
        public string Input { get; set; }

        [Option('o', "output", Required = true, HelpText = ("Output fileFile."))]
        public string Output { get; set; }

        [Option('r', "rules", Required = false, HelpText = ("JSON SCHEMA RULES."))]
        public string Rules { get; set; }
    }

    class Program
    {

        static void Main(string[] args)
        {
            StreamReader r = null;
            StreamWriter w = null;

            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o =>
            {
                if (o.Verbose)
                {
                    Console.WriteLine($"Verbose output enabled");
                }

                r = new StreamReader(o.Input);
                w = new StreamWriter(o.Output);
            });
            
            if (r != null)
            {
                ParseInputFile(r);
            }
        }
    }
}
