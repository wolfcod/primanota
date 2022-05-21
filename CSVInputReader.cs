using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace pn
{
    enum CSVState
    {
        Header,
        Content,
        Invalid,
        EndOfFile
    };

    class CSVInputReader
    {
        private CSVState state = CSVState.Invalid;
        private int numberOfFields = 0;
        private int numberOfHeader = 1;

        public CSVInputReader(int fields)
        {
            this.numberOfFields = fields;
            this.numberOfHeader = 1;
        }

        private bool ParseLine(out RigheBanca riga, string [] fields)
        {
            riga = new RigheBanca();
            riga.CodiceIdentificativo = Convert.ToInt64(fields[0].Replace('"', ' ').Trim());
            riga.DataOperazione = DateTime.ParseExact(fields[1].Replace('"', ' ').Trim(), "dd/MM/yyyy", null);
            riga.DataValuta = DateTime.ParseExact(fields[2].Replace('"', ' ').Trim(), "dd/MM/yyyy", null);
            riga.Descrizione = fields[3].Replace('"', ' ').Trim();
            riga.Divisa = fields[4].Replace('"', ' ').Trim();
            if (fields[5].Length > 2)
            {
                string Valore = fields[5].Replace('"', ' ').Replace('-', ' ').Trim();
                riga.Valore = decimal.Parse(Valore, new NumberFormatInfo() { NumberDecimalSeparator = ",", NumberGroupSeparator = "." });
                riga.Valore = -riga.Valore;
            }
            else
            {
                string Valore = fields[6].Replace('"', ' ').Replace('+', ' ').Trim();
                riga.Valore = decimal.Parse(Valore, new NumberFormatInfo() { NumberDecimalSeparator = ",", NumberGroupSeparator = "." });
            }
            riga.Categoria = fields[7].Replace('"', ' ').Trim();
            riga.Etichette = fields[7].Replace('"', ' ').Trim();
            return true;
        }

        public List<RigheBanca> ParseInputFile(StreamReader reader)
        {
            this.state = CSVState.Header;

            string line;
            List<RigheBanca> righe = new List<RigheBanca>();

            Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

            while ((line = reader.ReadLine()) != null)
            {
                string[] fields = CSVParser.Split(line);

                if (fields.Length != this.numberOfFields)
                    this.state = CSVState.Invalid;

                RigheBanca outR = null;

                switch(this.state)
                {
                    case CSVState.Header:   // consume "numberOfHeader" fields before switching to content!
                        this.numberOfHeader--;
                        if (this.numberOfHeader == 0)
                            this.state = CSVState.Content;
                        break;
                    case CSVState.Content:
                        if (ParseLine(out outR, fields))
                            if (outR != null)
                                righe.Add(outR);
                        break;
                    case CSVState.Invalid:
                        Console.WriteLine("Processing {0} => INVALID.", line);
                        this.state = CSVState.Content;  // the wrong line could be a page footer or document footer.. switch back to content
                        break;
                }

            }
            return righe;
        }
    }
}
