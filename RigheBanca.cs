using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pn
{
    class RigheBanca
    {
        public RigheBanca()
        {

        }

        public Int64 CodiceIdentificativo { get; set; }
        public DateTime DataOperazione { get; set; }
        public DateTime DataValuta { get; set; }
        public string Descrizione { get; set; }
        public string Divisa { get; set; }
        public decimal Valore { get; set; }
        public string Categoria { get; set; }
        public string Etichette { get; set; }
    }
}
