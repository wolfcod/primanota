using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pn
{    
    class OutputBase
    {
        protected List<string> field = new List<string>();
        protected List<string> values = new List<string>();

        public override string ToString()
        {
            string value = "";
            foreach (string s in values)
            {
                if (value.Length > 0)
                    value += ";";

                value += s;
            }

            return value;
        }

        protected OutputBase(string header, string initialValues)
        {
            foreach (string s in header.Split(';'))
            {
                field.Add(s);
                values.Add(string.Empty);
            }

            if (initialValues.Length > 0)
            {
                int n = 0;
                foreach (string s in initialValues.Split(";"))
                {
                    values[n] = s;
                    n++;
                }
            }

        }

        private int findKey(string k)
        {
            int pos = 0;
            foreach(string s in field)
            {
                if (s.CompareTo(k) == 0)
                {
                    return pos;
                }

                pos++;
            }

            return -1;
        }
        public string this[string k]
        {
            get { int i = findKey(k); if (i >= 0) return values[i]; return string.Empty; }
            set { int i = findKey(k); if (i >= 0) values[i] = value; }
        }
    }

    class Testata : OutputBase
    {
        private static string header = "PNPRN;PNDRE;PNOPE;PNCAU;CAUGUI;PNTDE;PNRPR;PNSPR;PNNPR;PNRPS;PNSPS;PNNPS;PNINT;SIGVAL;PNEST;SERDOC;PNNDO;PNDDO;DTFTPG;NREGIP;GIORN;REGIP;REGIS;PNBIL;PNDST;DAPARC;USOINT1;USOINT2;USOINT3;USOINT4;USOINT5;_PNGCO;_PNNVA;GESTCORR;VERS;UTENTE;DATAINVIO;ORAINVIO;PNDAVERIF;PNPROVEN;PNCODSAZ;PNNUMRE;PNNVABL;PNNVPBL;PNCTRBL;PNVRP;PNIOPBL;PNEOPBL;PNDCOBL;PNNIDPN;PNAIDPN;PNDRV;PNEDPE;PNFCS;PNTDER;PNCIC1;PNCIC2;PNCIC3;PNCIC4;PNCIC5;PNCIC6;PNCIC7;PNCIC8;PNCIP1;PNCIP2;PNCIP3;PNCIP4;PNCIP5;PNCIP6;PNCIP7;PNCIP8;PNILSP;PNNPPM;PNEDTS;PNEDSF;PNCFSF;PNCOMI;PNPTTS;PNPAEO;PNDPDS";
        private static string def = "3032;11032021;1;GR;-1;INCASSO CORRISP;;;;;;;N;.;;;;11032021;;;N;N;N;;;N;;;;;;S;N;S;3;ADMIN sul terminale 1;23012022;07:32;;N;1;;N;N;;N;N;N;;3031;21;;N;;;;;;;;;;;;;;;;;;;N;N;N;N;;N;S;;N";

       public Testata() : base(Testata.header, Testata.def)
        {
        }

        public static string Header() { return Testata.header; }
    }

    class Riga : OutputBase
    {
        private static string header = "PNPRN;PNDRE;PNCTO;PNIMP;PNCCR;PNDES;PNIMB1;PNIVA1;PNALI1;PNALD1;PNIMB2;PNIVA2;PNALI2;PNALD2;PNIMB3;PNIVA3;PNALI3;PNALD3;PNIMB4;PNIVA4;PNALI4;PNALD4;CODPAG;CODAGE;IMPPRO;USOINT1;USOINT2;USOINT3;USOINT4;USOINT5;USOINT6;USOINT7;USOINT8;USOINT9;PNEDS;PNEDN;PNEDC;PNDDO;PNCND1;PNCND2;PNCND3;PNCND4;PNRSVA;PNRDN;PNRDVA;PNORCO;PNECAC;PNCFPI;PNCCS;PNCCTR;PNTPC;PNTRC;PNDCP;PNTPM";
        private static string def = "3031;10032021;20201004;60,00;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;N;N;N;N;;;;N;N;;;;;;;";

        public Riga()
            : base(Riga.header, def)
        {
        }

        public static string Header() { return Riga.header; }
    }


    class PrimaNota
    {
        public PrimaNota()
        {
            testate = new List<string>();
            righe = new List<string>();
        }

        public bool Converti(int progressivo, RigheBanca r)
        {
            Testata t = new Testata();

            t["PNPRN"] = progressivo.ToString();
            t["PNDRE"] = r.DataValuta.ToString("ddMMyyyy");
            t["PNDDO"] = r.DataOperazione.ToString("ddMMyyyy");
            t["DATAINVIO"] = r.DataValuta.ToString("ddMMyyyy");
            t["ORAINVIO"] = "08:00";
            t["PNNIDPN"] = progressivo.ToString();

            for (int i = 0; i < 2; i++)
            {
                Riga riga = new();

                riga["PNPRN"] = progressivo.ToString();
                riga["PNDRE"] = r.DataValuta.ToString("ddMMyyyy");
                riga["PNCTO"] = (i == 0) ? "20201004" : "22200081";

                decimal valore = r.Valore;

                if (valore < 0)
                    valore = -valore;
                riga["PNIMP"] = (i == 0) ? valore.ToString("0.00") : valore.ToString("-0.00");

                righe.Add(riga.ToString());
            }

            testate.Add(t.ToString());

            return true;
        }

        public List<string> Testate { get { return this.testate; } }
        public string HeaderTestate { get { return Testata.Header(); } }
        public string HeaderRighe { get { return Riga.Header(); } }
        public List<string> Righe { get { return this.righe; } }

        private List<string> testate;
        private List<string> righe;
    }
}
