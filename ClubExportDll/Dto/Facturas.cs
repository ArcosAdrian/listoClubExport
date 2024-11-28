using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubExportDll.Dto
{
    public class Facturas
    {
        private String grupo;
        private String tipodctofa;
        private String factura;
        private String tipodcto;
        private String nrodcto;
        private String nit;
        private String ffactura;
        private decimal dscto;
        private decimal puntos;
        private double comision;
        private String fmaxcobro;
        private String nroegreso;
        private String fechapago;
        private String codcc;
        private String fingreso;
        private String codven;
        private int activo;
        private double subtotal;
        private long idfactura;
        private Boolean syncronizado;


        public Facturas(string grupo, string tipodctofa, string factura,string tipodcto, string nrodcto, string nit, string ffactura, decimal dscto, decimal puntos, double comision, string fmaxcobro, string nroegreso, string fechapago, string codcc, String fingreso, String codven, double subtotal, int activo, long idfactura, Boolean syncronizado)
        {
            this.Grupo = grupo;
            this.Tipodctofa = tipodctofa;
            this.Factura = factura;
            this.Tipodcto = tipodcto;
            this.Nrodcto = nrodcto;
            this.Nit = nit;
            this.Ffactura = ffactura;
            this.Dscto = dscto;
            this.Puntos = puntos;
            this.Comision = comision;
            this.Fmaxcobro = fmaxcobro;
            this.Nroegreso = nroegreso;
            this.Fechapago = fechapago;
            this.Codcc = codcc;
            this.Fingreso = fingreso;
            this.Codven = codven;
            this.Subtotal = subtotal;
            this.Activo = activo;
            this.Idfactura = idfactura; 
            this.Syncronizado = syncronizado;
        }

        public string Grupo { get => grupo; set => grupo = value; }
        public string Nit { get => nit; set => nit = value; }
        public string Tipodctofa { get => tipodctofa; set => tipodctofa = value; }
        public string Factura { get => factura; set => factura = value; }
        public string Tipodcto { get => tipodcto; set => tipodcto = value; }
        public string Nrodcto { get => nrodcto; set => nrodcto = value; }
        public string Ffactura { get => ffactura; set => ffactura = value; }
        public decimal Dscto { get => dscto; set => dscto = value; }
        public decimal Puntos { get => puntos; set => puntos = value; }
        public double Comision { get => comision; set => comision = value; }
        public string Fmaxcobro { get => fmaxcobro; set => fmaxcobro = value; }
        public string Nroegreso { get => nroegreso; set => nroegreso = value; }
        public string Fechapago { get => fechapago; set => fechapago = value; }
        public string Codcc { get => codcc; set => codcc = value; }
        public int Activo { get => activo; set => activo = value; }
        public bool Syncronizado { get => syncronizado; set => syncronizado = value; }
        public long Idfactura { get => idfactura; set => idfactura = value; }
        public string Fingreso { get => fingreso; set => fingreso = value; }
        public string Codven { get => codven; set => codven = value; }
        public double Subtotal { get => subtotal; set => subtotal = value; }        
        
    }
}
