using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubExportDll.Dto
{
    public class Profesionales
    {

        private String nit;
        private Int32  carnet;
        private String profesional;
        private String direccion;
        private String telefono;
        private String celular;
        private String email;
        private String codven;
        private int activo;
        private Boolean syncronizado;


        public Profesionales(String nit, Int32 carnet, String profesional,String direccion,String telefono,String celular, String email, String codven,int activo, Boolean syncronizado) {

            this.nit         = nit;
            this.carnet      = carnet;
            this.profesional = profesional;
            this.direccion   = direccion;
            this.telefono    = telefono;
            this.celular     = celular;
            this.email       = email;
            this.codven      = codven;
            this.activo      = activo;
            this.syncronizado = syncronizado;  
        }

        public string Nit { get => nit; set => nit = value; }
        public Int32 Carnet { get => carnet; set => carnet = value; }
        public string Profesional { get => profesional; set => profesional = value; }
        public string Direccion { get => direccion; set => direccion = value; }
        public string Telefono { get => telefono; set => telefono = value; }
        public string Celular { get => celular; set => celular = value; }
        public string Email { get => email; set => email = value; }
        public string Codven { get => codven; set => codven = value; }
        public int Activo { get => activo; set => activo = value; }
        public bool Syncronizado { get => syncronizado; set => syncronizado = value; }
    }
}
