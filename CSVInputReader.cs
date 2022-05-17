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
    class CSVInputReader
    {
        public List<RigheBanca> ParseInputFile(StreamReader reader, int headerLineNo)
        {
            string line;
            List<RigheBanca> righe = new List<RigheBanca>();

            // consume N line from the header
            while (headerLineNo-- > 0)
            {
                reader.ReadLine();  //
            }

            while ((line = reader.ReadLine()) != null)
            {
                //Define pattern
                Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

                
                //Separating columns to array
                string[] X = CSVParser.Split(line);

                if (X.Length < 9)
                    continue;

                /* Do something with X */
                RigheBanca riga = new RigheBanca();

                riga.CodiceIdentificativo = Convert.ToInt64(X[0].Replace('"', ' ').Trim());
                riga.DataOperazione = DateTime.ParseExact(X[1].Replace('"', ' ').Trim(), "dd/MM/yyyy", null);
                riga.DataValuta = DateTime.ParseExact(X[2].Replace('"', ' ').Trim(), "dd/MM/yyyy", null);
                riga.Descrizione = X[3].Replace('"', ' ').Trim();
                riga.Divisa = X[4].Replace('"', ' ').Trim();
                if (X[5].Length > 2)
                {
                    string Valore = X[5].Replace('"', ' ').Replace('-', ' ').Trim();
                    riga.Valore = decimal.Parse(Valore, new NumberFormatInfo() { NumberDecimalSeparator = ",", NumberGroupSeparator = "." });
                    riga.Valore = -riga.Valore;
                }
                else
                {
                    string Valore = X[6].Replace('"', ' ').Replace('+', ' ').Trim();
                    riga.Valore = decimal.Parse(Valore, new NumberFormatInfo() { NumberDecimalSeparator = ",", NumberGroupSeparator = "." });
                }
                riga.Categoria = X[7].Replace('"', ' ').Trim();
                riga.Etichette = X[7].Replace('"', ' ').Trim();
                righe.Add(riga);
            }

            return righe;
        }
    }
}
