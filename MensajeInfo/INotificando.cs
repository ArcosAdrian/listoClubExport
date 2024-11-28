using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MensajeInfo
{
    public interface INotificando
    {
        void NotificarSalto();
        void Notificar(MensajeInfo.TipoMensaje tipo, string mensaje, bool generarSalto = false);
        void Notificar(string mensaje, bool generarSalto = false);
        void NotificarError(string mensaje, bool generarSalto = false);
        void NotificarError(Exception ex, bool generarSalto = false);
        void NotificarOk(string mensaje, bool generarSalto = false);
        void NotificarWarning(string mensaje, bool generarSalto = false);

    }
}
