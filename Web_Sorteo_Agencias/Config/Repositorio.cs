using Oracle.ManagedDataAccess.Client;
using Web_Sorteo_Agencias.Models;

namespace Web_Sorteo_Agencias.Config
{
    public class Repositorio
    {
        //string oradb_DESA = "DATA SOURCE=10.200.50.137:1521/desarrollo;PASSWORD=sifizsoft;USER ID=system;PERSIST SECURITY INFO=True;";
        string oradb_DESA = "DATA SOURCE=10.200.50.31:1521/FINANCIAL;PASSWORD=sifizsoft;USER ID=system;PERSIST SECURITY INFO=True;";

        private List<Sucursal> listadoSucursales = new List<Sucursal>();

        // Rango de fechas
        public string fechaInicial = "2025-11-01";
        public string fechaFinal = "2025-11-30";

        public Repositorio()
        {
            OracleConnection con = new OracleConnection(oradb_DESA);
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = $@"SELECT 
              sub.SECUENCIALOFICINA SECUENCIALDIVISION,
              ('CUENCA ARENAL TV') AS NOMBRE,
              COUNT(DISTINCT sub.SECUENCIALCLIENTEPRINCIPAL) AS NUMERO_SOCIOS,
              1 AS TIPO
            FROM (
	            SELECT 
	            --p.identificacion, pm.NUMEROPRESTAMO
	            pm.* 
	            FROM FBS_CARTERA.PRESTAMOMAESTRO pm 
	            INNER JOIN FBS_CLIENTES.CLIENTE c ON c.secuencial = pm.SECUENCIALCLIENTEPRINCIPAL
	            INNER JOIN FBS_PERSONAS.PERSONA p ON p.secuencial = c.secuencialpersona
	            WHERE pm.SECUENCIALOFICINA = 69475
	            AND pm.CODIGOESTADOPRESTAMO = 'A'
	            AND NOT EXISTS (
	                  SELECT 1
	                  FROM FBS_CARTERA.PRESTAMOCOMPONENTE_CARTERA pc
	                  WHERE pc.SECUENCIALPRESTAMO = pm.SECUENCIAL
	                    AND pc.DIASATRASO <> 0
	            )
	            AND NOT EXISTS (
		              SELECT 1 
		              FROM FBS_SEGURIDADES.USUARIO_COMPLEMENTO uc 
		              WHERE uc.SECUENCIALPERSONA = p.SECUENCIAL
	            )
	            AND PM.FECHAADJUDICACION BETWEEN TO_DATE('2026-01-01', 'YYYY-MM-DD') AND TO_DATE('2026-10-31', 'YYYY-MM-DD')
            ) sub  GROUP BY sub.SECUENCIALOFICINA
            UNION ALL
            SELECT 
              sub.SECUENCIALOFICINA SECUENCIALDIVISION,
              ('CUENCA ARENAL COCINA') AS NOMBRE,
              COUNT(DISTINCT sub.SECUENCIALCLIENTEPRINCIPAL) AS NUMERO_SOCIOS,
              2 AS TIPO
            FROM (
	            SELECT 
	            --p.identificacion, pm.NUMEROPRESTAMO
	            pm.* 
	            FROM FBS_CARTERA.PRESTAMOMAESTRO pm 
	            INNER JOIN FBS_CLIENTES.CLIENTE c ON c.secuencial = pm.SECUENCIALCLIENTEPRINCIPAL
	            INNER JOIN FBS_PERSONAS.PERSONA p ON p.secuencial = c.secuencialpersona
	            WHERE pm.SECUENCIALOFICINA = 69475
	            AND pm.CODIGOESTADOPRESTAMO = 'A'
	            AND pm.DEUDAINICIAL < 10000 
	            AND NOT EXISTS (
	                  SELECT 1
	                  FROM FBS_CARTERA.PRESTAMOCOMPONENTE_CARTERA pc
	                  WHERE pc.SECUENCIALPRESTAMO = pm.SECUENCIAL
	                    AND pc.DIASATRASO > 5
	            )
	            AND NOT EXISTS (
		              SELECT 1 
		              FROM FBS_SEGURIDADES.USUARIO_COMPLEMENTO uc 
		              WHERE uc.SECUENCIALPERSONA = p.SECUENCIAL
	            )
	            AND PM.FECHAADJUDICACION BETWEEN TO_DATE('2026-01-01', 'YYYY-MM-DD') AND TO_DATE('2026-10-31', 'YYYY-MM-DD')
            ) sub GROUP BY sub.SECUENCIALOFICINA
            UNION ALL 
            SELECT 
              sub.SECUENCIALOFICINA SECUENCIALDIVISION,
              ('CUENCA ARENAL LAVADORA') AS NOMBRE,
              COUNT(DISTINCT sub.SECUENCIALCLIENTEPRINCIPAL) AS NUMERO_SOCIOS,
              3 AS TIPO
            FROM (
	            SELECT 
	            --p.identificacion, D.codigo
	            d.* 
	            FROM FBS_CAPTACIONESPLAZO.DEPOSITOMAESTRO d 
	            INNER JOIN FBS_CLIENTES.CLIENTE c ON c.secuencial = d.SECUENCIALCLIENTEPRINCIPAL
	            INNER JOIN FBS_PERSONAS.PERSONA p ON p.secuencial = c.secuencialpersona
	            WHERE d.SECUENCIALOFICINA = 69475
	            AND d.PLAZOENDIAS < 360
	            AND d.CODIGOESTADODEPOSITO = 'A'
	            AND NOT EXISTS (
			              SELECT 1 
			              FROM FBS_SEGURIDADES.USUARIO_COMPLEMENTO uc 
			              WHERE uc.SECUENCIALPERSONA = p.SECUENCIAL
		            )
	            AND d.FECHACREACION BETWEEN TO_DATE('2026-01-01', 'YYYY-MM-DD') AND TO_DATE('2026-10-31', 'YYYY-MM-DD')
            ) sub GROUP BY sub.SECUENCIALOFICINA
            UNION ALL
            SELECT 
              sub.SECUENCIALOFICINA SECUENCIALDIVISION,
              ('CUENCA ARENAL BICICLETA') AS NOMBRE,
              COUNT(DISTINCT sub.SECUENCIALCLIENTEPRINCIPAL) AS NUMERO_SOCIOS,
              4 AS TIPO
            FROM (
	            SELECT 
	            --p.identificacion, CM.codigo
	            cm.* 
	            FROM FBS_CAPTACIONESVISTA.CUENTAMAESTRO cm
	            INNER JOIN FBS_CLIENTES.CLIENTE c ON c.secuencial = cm.SECUENCIALCLIENTEPRINCIPAL
	            INNER JOIN FBS_PERSONAS.PERSONA p ON p.secuencial = c.secuencialpersona
	            WHERE cm.SECUENCIALOFICINA = 69475
	            AND cm.CODIGOTIPOCUENTA = 12
	            AND cm.CODIGOESTADO = 'A'
	            AND NOT EXISTS (
				              SELECT 1 
				              FROM FBS_SEGURIDADES.USUARIO_COMPLEMENTO uc 
				              WHERE uc.SECUENCIALPERSONA = p.SECUENCIAL
	            )
	            AND cm.FECHASISTEMACREACION BETWEEN TO_DATE('2026-01-01', 'YYYY-MM-DD') AND TO_DATE('2026-10-31', 'YYYY-MM-DD')
            ) sub GROUP BY sub.SECUENCIALOFICINA
            UNION ALL
            SELECT 
              sub.SECUENCIALOFICINA SECUENCIALDIVISION,
              ('CUENCA CENTRO TV') AS NOMBRE,
              COUNT(DISTINCT sub.SECUENCIALCLIENTEPRINCIPAL) AS NUMERO_SOCIOS,
              1 AS TIPO
            FROM (
	            SELECT 
	            --p.identificacion, pm.NUMEROPRESTAMO
	            pm.* 
	            FROM FBS_CARTERA.PRESTAMOMAESTRO pm 
	            INNER JOIN FBS_CLIENTES.CLIENTE c ON c.secuencial = pm.SECUENCIALCLIENTEPRINCIPAL
	            INNER JOIN FBS_PERSONAS.PERSONA p ON p.secuencial = c.secuencialpersona
	            WHERE pm.SECUENCIALOFICINA = 69479
	            AND pm.CODIGOESTADOPRESTAMO = 'A'
	            AND NOT EXISTS (
	                  SELECT 1
	                  FROM FBS_CARTERA.PRESTAMOCOMPONENTE_CARTERA pc
	                  WHERE pc.SECUENCIALPRESTAMO = pm.SECUENCIAL
	                    AND pc.DIASATRASO <> 0
	            )
	            AND NOT EXISTS (
		              SELECT 1 
		              FROM FBS_SEGURIDADES.USUARIO_COMPLEMENTO uc 
		              WHERE uc.SECUENCIALPERSONA = p.SECUENCIAL
	            )
	            AND PM.FECHAADJUDICACION BETWEEN TO_DATE('2026-01-01', 'YYYY-MM-DD') AND TO_DATE('2026-10-31', 'YYYY-MM-DD')
            ) sub  GROUP BY sub.SECUENCIALOFICINA
            UNION ALL
            SELECT 
              sub.SECUENCIALOFICINA SECUENCIALDIVISION,
              ('CUENCA CENTRO COCINA') AS NOMBRE,
              COUNT(DISTINCT sub.SECUENCIALCLIENTEPRINCIPAL) AS NUMERO_SOCIOS,
              2 AS TIPO
            FROM (
	            SELECT 
	            --p.identificacion, pm.NUMEROPRESTAMO
	            pm.* 
	            FROM FBS_CARTERA.PRESTAMOMAESTRO pm 
	            INNER JOIN FBS_CLIENTES.CLIENTE c ON c.secuencial = pm.SECUENCIALCLIENTEPRINCIPAL
	            INNER JOIN FBS_PERSONAS.PERSONA p ON p.secuencial = c.secuencialpersona
	            WHERE pm.SECUENCIALOFICINA = 69479
	            AND pm.CODIGOESTADOPRESTAMO = 'A'
	            AND pm.DEUDAINICIAL < 10000 
	            AND NOT EXISTS (
	                  SELECT 1
	                  FROM FBS_CARTERA.PRESTAMOCOMPONENTE_CARTERA pc
	                  WHERE pc.SECUENCIALPRESTAMO = pm.SECUENCIAL
	                    AND pc.DIASATRASO > 5
	            )
	            AND NOT EXISTS (
		              SELECT 1 
		              FROM FBS_SEGURIDADES.USUARIO_COMPLEMENTO uc 
		              WHERE uc.SECUENCIALPERSONA = p.SECUENCIAL
	            )
	            AND PM.FECHAADJUDICACION BETWEEN TO_DATE('2026-01-01', 'YYYY-MM-DD') AND TO_DATE('2026-10-31', 'YYYY-MM-DD')
            ) sub GROUP BY sub.SECUENCIALOFICINA
            UNION ALL 
            SELECT 
              sub.SECUENCIALOFICINA SECUENCIALDIVISION,
              ('CUENCA CENTRO REFRIGERADORA') AS NOMBRE,
              COUNT(DISTINCT sub.SECUENCIALCLIENTEPRINCIPAL) AS NUMERO_SOCIOS,
              3 AS TIPO
            FROM (
	            SELECT 
	            --p.identificacion, D.codigo
	            d.* 
	            FROM FBS_CAPTACIONESPLAZO.DEPOSITOMAESTRO d 
	            INNER JOIN FBS_CLIENTES.CLIENTE c ON c.secuencial = d.SECUENCIALCLIENTEPRINCIPAL
	            INNER JOIN FBS_PERSONAS.PERSONA p ON p.secuencial = c.secuencialpersona
	            WHERE d.SECUENCIALOFICINA = 69479
	            AND d.PLAZOENDIAS < 360
	            AND d.CODIGOESTADODEPOSITO = 'A'
	            AND NOT EXISTS (
			              SELECT 1 
			              FROM FBS_SEGURIDADES.USUARIO_COMPLEMENTO uc 
			              WHERE uc.SECUENCIALPERSONA = p.SECUENCIAL
		            )
	            AND d.FECHACREACION BETWEEN TO_DATE('2026-01-01', 'YYYY-MM-DD') AND TO_DATE('2026-10-31', 'YYYY-MM-DD')
            ) sub GROUP BY sub.SECUENCIALOFICINA
            UNION ALL
            SELECT 
              sub.SECUENCIALOFICINA SECUENCIALDIVISION,
              ('CUENCA CENTRO TABLET') AS NOMBRE,
              COUNT(DISTINCT sub.SECUENCIALCLIENTEPRINCIPAL) AS NUMERO_SOCIOS,
              4 AS TIPO
            FROM (
	            SELECT 
	            --p.identificacion, cm.codigo
	            cm.* 
	            FROM FBS_CAPTACIONESVISTA.CUENTAMAESTRO cm
	            INNER JOIN FBS_CLIENTES.CLIENTE c ON c.secuencial = cm.SECUENCIALCLIENTEPRINCIPAL
	            INNER JOIN FBS_PERSONAS.PERSONA p ON p.secuencial = c.secuencialpersona
	            WHERE cm.SECUENCIALOFICINA = 69479
	            AND cm.CODIGOTIPOCUENTA = 12
	            AND cm.CODIGOESTADO = 'A'
	            AND NOT EXISTS (
				              SELECT 1 
				              FROM FBS_SEGURIDADES.USUARIO_COMPLEMENTO uc 
				              WHERE uc.SECUENCIALPERSONA = p.SECUENCIAL
	            )
	            AND cm.FECHASISTEMACREACION BETWEEN TO_DATE('2026-01-01', 'YYYY-MM-DD') AND TO_DATE('2026-10-31', 'YYYY-MM-DD')
            ) sub GROUP BY sub.SECUENCIALOFICINA";
            cmd.Connection = con;
            con.Open();
            OracleDataReader dr = cmd.ExecuteReader();

            listadoSucursales.Add(new Sucursal(0, "Seleccione una agencia", 0, 0,0));
            while (dr.Read())
            {
                var suc = dr["SECUENCIALDIVISION"];
                var des = dr["NOMBRE"].ToString();
                var est = 0;
                var tipo = dr["TIPO"].ToString();
                var numsoc = dr["NUMERO_SOCIOS"].ToString();
                cmd.CommandText = $"SELECT count(*) TOTAL FROM FBS_RIFAS.TEMP_SORTEO_AGENCIAS WHERE SUCURSAL={suc} AND TIPO={tipo}";
                OracleDataReader qr = cmd.ExecuteReader();
                if (qr.Read())
                {
                    est = Convert.ToInt32(qr["TOTAL"]) > 0 ? 0 : 0;
                }
                listadoSucursales.Add(new Sucursal(Convert.ToInt32(suc), des, est, Convert.ToInt32(numsoc), Convert.ToInt32(tipo)));
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

        public Sucursal BuscarSucursal(long codigo, int tipo)
        {
            return listadoSucursales.Find(c => c.SUCURSAL == codigo && c.TIPO == tipo)!;
        }
        public List<Socio> BuscarSociosSucursales(long codigo, long tipo)
        {
            List<Socio> listadoSociosRegistrados = new List<Socio>();
            OracleConnection con = new OracleConnection(oradb_DESA);
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = $@"SELECT A.*, d.NOMBRE NOMBRESUCURSAL FROM FBS_RIFAS.TEMP_SORTEO_AGENCIAS a
                    INNER JOIN FBS_ORGANIZACIONES.OFICINA o ON o.SECUENCIALDIVISION = a.SUCURSAL
                    INNER JOIN FBS_GENERALES.DIVISION d ON d.SECUENCIAL = o.SECUENCIALDIVISION 
                    WHERE a.SUCURSAL = {codigo} AND a.tipo = {tipo}
                    ORDER BY a.SECUENCIAL";
            cmd.Connection = con;
            con.Open();
            OracleDataReader qr = cmd.ExecuteReader();
            while (qr.Read())
            {
                var sec = Convert.ToInt32(qr["SECUENCIAL"]);
                var numsoc = qr["NUMEROSOCIO"].ToString();
                var soc = qr["NOMBRESOCIO"].ToString();
                var nomcuenta = qr["NOMBRECUENTA"].ToString();
                var ced = qr["CEDULA"].ToString()!;
                var est = qr["ESTADO"].ToString()!;
                var nomsuc = qr["NOMBRESUCURSAL"].ToString()!;
                var suc = Convert.ToInt32(qr["SUCURSAL"]);
                var det = qr["DETALLE"].ToString()!;
                listadoSociosRegistrados.Add(new Socio(sec, numsoc, soc, nomcuenta, ced, est, nomsuc, suc, det));

            }
            return listadoSociosRegistrados;
        }

        public Sucursal ActualizarEstadosucursal(long codigo, int tipo)
        {
            Sucursal sucursal = listadoSucursales.Find(c => c.SUCURSAL == codigo && c.TIPO == tipo)!;
            if (sucursal != null)
            {
                sucursal.ESTADO = 1;
            }
            return sucursal!;
        }

        public IEnumerable<Socio> ListadoSociosSucursal(int sucursal, int tipo)
        {
            List<Socio> listadoSocios = new List<Socio>();
            List<Socio> listadoSociosSeleccionados = new List<Socio>();

            listadoSocios.Clear();
            OracleConnection con = new OracleConnection(oradb_DESA);
            OracleCommand cmd = new OracleCommand();

            if(tipo == 1)
            {
                /* TODOS LOS CREDITOS DESEMBOLSADOS DESDE EL 1 DE ENERO DEL 2026 AL 31 DE OCTUBRE DEL 2026, 
                   QUE SE HAYAN PAGADO TODAS LAS CUOTAS CON 0 DIAS DE RETRASO Y SE ENCUENTRE EL CRÉDITO ACTIVO */
                cmd.CommandText = $@"SELECT 
	                pm.SECUENCIALOFICINA SUCURSAL,
                    pm.SECUENCIALCLIENTEPRINCIPAL,
                    p.NOMBREUNIDO SOCIO,
                    p.IDENTIFICACION CEDULA,
                    c.NUMEROCLIENTE NUMSOCIO,
                    pm.NUMEROPRESTAMO DETALLE,
                    d.NOMBRE NOMBRESUCURSAL
	                FROM FBS_CARTERA.PRESTAMOMAESTRO pm 
	                INNER JOIN FBS_CLIENTES.CLIENTE c ON c.secuencial = pm.SECUENCIALCLIENTEPRINCIPAL
	                INNER JOIN FBS_PERSONAS.PERSONA p ON p.secuencial = c.secuencialpersona
	                INNER JOIN FBS_ORGANIZACIONES.OFICINA o ON o.SECUENCIALDIVISION = pm.SECUENCIALOFICINA
	                INNER JOIN FBS_GENERALES.DIVISION d ON d.SECUENCIAL = o.SECUENCIALDIVISION
	                WHERE pm.SECUENCIALOFICINA = {sucursal}
	                AND pm.CODIGOESTADOPRESTAMO = 'A'
	                AND NOT EXISTS (
	                      SELECT 1
	                      FROM FBS_CARTERA.PRESTAMOCOMPONENTE_CARTERA pc
	                      WHERE pc.SECUENCIALPRESTAMO = pm.SECUENCIAL
	                        AND pc.DIASATRASO <> 0
	                )
	                AND NOT EXISTS (
		                  SELECT 1 
		                  FROM FBS_SEGURIDADES.USUARIO_COMPLEMENTO uc 
		                  WHERE uc.SECUENCIALPERSONA = p.SECUENCIAL
	                )
	                AND PM.FECHAADJUDICACION BETWEEN TO_DATE('2026-01-01', 'YYYY-MM-DD') AND TO_DATE('2026-10-31', 'YYYY-MM-DD')";
            }

            if(tipo == 2)
            {
                /* TODOS LOS CRÉDITOS INFERIORES A $10.000 DOLARES DESEMBOLSADOS  DESDE EL 1 DE ENERO DEL 2026 AL 31 DE OCTUBRE DEL 2026,  
                   QUE SE HAYAN PAGADO TODAS LAS CUOTAS HASTA CON 5 DIAS DE RETRASO   Y SE ENCUENTREN ACTIVOS */
                cmd.CommandText = $@"SELECT 
	                pm.SECUENCIALOFICINA SUCURSAL,
                    pm.SECUENCIALCLIENTEPRINCIPAL,
                    p.NOMBREUNIDO SOCIO,
                    p.IDENTIFICACION CEDULA,
                    c.NUMEROCLIENTE NUMSOCIO,
                    pm.NUMEROPRESTAMO DETALLE,
                    d.NOMBRE NOMBRESUCURSAL
	                FROM FBS_CARTERA.PRESTAMOMAESTRO pm 
	                INNER JOIN FBS_CLIENTES.CLIENTE c ON c.secuencial = pm.SECUENCIALCLIENTEPRINCIPAL
	                INNER JOIN FBS_PERSONAS.PERSONA p ON p.secuencial = c.secuencialpersona
	                INNER JOIN FBS_ORGANIZACIONES.OFICINA o ON o.SECUENCIALDIVISION = pm.SECUENCIALOFICINA
	                INNER JOIN FBS_GENERALES.DIVISION d ON d.SECUENCIAL = o.SECUENCIALDIVISION
	                WHERE pm.SECUENCIALOFICINA = {sucursal}
	                AND pm.CODIGOESTADOPRESTAMO = 'A'
	                AND pm.DEUDAINICIAL < 10000 
	                AND NOT EXISTS (
	                      SELECT 1
	                      FROM FBS_CARTERA.PRESTAMOCOMPONENTE_CARTERA pc
	                      WHERE pc.SECUENCIALPRESTAMO = pm.SECUENCIAL
	                        AND pc.DIASATRASO > 5
	                )
	                AND NOT EXISTS (
		                  SELECT 1 
		                  FROM FBS_SEGURIDADES.USUARIO_COMPLEMENTO uc 
		                  WHERE uc.SECUENCIALPERSONA = p.SECUENCIAL
	                )
	                AND PM.FECHAADJUDICACION BETWEEN TO_DATE('2026-01-01', 'YYYY-MM-DD') AND TO_DATE('2026-10-31', 'YYYY-MM-DD')";
            }

            if (tipo == 3)
            {
                /* TODOS LOS DEPOSITOS A PLAZO FIJO QUE SE HAYAN APRETURADO DESDE EL 1 DE ENERO DEL 2026 AL 31 DE OCTUBRE DEL 2026, 
                   QUE SU PLAZO SEA MENOR A 360 DÍAS Y SE ENCUENTREN ACTIVOS*/
                cmd.CommandText = $@"SELECT 
	                d.SECUENCIALOFICINA SUCURSAL,
                    d.SECUENCIALCLIENTEPRINCIPAL,
                    p.NOMBREUNIDO SOCIO,
                    p.IDENTIFICACION CEDULA,
                    c.NUMEROCLIENTE NUMSOCIO,
                    d.CODIGO DETALLE,
                    di.NOMBRE NOMBRESUCURSAL
	                FROM FBS_CAPTACIONESPLAZO.DEPOSITOMAESTRO d 
	                INNER JOIN FBS_CLIENTES.CLIENTE c ON c.secuencial = d.SECUENCIALCLIENTEPRINCIPAL
	                INNER JOIN FBS_PERSONAS.PERSONA p ON p.secuencial = c.secuencialpersona
	                INNER JOIN FBS_ORGANIZACIONES.OFICINA o ON o.SECUENCIALDIVISION = d.SECUENCIALOFICINA
	                INNER JOIN FBS_GENERALES.DIVISION di ON di.SECUENCIAL = o.SECUENCIALDIVISION
	                WHERE d.SECUENCIALOFICINA = {sucursal}
	                AND d.PLAZOENDIAS < 360
	                AND d.CODIGOESTADODEPOSITO = 'A'
	                AND NOT EXISTS (
			                  SELECT 1 
			                  FROM FBS_SEGURIDADES.USUARIO_COMPLEMENTO uc 
			                  WHERE uc.SECUENCIALPERSONA = p.SECUENCIAL
		                )
	                AND d.FECHACREACION BETWEEN TO_DATE('2026-01-01', 'YYYY-MM-DD') AND TO_DATE('2026-10-31', 'YYYY-MM-DD')";
            }

            if (tipo == 4)
            {
                /* TODAS LAS CUENTAS INFANTILES QUE SE APERTUREN DESDE EL 1 DE ENERO DEL 2026 AL 31 DE OCTUBRE DEL 2026, 
                   SE ENCUENTREN EN ESTADO ACTIVO  */
                cmd.CommandText = $@"SELECT 
	                cm.SECUENCIALOFICINA SUCURSAL,
                    cm.SECUENCIALCLIENTEPRINCIPAL,
                    p.NOMBREUNIDO SOCIO,
                    p.IDENTIFICACION CEDULA,
                    c.NUMEROCLIENTE NUMSOCIO,
                     cmp.NOMBRECUENTA || ' - ' || cm.CODIGO DETALLE,
                    di.NOMBRE NOMBRESUCURSAL
	                FROM FBS_CAPTACIONESVISTA.CUENTAMAESTRO cm
                    INNER JOIN FBS_CAPTACIONESVISTA.CUENTAMAESTRO_PERSONALIZADO cmp ON cmp.SECUENCIALCUENTA = cm.SECUENCIAL 
	                INNER JOIN FBS_CLIENTES.CLIENTE c ON c.secuencial = cm.SECUENCIALCLIENTEPRINCIPAL
	                INNER JOIN FBS_PERSONAS.PERSONA p ON p.secuencial = c.secuencialpersona
	                INNER JOIN FBS_ORGANIZACIONES.OFICINA o ON o.SECUENCIALDIVISION = cm.SECUENCIALOFICINA
	                INNER JOIN FBS_GENERALES.DIVISION di ON di.SECUENCIAL = o.SECUENCIALDIVISION
	                WHERE cm.SECUENCIALOFICINA = {sucursal}
	                AND cm.CODIGOTIPOCUENTA = 12
	                AND cm.CODIGOESTADO = 'A'
	                AND NOT EXISTS (
				                  SELECT 1 
				                  FROM FBS_SEGURIDADES.USUARIO_COMPLEMENTO uc 
				                  WHERE uc.SECUENCIALPERSONA = p.SECUENCIAL
	                )
	                AND cm.FECHASISTEMACREACION BETWEEN TO_DATE('2026-01-01', 'YYYY-MM-DD') AND TO_DATE('2026-10-31', 'YYYY-MM-DD')";
            }

            cmd.Connection = con;
            con.Open();
            OracleDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                var numsoc = dr["NUMSOCIO"].ToString();
                var soc = dr["SOCIO"].ToString();
                var nomcuenta = dr["NOMBRESUCURSAL"].ToString();
                var ced = dr["CEDULA"].ToString();
                var suc = dr["SUCURSAL"];
                var det = dr["DETALLE"].ToString();
                listadoSocios.Add(new Socio(numsoc, soc, nomcuenta, ced, Convert.ToInt32(suc),det ));
            }
            con.Close();

            var listadoSociosAnterior = BuscarSociosSucursales(sucursal, tipo);

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
                cmd2.CommandText = $"INSERT INTO FBS_RIFAS.TEMP_SORTEO_AGENCIAS VALUES (0,'{socioAleatorio.NumeroSocio}', '{socioAleatorio.NombreSocio}', '{socioAleatorio.NombreCuenta}', '{socioAleatorio.Cedula}', '{socioAleatorio.FechaActualizacion}', 'Descartado', {sucursal}, {tipo}, '{socioAleatorio.Detalle}')";
                cmd2.Connection = con2;
                con2.Open();
                OracleDataReader dr2 = cmd2.ExecuteReader();
                con2.Close();
            }

            listadoSociosSeleccionados = BuscarSociosSucursales(sucursal, tipo);

            return listadoSociosSeleccionados;
        }

        public bool GuardarGanadorAgencia(long codigo, string numeroSocio, int tipo)
        {
            OracleConnection con = new OracleConnection(oradb_DESA);
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = $"UPDATE FBS_RIFAS.TEMP_SORTEO_AGENCIAS SET ESTADO='Ganador' WHERE SUCURSAL = {codigo} AND NUMEROSOCIO = '{numeroSocio}' AND TIPO = '{tipo}'";
            cmd.Connection = con;
            con.Open();
            OracleDataReader dr = cmd.ExecuteReader();
            con.Close();

            return true;
        }

    }
}
