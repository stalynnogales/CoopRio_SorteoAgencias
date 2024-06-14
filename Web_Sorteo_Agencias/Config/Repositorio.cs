using Oracle.ManagedDataAccess.Client;
using Web_Sorteo_Agencias.Models;

namespace Web_Sorteo_Agencias.Config
{
    public class Repositorio
    {
        string oradb_PROD = "DATA SOURCE=192.168.1.194:1521/topazpro;PASSWORD=;USER ID=RIOBAMBA;PERSIST SECURITY INFO=True;";
        string oradb_DESA = "DATA SOURCE=10.200.50.137:1521/BASE;PASSWORD=RIOBAMBA;USER ID=RIOBAMBA;PERSIST SECURITY INFO=True;";

        private List<Sucursal> listadoSucursales = new List<Sucursal>();

        // Rango de fechas
        public string fechaInicial = "01/12/2023";
        public string fechaFinal = "14/05/2024";

        public Repositorio()
        {
            OracleConnection con = new OracleConnection(oradb_DESA);
            OracleCommand cmd = new OracleCommand();
            //cmd.CommandText = "SELECT * from SUCURSALESSC WHERE SUCURSAL NOT IN(99,20) ORDER BY SUCURSAL";
            //cmd.CommandText = $"SELECT S.DESCRIPCION, C.C1960 SUCURSAL, COUNT(*) NUMEROSOCIOS FROM CL_CLIENTES C, CL_CLIENTPERSONA CP, SUCURSALESSC S WHERE S.SUCURSAL=C.C1960 AND C.C0902=CP.C1430 AND C.C1038='V' AND C.C1069>= TO_DATE('{fechaInicial}','DD/MM/YYYY') AND C.C1069<= TO_DATE('{fechaFinal}','DD/MM/YYYY') GROUP BY S.DESCRIPCION, C.C1960 HAVING C.C1960 NOT IN(99) ORDER BY C.C1960";
            cmd.CommandText = $@"SELECT S.DESCRIPCION, U.NROSUCURSAL SUCURSAL, COUNT(*) NUMEROSOCIOS 
                                FROM CL_CLIENTES C, SUCURSALESSC S, USUARIOS U
                                WHERE S.SUCURSAL=U.NROSUCURSAL
                                AND C.C0902 NOT IN (SELECT pr.c6287 FROM pr_proveedores pr WHERE pr.tz_lock=0)
                                AND C.C1038='V' 
                                AND C.C1053= U.INICIALES
                                AND C.TZ_LOCK = 0
                                AND (C.C1053 IS NOT NULL OR C.C1053=' ')
                                AND C.C1069 BETWEEN to_date('{fechaInicial}','DD/MM/YYYY') AND to_date('{fechaFinal}','DD/MM/YYYY')
                                GROUP BY S.DESCRIPCION, U.NROSUCURSAL 
                                ORDER BY U.NROSUCURSAL";
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
                cmd.CommandText = $"SELECT count(*) TOTAL FROM TEMP_SORTEO_AGENCIAS WHERE SUCURSAL={suc}";
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
            cmd.CommandText = $"SELECT * FROM TEMP_SORTEO_AGENCIAS WHERE SUCURSAL={codigo} ORDER BY SECUENCIAL";
            cmd.Connection = con;
            con.Open();
            OracleDataReader qr = cmd.ExecuteReader();
            while (qr.Read())
            {
                var numsoc = qr["NUMEROSOCIO"].ToString();
                var soc = qr["NOMBRESOCIO"].ToString();
                var ced = qr["CEDULA"].ToString()!;
                var fecact = Convert.ToDateTime(qr["FECHACTUALIZACION"].ToString())!;
                var est = qr["ESTADO"].ToString()!;
                var suc = Convert.ToInt32(qr["SUCURSAL"]);
                listadoSociosRegistrados.Add(new Socio(numsoc, soc, ced, fecact.ToString("dd/MM/yyyy"), est, Convert.ToInt32(suc)));

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
            //cmd.CommandText = $"SELECT C.C0902 NUMSOCIO, C.C1000 SOCIO, CP.C1432 CEDULA, C.C1069 FECHAACT, C1038 ESTADO, C1960 SUCURSAL FROM CL_CLIENTES C, CL_CLIENTPERSONA CP WHERE C.C0902=CP.C1430 AND C.C1038='V' AND C.C1069>= TO_DATE('{fechaInicial}','DD/MM/YYYY') AND C.C1069<= TO_DATE('{fechaFinal}','DD/MM/YYYY') AND C.C1960={sucursal} ORDER BY C.C1960";
            cmd.CommandText = $@"SELECT C.C0902 NUMSOCIO, C.C1000 SOCIO, CP.C1432 CEDULA, C.C1069 FECHAACT, C1038 ESTADO, C1960 SUCURSAL
                                    FROM CL_CLIENTES C, CL_CLIENTPERSONA CP, USUARIOS U
                                    WHERE C.C0902=CP.C1430
                                    AND C.C0902 NOT IN (SELECT pr.c6287 FROM pr_proveedores pr WHERE pr.tz_lock=0)
                                    AND C.C1038='V'
                                    AND C.C1053= U.INICIALES
                                    AND C.TZ_LOCK = 0
                                    AND (C.C1053 IS NOT NULL OR C.C1053=' ')
                                    AND C.C1069 BETWEEN to_date('{fechaInicial}','DD/MM/YYYY') AND to_date('{fechaFinal}','DD/MM/YYYY')
                                    AND U.NROSUCURSAL IN ({sucursal}" + (sucursal == 1 ? ", 99" : "") + ")  ORDER BY C.C1960";
            cmd.Connection = con;
            con.Open();
            OracleDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                var numsoc = dr["NUMSOCIO"].ToString();
                var soc = dr["SOCIO"].ToString();
                var ced = dr["CEDULA"].ToString();
                var fecact = Convert.ToDateTime(dr["FECHAACT"].ToString())!;
                var est = dr["ESTADO"].ToString() == "V" ? "VIGENTE" : "";
                var suc = dr["SUCURSAL"];
                listadoSocios.Add(new Socio(numsoc, soc, ced, fecact.ToString("dd/MM/yyyy"), est, Convert.ToInt32(suc)));
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
                cmd2.CommandText = $"INSERT INTO TEMP_SORTEO_AGENCIAS VALUES (0,'{socioAleatorio.NumeroSocio}', '{socioAleatorio.NombreSocio}', '{socioAleatorio.Cedula}', '{socioAleatorio.FechaActualizacion}', 'Descartado', {sucursal})";
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
            cmd.CommandText = $"UPDATE TEMP_SORTEO_AGENCIAS SET ESTADO='Ganador' WHERE SUCURSAL = {codigo} AND NUMEROSOCIO = '{numeroSocio}'";
            cmd.Connection = con;
            con.Open();
            OracleDataReader dr = cmd.ExecuteReader();
            con.Close();

            return true;
        }

    }
}
