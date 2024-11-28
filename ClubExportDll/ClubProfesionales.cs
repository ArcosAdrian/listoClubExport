using Renci.SshNet;

using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Text;
using ClubExportDll.Dto;
using MensajeInfo;

namespace ClubExportDll
{
    public class ClubProfesionales : NotificadorBase
    {

        public void Syncronizar()
        {
            Notificar(TipoMensaje.Ok, "INICIO");
            Notificar("ClubProfesionales-Syncronizar");
            SqlConnection sqlConn = null;

            try
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings["connectionStandar"].ConnectionString;

                var databaseUserName = "listaelf_pruebas";
                var databasePassword = "lisdm4estr0*80&";
                var databaseServer = "127.0.0.1";
                var Database = "listaelf_TODACOSA";

                var sshServer = "server216.web-hosting.com";
                var sshPort = 21098;
                var sshUserName = "listaelf";
                var sshPassword = "*listoPinguino80%";

                sqlConn = new SqlConnection(ConnectionString);
                sqlConn.Open();
                
                Notificar("Obteniendo Susursales");
                var sucursales = ObtenerSucursales(sqlConn);
                Notificar($"{sucursales.Count} Susursales a procesar");
                
                Notificar("Obteniendo Profesionales");
                var profesionales = ObtenerProfesionales(sqlConn);
                Notificar($"{profesionales.Count} Profesionales a procesar");
                
                Notificar("Obteniendo Asesores");
                var asesores = ObtenerAsesores(sqlConn);
                Notificar($"{asesores.Count} Asesores a procesar");
                
                Notificar("Obteniendo Facturas");
                var facturas = ObtenerFacturas(sqlConn);

                Notificar($"{facturas.Count} Facturas a procesar");

                sqlConn.Close();

                NotificarSalto();
                if (sucursales.Count == 0 && profesionales.Count == 0 && asesores.Count == 0 && facturas.Count == 0) 
                {
                    NotificarOk("No hay nada por Syncronizar");
                    return;
                }


                
                Notificar("Obteniendo client SSH");

                var (sshClient, localPort) = ConnectSsh(sshServer, sshUserName, sshPassword, databaseServer: databaseServer, sshPort: sshPort);
                using (sshClient)
                {

                    string connStr = $"server={databaseServer};user={databaseUserName};database={Database};password={databasePassword};port={localPort};";
                    using (var myConn = new MySqlConnection(connStr))
                    {
                        myConn.Open();

                        Notificar(TipoMensaje.Warning, "Actualizar Sucursales (mysql)");
                        ActualizarSucursales(sucursales, myConn);

                        Notificar(TipoMensaje.Warning, "Actualizar Profesionales (mysql)");
                        ActualizarProfesionales(profesionales, myConn);

                        Notificar(TipoMensaje.Warning, "Actualizar Asesores (mysql)");
                        ActualizarAsesores(asesores, myConn);

                        Notificar(TipoMensaje.Warning, "Actualizar Facturas (mysql)");
                        ActualizarFacturas(facturas, myConn);
                    }

                }

                sqlConn.Open();

                //Notificar("Confirmadando Sucursales");
                //ActualizarSucursalesConfirmadas(sucursales, sqlConn);

                Notificar(TipoMensaje.Warning,"Confirmadando Profesionales");
                ActualizarProfesionalesConfirmados(profesionales, sqlConn);

                Notificar(TipoMensaje.Warning, "Confirmadando Asesores");
                ActualizarAsesoresConfirmados(asesores, sqlConn);

                Notificar(TipoMensaje.Warning, "Confirmadando Facturas");
                
                ActualizarFacturasConfirmadas(facturas, sqlConn);

                sqlConn.Close();
            }
            catch (Exception ex)
            {
                NotificarError(ex);            
            }
            finally {
                if (sqlConn != null && sqlConn.State != ConnectionState.Closed) 
                {
                    sqlConn.Close();
                }
                Notificar(TipoMensaje.Ok, "FIN");
            }
        }

 

        public (SshClient SshClient, uint Port) ConnectSsh(string sshHostName, string sshUserName, string sshPassword = null, string sshKeyFile = null, string sshPassPhrase = null, int sshPort = 22, string databaseServer = "localhost", int databasePort = 3306)
        {
            // check arguments
            if (string.IsNullOrEmpty(sshHostName))
                throw new ArgumentException($"{nameof(sshHostName)} must be specified.", nameof(sshHostName));
            if (string.IsNullOrEmpty(sshHostName))
                throw new ArgumentException($"{nameof(sshUserName)} must be specified.", nameof(sshUserName));
            if (string.IsNullOrEmpty(sshPassword) && string.IsNullOrEmpty(sshKeyFile))
                throw new ArgumentException($"One of {nameof(sshPassword)} and {nameof(sshKeyFile)} must be specified.");
            if (string.IsNullOrEmpty(databaseServer))
                throw new ArgumentException($"{nameof(databaseServer)} must be specified.", nameof(databaseServer));

            // define the authentication methods to use (in order)
            var authenticationMethods = new List<AuthenticationMethod>();
            if (!string.IsNullOrEmpty(sshKeyFile))
            {
                authenticationMethods.Add(new PrivateKeyAuthenticationMethod(sshUserName,
                    new PrivateKeyFile(sshKeyFile, string.IsNullOrEmpty(sshPassPhrase) ? null : sshPassPhrase)));
            }
            if (!string.IsNullOrEmpty(sshPassword))
            {
                authenticationMethods.Add(new PasswordAuthenticationMethod(sshUserName, sshPassword));
            }

            // connect to the SSH server
            var sshClient = new SshClient(new ConnectionInfo(sshHostName, sshPort, sshUserName, authenticationMethods.ToArray()));
            sshClient.Connect();

            // forward a local port to the database server and port, using the SSH server
            var forwardedPort = new ForwardedPortLocal("127.0.0.1", databaseServer, (uint)databasePort);
            sshClient.AddForwardedPort(forwardedPort);
            forwardedPort.Start();

            return (sshClient, forwardedPort.BoundPort);
        }

        public string RemoveSpecialCharacters(string strx)
        {
            string str = strx.Trim();
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == ' ')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString().Trim();
        }


        #region Sucursales
        public List<Sucursales> ObtenerSucursales(SqlConnection conn)
        {

            List<Sucursales> sucursales = new List<Sucursales>();

            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandText = "LST_CPAPPS_MANAGER_PROFESIONALES";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 60;
            command.Parameters.AddWithValue("@tarea", 101);
            command.Parameters.AddWithValue("@idstandar", 0);
            command.Parameters.AddWithValue("@codstandar", "");


            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    String codcc = reader.GetString(0).ToString();
                    String nombre = reader.GetString(1).ToString();
                    String direccion = reader.GetString(2).ToString();
                    String telefono = reader.GetString(3).ToString();
                    int activo = reader.GetInt32(4);

                    sucursales.Add(new Sucursales(codcc, nombre, direccion, telefono, activo, false));

                }
            }

            return sucursales;

        }
        public void ActualizarSucursales(List<Sucursales> sucursales, MySqlConnection conn)
        {

            try
            {

                //MySqlCommand sqlCommand = new MySqlCommand();
                //sqlCommand.Connection = conn;
                //sqlCommand.CommandText = "SET character_set_results=utf8";
                //sqlCommand.ExecuteNonQuery();

                for (int r = 0; r < sucursales.Count; r++)
                {
                    Notificar($"Actualizando {r + 1}/{sucursales.Count} Sucursales");
                    Sucursales sucursal = sucursales[r];
                    String codcc = sucursal.Codcc;
                    String nombre = sucursal.Nombre;
                    String direccion = sucursal.Direccion;
                    String telefono = sucursal.Telefono;
                    int activo = sucursal.Activo;

                    string rtn = "syncro_cp_sucursales_p";
                    MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(rtn, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("pcodcc", codcc);
                    cmd.Parameters.AddWithValue("pnombre", nombre);
                    cmd.Parameters.AddWithValue("pdireccion", direccion);
                    cmd.Parameters.AddWithValue("ptelefono", telefono);
                    cmd.Parameters.AddWithValue("pactivo", activo);
                    cmd.ExecuteNonQuery();
                    sucursales[r].Syncronizado = true;
                }
            }
            catch (MySqlException ex)
            {
                var error = ex.Message;
                throw;
            }
        }
        public void ActualizarSucursalesConfirmadas(List<Sucursales> sucursales, SqlConnection conn)
        {

            try
            {
                for (int r = 0; r < sucursales.Count; r++)
                {

                    if (sucursales[r].Syncronizado == false)
                    {
                        continue;
                    }

                    Sucursales sucursal = sucursales[r];
                    String codcc = sucursal.Codcc;

                    SqlCommand Command = new SqlCommand();
                    Command.Connection = conn;
                    Command.CommandText = "LST_CPAPPS_MANAGER_PROFESIONALES";
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.CommandTimeout = 60;
                    Command.Parameters.AddWithValue("@tarea", 201);
                    Command.Parameters.AddWithValue("@idstandar", 0);
                    Command.Parameters.AddWithValue("@codstandar", codcc);
                    Command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                var error = e.Message;
                throw;
            }

        }
        #endregion

        #region Profesionales   
        public List<Profesionales> ObtenerProfesionales(SqlConnection conn)
        {

            List<Profesionales> profesionales = new List<Profesionales>();

            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandText = "LST_CPAPPS_MANAGER_PROFESIONALES";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 60;
            command.Parameters.AddWithValue("@tarea", 100);
            command.Parameters.AddWithValue("@idstandar", 0);
            command.Parameters.AddWithValue("@codstandar", "");


            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    String nit = reader.GetString(0).ToString();
                    Int32 carnet = reader.GetInt32(1);
                    String profesional = reader.GetString(2).ToString();
                    String direccion = reader.GetString(3).ToString();
                    String telefono = reader.GetString(4).ToString();
                    String celular = reader.GetString(5).ToString();
                    String email = reader.GetString(6).ToString();
                    String codven = reader.GetString(7).ToString();
                    int activo = reader.GetInt32(8);

                    profesionales.Add(new Profesionales(nit, carnet, profesional, direccion, telefono, celular, email, codven, activo, false));

                }
            };

            return profesionales;

        }
        public void ActualizarProfesionales(List<Profesionales> profesionales, MySqlConnection conn)
        {

            try
            {

                MySqlCommand sqlCommand = new MySqlCommand();
                sqlCommand.Connection = conn;
                sqlCommand.CommandText = "SET character_set_results=utf8";
                sqlCommand.ExecuteNonQuery();

                for (int r = 0; r < profesionales.Count; r++)
                {
                    Notificar($"Actualizando {r + 1}/{profesionales.Count} Profesionales");
                    Profesionales profesional = profesionales[r];

                    String nit = RemoveSpecialCharacters(profesional.Nit);
                    int carnet = profesional.Carnet;
                    String nombre = RemoveSpecialCharacters(profesional.Profesional);
                    String direccion = RemoveSpecialCharacters(profesional.Direccion);
                    String telefono = RemoveSpecialCharacters(profesional.Telefono);
                    String celular = RemoveSpecialCharacters(profesional.Celular);
                    String email = RemoveSpecialCharacters(profesional.Email);
                    String codven = RemoveSpecialCharacters(profesional.Codven);
                    int activo = profesional.Activo;


                    string rtn = "syncro_cp_profesionales";
                    MySqlCommand cmd = new MySqlCommand(rtn, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("pnit", nit);
                    cmd.Parameters.AddWithValue("pcarnet", carnet);
                    cmd.Parameters.AddWithValue("pprofesional", nombre);
                    cmd.Parameters.AddWithValue("pdireccion", direccion);
                    cmd.Parameters.AddWithValue("ptelefono", telefono);
                    cmd.Parameters.AddWithValue("pcelular", celular);
                    cmd.Parameters.AddWithValue("pemail", email);
                    cmd.Parameters.AddWithValue("pcodven", codven);
                    cmd.Parameters.AddWithValue("pactivo", activo);

                    cmd.ExecuteNonQuery();
                    profesionales[r].Syncronizado = true;
                }
            }
            catch (MySqlException ex)
            {
                var error = ex.Message;
                throw;
            }
        }
        public void ActualizarProfesionalesConfirmados(List<Profesionales> profesionales, SqlConnection conn)
        {

            try
            {
                for (int r = 0; r < profesionales.Count; r++)
                {

                    if (profesionales[r].Syncronizado == false)
                    {
                        continue;
                    }

                    Profesionales profesional = profesionales[r];
                    int idprofesional = profesional.Carnet;

                    SqlCommand Command = new SqlCommand();
                    Command.Connection = conn;
                    Command.CommandText = "LST_CPAPPS_MANAGER_PROFESIONALES";
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.CommandTimeout = 60;
                    Command.Parameters.AddWithValue("@tarea", 200);
                    Command.Parameters.AddWithValue("@idstandar", idprofesional);
                    Command.Parameters.AddWithValue("@codstandar", "");
                    Command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                var error = e.Message;
                throw;
            }

        }
        #endregion

        #region Asesores
        public List<Asesores> ObtenerAsesores(SqlConnection conn)
        {

            List<Asesores> asesores = new List<Asesores>();



            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandText = "LST_CPAPPS_MANAGER_PROFESIONALES";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 60;
            command.Parameters.AddWithValue("@tarea", 102);
            command.Parameters.AddWithValue("@idstandar", 0);
            command.Parameters.AddWithValue("@codstandar", "");


            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    String codven = reader.GetString(0).ToString();
                    String asesor = reader.GetString(1).ToString();
                    String telefono = reader.GetString(2).ToString();
                    String email = reader.GetString(3).ToString();
                    String codcc = reader.GetString(4).ToString();
                    int activo = reader.GetInt32(5);

                    asesores.Add(new Asesores(codven, asesor, telefono, email, codcc, activo, false));

                }
            }

            return asesores;

        }
        public void ActualizarAsesores(List<Asesores> asesores, MySqlConnection conn)
        {

            try
            {

                //MySqlCommand sqlCommand = new MySqlCommand();
                //sqlCommand.Connection = conn;
                //sqlCommand.CommandText = "SET character_set_results=utf8";
                //sqlCommand.ExecuteNonQuery();

                for (int r = 0; r < asesores.Count; r++)
                {
                    Notificar($"Actualizando {r + 1}/{asesores.Count} Asesores");

                    Asesores asesor = asesores[r];

                    String codven = asesor.Codven;
                    String vendendor = asesor.Asesor;
                    String telefono = asesor.Telefono;
                    String email = asesor.Email;
                    String codcc = asesor.Codcc;
                    int activo = asesor.Activo;



                    string rtn = "syncro_cp_asesores";
                    MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(rtn, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("pcodven", codven);
                    cmd.Parameters.AddWithValue("pasesor", vendendor);
                    cmd.Parameters.AddWithValue("ptelefono", telefono);
                    cmd.Parameters.AddWithValue("pemail", email);
                    cmd.Parameters.AddWithValue("pcodcc", codcc);
                    cmd.Parameters.AddWithValue("pactivo", activo);
                    cmd.ExecuteNonQuery();
                    asesores[r].Syncronizado = true;
                }
            }
            catch (MySqlException ex)
            {
                var error = ex.Message;
                throw;
            }
        }
        public void ActualizarAsesoresConfirmados(List<Asesores> asesores, SqlConnection conn)
        {

            try
            {
                for (int r = 0; r < asesores.Count; r++)
                {

                    if (asesores[r].Syncronizado == false)
                    {
                        continue;
                    }

                    Asesores asesor = asesores[r];
                    String codasesor = asesor.Codven;

                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = "LST_CPAPPS_MANAGER_PROFESIONALES";
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 60;
                    command.Parameters.AddWithValue("@tarea", 202);
                    command.Parameters.AddWithValue("@idstandar", 0);
                    command.Parameters.AddWithValue("@codstandar", codasesor);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                var error = e.Message;
                throw;
            }

        }
        #endregion

        #region Facturas
        public List<Facturas> ObtenerFacturas(SqlConnection conn)
        {

            List<Facturas> facturas = new List<Facturas>();

            if (conn.State != ConnectionState.Open) 
            {
                conn.Open();
            }

            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandText = "LST_CPAPPS_MANAGER_PROFESIONALES";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 60;
            command.Parameters.AddWithValue("@tarea", 103);
            command.Parameters.AddWithValue("@idstandar", 0);
            command.Parameters.AddWithValue("@codstandar", "");


            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    String grupo = reader.GetString(0).ToString();
                    String tipodctofa = reader.GetString(1).ToString();
                    String factura = reader.GetString(2).ToString();
                    String tipodcto = reader.GetString(3).ToString();
                    String nrodcto = reader.GetString(4).ToString();
                    String nit = reader.GetString(5).ToString();
                    String ffactura = reader.GetString(6).ToString();
                    decimal dscto = reader.GetDecimal(7);
                    decimal puntos = reader.GetDecimal(8);
                    double comision = Convert.ToDouble(reader.GetDecimal(9));
                    String fmaxcobro = reader.GetString(10);
                    String nroegreso = reader.GetString(11);
                    String fechapago = reader.GetString(12);
                    String codcc = reader.GetString(13);
                    String fingreso = reader.GetString(14);
                    String codven = reader.GetString(15);
                    int activo = 1; // Reader.GetInt16(13);
                    double subtotal = Convert.ToDouble(reader.GetDecimal(17));
                    long idfactura = reader.GetInt32(18);


                    facturas.Add(new Facturas(grupo, tipodctofa, factura, tipodcto, nrodcto, nit, ffactura, dscto, puntos, comision, fmaxcobro, nroegreso, fechapago, codcc, fingreso, codven, subtotal, activo, idfactura, false));

                }
            }

            return facturas;

        }
        public void ActualizarFacturas(List<Facturas> facturas, MySqlConnection conn)
        {

            try
            {

                //MySqlCommand sqlCommand = new MySqlCommand();
                //sqlCommand.Connection = conn;
                //sqlCommand.CommandText = "SET character_set_results=utf8";
                //sqlCommand.ExecuteNonQuery();

                for (int r = 0; r < facturas.Count; r++)
                {
                    Notificar($"Actualizando {r + 1}/{facturas.Count} Facturas");

                    Facturas factura = facturas[r];
                    String grupo = factura.Grupo;
                    String tipodctofa = factura.Tipodctofa;
                    String nrofactura = factura.Factura;
                    String tipodcto = factura.Tipodcto;
                    String nrodcto = factura.Nrodcto;
                    String nit = factura.Nit;
                    String ffactura = factura.Ffactura;
                    decimal dscto = factura.Dscto;
                    decimal puntos = factura.Puntos;
                    double comision = factura.Comision;
                    String fmaxcobro = factura.Fmaxcobro;
                    String nroegreso = factura.Nroegreso;
                    String fechapago = factura.Fechapago;
                    String codcc = factura.Codcc;
                    String fingreso = factura.Fingreso;
                    int activo = factura.Activo;
                    String codven = factura.Codven;
                    double subtotal = factura.Subtotal;
                    long idfactura = factura.Idfactura;

                    string rtn = "syncro_cp_facturas";
                    MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(rtn, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("pidfactura", idfactura);
                    cmd.Parameters.AddWithValue("pgrupo", grupo);
                    cmd.Parameters.AddWithValue("ptipodctofa", tipodctofa);
                    cmd.Parameters.AddWithValue("pfactura", nrofactura);
                    cmd.Parameters.AddWithValue("ptipodcto", tipodcto);
                    cmd.Parameters.AddWithValue("pnrodcto", nrodcto);
                    cmd.Parameters.AddWithValue("pnit", nit);
                    cmd.Parameters.AddWithValue("pffactura", ffactura);
                    cmd.Parameters.AddWithValue("pdscto", dscto);
                    cmd.Parameters.AddWithValue("ppuntos", puntos);
                    cmd.Parameters.AddWithValue("pcomision", comision);
                    cmd.Parameters.AddWithValue("pfmaxcobro", fmaxcobro);
                    cmd.Parameters.AddWithValue("pnroegreso", nroegreso);
                    cmd.Parameters.AddWithValue("pfechapago", fechapago);
                    cmd.Parameters.AddWithValue("pcodcc", codcc);
                    cmd.Parameters.AddWithValue("pfingreso", fingreso);
                    cmd.Parameters.AddWithValue("pactivo", activo);
                    cmd.Parameters.AddWithValue("pcodven", codven);
                    cmd.Parameters.AddWithValue("psubtotal", subtotal);
                    cmd.ExecuteNonQuery();
                    facturas[r].Syncronizado = true;
                }
            }
            catch (MySqlException ex)
            {
                var error = ex.Message;
                throw;
            }
        }
        public void ActualizarFacturasConfirmadas(List<Facturas> facturas, SqlConnection conn)
        {

            try
            {
                for (int r = 0; r < facturas.Count; r++)
                {

                    if (facturas[r].Syncronizado == false)
                    {
                        continue;
                    }

                    if (facturas[r].Syncronizado == false)
                    {
                        continue;
                    }

                    Facturas factura = facturas[r];
                    long idfacturas = factura.Idfactura;

                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = "LST_CPAPPS_MANAGER_PROFESIONALES";
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 60;
                    command.Parameters.AddWithValue("@tarea", 203);
                    command.Parameters.AddWithValue("@idstandar", idfacturas);
                    command.Parameters.AddWithValue("@codstandar", "");
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                var error = e.Message;
                throw;
            }

        }

     
        #endregion






    }
}