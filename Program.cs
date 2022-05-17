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

        [Option('t', "testata", Required = false, HelpText = ("Testata output file."))]
        public string Testate { get; set; }


        [Option('r', "righe", Required = false, HelpText = ("Righe output file"))]
        public string Righe { get; set; }
    }

    class Program
    {

        static void Main(string[] args)
        {
            StreamReader r = null;
            
            StreamWriter tOutput = null;
            StreamWriter rOutput = null;
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o =>
            {
                if (o.Verbose)
                {
                    Console.WriteLine($"Verbose output enabled");
                }

                r = new StreamReader(o.Input);

                if (o.Testate.Length > 0)
                    tOutput = new StreamWriter(o.Testate);

                if (o.Righe.Length > 0)
                    rOutput = new StreamWriter(o.Righe);
            });

            List<RigheBanca> data = null;
            if (r != null)
            {
                CSVInputReader input = new CSVInputReader();
                data = input.ParseInputFile(r, 1);
            }

            PrimaNota pn = new PrimaNota();

            if (data != null)
            {

                int i = 1;
                foreach(RigheBanca riga in data)
                {
                    pn.Converti(i++, riga);
                }
            }

            if (tOutput != null)
            {
                tOutput.WriteLine(pn.HeaderTestate);
                foreach (string txt in pn.Testate)
                {
                    tOutput.WriteLine(txt);
                }

                tOutput.Flush();
                tOutput.Close();
            }

            if (rOutput != null)
            {
                rOutput.WriteLine(pn.HeaderRighe);
                foreach (string txt in pn.Righe)
                {
                    rOutput.WriteLine(txt);
                }

                rOutput.Flush();
                rOutput.Close();

            }
        }
    }
}
