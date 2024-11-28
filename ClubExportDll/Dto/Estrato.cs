using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubExportDll.Dto
{
    public class Estrato
    {

        private string  tipodctofa;
        private string  factura;
        private string  tipodcto;
        private string  nrodcto;
        private string  fecha;
        private decimal puntos;
        private double  comision;
        private string  dctomae;
        private long    idestrato;
        private Boolean syncronizado;



        public Estrato(string tipodctofa, string factura, string tipodcto, string nrodcto, string fecha, decimal puntos, double comision, string dctomae, long idestrato, Boolean syncronizado)
        {
            this.tipodctofa   = tipodctofa; 
            this.factura      = factura;
            this.tipodcto     = tipodcto;
            this.nrodcto      = nrodcto;
            this.fecha        = fecha;
            this.puntos       = puntos;
            this.comision     = comision;
            this.dctomae      = dctomae;
            this.idestrato    = idestrato;
            this.Syncronizado = syncronizado;
        }

        public string Tipodctofa { get => tipodctofa; set => tipodctofa = value; }
        public string Factura { get => factura; set => factura = value; }
        public string Tipodcto { get => tipodcto; set => tipodcto = value; }
        public string Nrodcto { get => nrodcto; set => nrodcto = value; }
        public string Fecha { get => fecha; set => fecha = value; }
        public decimal Puntos { get => puntos; set => puntos = value; }
        public double Comision { get => comision; set => comision = value; }
        public string Dctomae { get => dctomae; set => dctomae = value; }
        public long Idestrato { get => idestrato; set => idestrato = value; }
        public bool Syncronizado { get => syncronizado; set => syncronizado = value; }
    }
}
