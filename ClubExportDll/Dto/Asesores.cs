using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubExportDll.Dto
{
    public class Asesores
    {
        private String codven;
        private String asesor;
        private String telefono;
        private String email;
        private String codcc;
        private int activo;
        private Boolean syncronizado;

        public Asesores(string codven, string asesor, string telefono, string email, string codcc, int activo, Boolean syncronizado)
        {
            this.Codven = codven;
            this.Asesor = asesor;
            this.Telefono = telefono;
            this.Email = email;
            this.Codcc = codcc;
            this.Activo = activo;
            this.Syncronizado = syncronizado;
            
        }

        public string Codven { get => codven; set => codven = value; }
        public string Asesor { get => asesor; set => asesor = value; }
        public string Telefono { get => telefono; set => telefono = value; }
        public string Email { get => email; set => email = value; }
        public string Codcc { get => codcc; set => codcc = value; }
        public int Activo { get => activo; set => activo = value; }
        public bool Syncronizado { get => syncronizado; set => syncronizado = value; }
    }
}
