// See https://aka.ms/new-console-template for more information
using ClubExportDll;
using MensajeInfo;
using System;

namespace ClubExportConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {

                ClubProfesionales club = new ClubProfesionales();
                club.ProcesarMensaje = ProsesarMessage;
                club.Syncronizar();

            }
            catch (Exception ex)
            {
                ProsesarMessage(new Mensaje(TipoMensaje.Error,ex.Message));
            }
        }

        private static void ProsesarMessage(Mensaje message)
        {
            //Console.BackgroundColor = ConsoleColor.Blue;
            //Console.WriteLine("White on blue.");
            //Console.WriteLine("Another line.");

            if (message.Tipo == TipoMensaje.Salto) {
                Console.WriteLine("");
            }
                    
            switch (message.Tipo)
            {
                case TipoMensaje.Normal:
                    break;
                case TipoMensaje.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case TipoMensaje.Ok:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case TipoMensaje.Warning:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                default:
                    break;
            }
            Console.WriteLine(message.Comentario);
            Console.ResetColor();
        }
    }
}