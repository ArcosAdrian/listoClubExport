
using Renci.SshNet;
using MySqlConnector;
using ClubExportDll;

namespace WinFormsApp1
{
    public partial class FrmPrincipal : Form
    {
        public FrmPrincipal()
        {
            InitializeComponent();
        }

        private void BtnExportar_Click(object sender, EventArgs e)
        {
            try
            {

                ClubProfesionales club = new ClubProfesionales();
                club.ProcesarMensaje = ProsesarMessage;
                club.Syncronizar();

            }
            catch (Exception ex)
            {
                var errror = ex.Message;
                throw;
            }
        }

        private  void ProsesarMessage(MensajeInfo.Mensaje message)
        {
            try
            {
                if(message.Tipo == MensajeInfo.TipoMensaje.Salto) 
                {
                    rtbMensajes.AppendText("\n");
                    return;
                }

                if (String.IsNullOrEmpty(message.Comentario)) 
                {
                    return;
                }
                        
                var color = Color.Black;
                var mensaje = message.Comentario + "\n";
                switch (message.Tipo)
                {
                   
                    case MensajeInfo.TipoMensaje.Normal:
                        rtbMensajes.AppendText(message.Comentario);
                        return;
                    case MensajeInfo.TipoMensaje.Error:
                        color = Color.Red;
                        break;
                    case MensajeInfo.TipoMensaje.Ok:
                        color = Color.Green;
                        break;
                    case MensajeInfo.TipoMensaje.Warning:
                        color = Color.Orange;
                        break;
                    
                    default:
                        break;
                }

                int length = rtbMensajes.TextLength; 
                rtbMensajes.AppendText(mensaje);
                rtbMensajes.SelectionStart = length;
                rtbMensajes.SelectionLength = mensaje.Length;
                rtbMensajes.SelectionColor = color;
                
            }
            catch (Exception ex)
            {
                rtbMensajes.AppendText(ex.Message);
            }
           
        }
    }
}
