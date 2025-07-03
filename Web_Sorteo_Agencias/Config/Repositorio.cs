using Oracle.ManagedDataAccess.Client;
using Web_Sorteo_Agencias.Models;

namespace Web_Sorteo_Agencias.Config
{
    public class Repositorio
    {
        string oradb_PROD = "DATA SOURCE=192.168.1.194:1521/topazpro;PASSWORD=;USER ID=RIOBAMBA;PERSIST SECURITY INFO=True;";
        string oradb_DESA = "DATA SOURCE=10.200.50.137:1521/desarrollo;PASSWORD=sifizsoft;USER ID=system;PERSIST SECURITY INFO=True;";

        private List<Sucursal> listadoSucursales = new List<Sucursal>();

        // Rango de fechas
        public string fechaInicial = "01/12/2023";
        public string fechaFinal = "14/05/2024";

        public Repositorio()
        {
            OracleConnection con = new OracleConnection(oradb_DESA);
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = $@"SELECT 1 AS SUCURSAL, 'AGENCIAS' AS DESCRIPCION, count(*) NUMEROSOCIOS FROM FBS_CAPTACIONESVISTA.CUENTAMAESTRO c 
                INNER JOIN FBS_RIFAS.RIFAMAESTRO r ON r.NUMEROCUENTA = c.CODIGO
                WHERE r.SECUENCIALCONFIGURACIONRIFA = 7 AND c.CODIGOESTADO = 'A'";
            cmd.Connection = con;
            con.Open();
            OracleDataReader dr = cmd.ExecuteReader();

            listadoSucursales.Add(new Sucursal(0, "Seleccione una agencia", 0, 0));
            while (dr.Read())
            {
                var suc = dr["SUCURSAL"];
                var des = dr["DESCRIPCION"].ToString();
                var numsoc = dr["NUMEROSOCIOS"];
                var est = 0;
                cmd.CommandText = $"SELECT count(*) TOTAL FROM FBS_RIFAS.TEMP_SORTEO_AGENCIAS WHERE SUCURSAL={suc}";
                OracleDataReader qr = cmd.ExecuteReader();
                if (qr.Read())
                {
                    est = Convert.ToInt32(qr["TOTAL"]) > 0 ? 0 : 0;
                }
                listadoSucursales.Add(new Sucursal(Convert.ToInt32(suc), des, est, Convert.ToInt32(numsoc)));
            }
            var numeroAdministrativo = listadoSucursales.Find(x => x.SUCURSAL == 99)?.NUMERO;
            listadoSucursales.Where(x => x.SUCURSAL == 1).Sum(x =>
            {
                x.NUMERO += numeroAdministrativo ?? 0;
                return x.NUMERO; 
            });
            listadoSucursales.RemoveAll(x => x.SUCURSAL == 99);
            con.Close();
        }

        public IEnumerable<Sucursal> ListadoSucursales()
        {
            return this.listadoSucursales;
        }

        public Sucursal BuscarSucursal(long codigo)
        {
            return listadoSucursales.Find(c => c.SUCURSAL == codigo)!;
        }
        public List<Socio> BuscarSociosSucursales(long codigo)
        {
            List<Socio> listadoSociosRegistrados = new List<Socio>();
            OracleConnection con = new OracleConnection(oradb_DESA);
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = $"SELECT * FROM FBS_RIFAS.TEMP_SORTEO_AGENCIAS WHERE SUCURSAL={codigo} ORDER BY SECUENCIAL";
            cmd.Connection = con;
            con.Open();
            OracleDataReader qr = cmd.ExecuteReader();
            while (qr.Read())
            {
                var numsoc = qr["NUMEROSOCIO"].ToString();
                var soc = qr["NOMBRESOCIO"].ToString();
                var nomcuenta = qr["NOMBRECUENTA"].ToString();
                var ced = qr["CEDULA"].ToString()!;
                var est = qr["ESTADO"].ToString()!;
                var suc = Convert.ToInt32(qr["SUCURSAL"]);
                listadoSociosRegistrados.Add(new Socio(numsoc, soc, nomcuenta, ced, est));

            }
            return listadoSociosRegistrados;
        }

        public Sucursal ActualizarEstadosucursal(long codigo)
        {
            Sucursal sucursal = listadoSucursales.Find(c => c.SUCURSAL == codigo)!;
            if (sucursal != null)
            {
                sucursal.ESTADO = 1;
            }
            return sucursal!;
        }

        public IEnumerable<Socio> ListadoSociosSucursal(int sucursal)
        {
            List<Socio> listadoSocios = new List<Socio>();
            List<Socio> listadoSociosSeleccionados = new List<Socio>();

            listadoSocios.Clear();
            OracleConnection con = new OracleConnection(oradb_DESA);
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = $@"SELECT c2.NUMEROCLIENTE NUMSOCIO, p.NOMBREUNIDO SOCIO, cp.NOMBRECUENTA NOMBRECUENTA, p.IDENTIFICACION CEDULA, 1 SUCURSAL
                    FROM FBS_CAPTACIONESVISTA.CUENTAMAESTRO c 
                    INNER JOIN FBS_CAPTACIONESVISTA.CUENTAMAESTRO_PERSONALIZADO cp ON CP.SECUENCIALCUENTA = C.SECUENCIAL 
                    INNER JOIN FBS_CLIENTES.CLIENTE c2 ON C2.SECUENCIAL = C.SECUENCIALCLIENTEPRINCIPAL 
                    INNER JOIN FBS_PERSONAS.PERSONA p ON P.SECUENCIAL = C2.SECUENCIALPERSONA 
                    INNER JOIN FBS_RIFAS.RIFAMAESTRO r ON r.NUMEROCUENTA = c.CODIGO
                    WHERE r.SECUENCIALCONFIGURACIONRIFA = 7 AND c.CODIGOESTADO = 'A'";
            cmd.Connection = con;
            con.Open();
            OracleDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                var numsoc = dr["NUMSOCIO"].ToString();
                var soc = dr["SOCIO"].ToString();
                var nomcuenta = dr["NOMBRECUENTA"].ToString();
                var ced = dr["CEDULA"].ToString();
                var suc = dr["SUCURSAL"];
                listadoSocios.Add(new Socio(numsoc, soc, nomcuenta, ced, Convert.ToInt32(suc)));
            }
            con.Close();

            var listadoSociosAnterior = BuscarSociosSucursales(sucursal);

            var seleccion = new Socio();
            var indexBtn = 0;
            do
            {
                Random r1 = new Random();
                indexBtn = r1.Next(listadoSocios.Count);
                seleccion = listadoSociosAnterior.Find(s => s.NumeroSocio == listadoSocios[indexBtn].NumeroSocio);
            } while (seleccion is not null);
         
            if (seleccion is null)
            {
                var socioAleatorio = listadoSocios[indexBtn];
                OracleConnection con2 = new OracleConnection(oradb_DESA);
                OracleCommand cmd2 = new OracleCommand();
                cmd2.CommandText = $"INSERT INTO FBS_RIFAS.TEMP_SORTEO_AGENCIAS VALUES (0,'{socioAleatorio.NumeroSocio}', '{socioAleatorio.NombreSocio}', '{socioAleatorio.NombreCuenta}', '{socioAleatorio.Cedula}', '{socioAleatorio.FechaActualizacion}', 'Descartado', {sucursal})";
                cmd2.Connection = con2;
                con2.Open();
                OracleDataReader dr2 = cmd2.ExecuteReader();
                con2.Close();
            }

            listadoSociosSeleccionados = BuscarSociosSucursales(sucursal);

            return listadoSociosSeleccionados;
        }

        public bool GuardarGanadorAgencia(long codigo, string numeroSocio)
        {
            OracleConnection con = new OracleConnection(oradb_DESA);
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = $"UPDATE FBS_RIFAS.TEMP_SORTEO_AGENCIAS SET ESTADO='Ganador' WHERE SUCURSAL = {codigo} AND NUMEROSOCIO = '{numeroSocio}'";
            cmd.Connection = con;
            con.Open();
            OracleDataReader dr = cmd.ExecuteReader();
            con.Close();

            return true;
        }

    }
}
