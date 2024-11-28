using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubExportDll.Dto
{
    public class Sucursales
    {
        private String  codcc;
        private String  nombre;
        private String  direccion;
        private String  telefono;
        private int     activo;
        private Boolean syncronizado;

        public Sucursales(String codcc, String nombre, String direccion, String telefono, int activo, Boolean syncronizado) {
            this.codcc = codcc;
            this.nombre = nombre;
            this.direccion = direccion;
            this.telefono = telefono;
            this.activo = activo;
            this.Syncronizado = syncronizado;
        }

        public string Codcc { get => codcc; set => codcc = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string Direccion { get => direccion; set => direccion = value; }
        public string Telefono { get => telefono; set => telefono = value; }
        public int Activo { get => activo; set => activo = value; }
        public bool Syncronizado { get => syncronizado; set => syncronizado = value; }
    }
}

