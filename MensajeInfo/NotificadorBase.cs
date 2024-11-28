using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MensajeInfo
{
    public class NotificadorBase : INotificando
    {
        public bool AdicionarFechaMensaje { get; set; } = true;
        public bool GenerarErrorProcesarMensajeSinAsignar { get; set; } = true;
        public Action<Mensaje> ProcesarMensaje;
        public void Notificar(TipoMensaje tipo, string mensaje, bool generarSalto = false)
        {

            if (ProcesarMensaje == null) {
                if (GenerarErrorProcesarMensajeSinAsignar) {
                    throw new Exception("No se ha establecido el action para el despliegue de mensajes");
                }
                return;
            }

            if (tipo != TipoMensaje.Salto)
            {
                if (string.IsNullOrEmpty(mensaje) && tipo != TipoMensaje.Salto)
                {
                    return;
                }
                if (AdicionarFechaMensaje)
                {
                    mensaje = $"{DateTime.Now}: {mensaje}";
                }
            }

            ProcesarMensaje(new Mensaje(tipo, mensaje));

            if (generarSalto) {
                ProcesarMensaje(new Mensaje(TipoMensaje.Salto, String.Empty));
            }
        }


        public void NotificarSalto()
        {
            Notificar(TipoMensaje.Salto, " ");
        }
        public void Notificar(string mensaje, bool generarSalto = false)
        {
            Notificar(TipoMensaje.Normal, mensaje, generarSalto);
        }

        public void NotificarError(string mensaje, bool generarSalto = false)
        {
            Notificar(TipoMensaje.Error, mensaje, generarSalto);
        }
        public void NotificarError(Exception ex, bool generarSalto = false)
        {
            Notificar(TipoMensaje.Error, ex.Message, generarSalto);
        }
        public void NotificarOk(string mensaje, bool generarSalto = false)
        {
            Notificar(TipoMensaje.Ok, mensaje, generarSalto);
        }
        public void NotificarWarning(string mensaje, bool generarSalto = false)
        {
            Notificar(TipoMensaje.Warning, mensaje, generarSalto);
        }
    }
}
