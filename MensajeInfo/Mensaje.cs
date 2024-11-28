
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MensajeInfo
{
    public class Mensaje
    {
        public TipoMensaje Tipo { get; set; }
        public string Comentario { get; set; }

        public Mensaje(TipoMensaje tipo, string comentario)
        {
            Tipo = tipo;
            Comentario = comentario;
        }
        public Mensaje(string comentario)
        {
            Tipo = TipoMensaje.Normal;
            Comentario = comentario;
        }
    }
}
