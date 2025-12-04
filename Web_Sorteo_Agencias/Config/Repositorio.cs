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
                        (sub.NOMBRE || ' TRANSACCIÓN CANALES') AS NOMBRE,
                        COUNT(*) AS NUMERO_SOCIOS,
                        1 AS TIPO
                    FROM (
                        SELECT 
                        c.SECUENCIALOFICINA,
                        c.SECUENCIALCLIENTEPRINCIPAL,
                        p.NOMBREUNIDO , p.IDENTIFICACION,
                        c2.NUMEROCLIENTE,
                        COUNT(*) AS TOTAL_MOVIMIENTOS,
                        d.NOMBRE
		                    FROM FBS_CAPTACIONESVISTA.MOVIMIENTOCUENTACOMP_VISTA mv
		                    INNER JOIN FBS_CAPTACIONESVISTA.CUENTAMAESTRO c ON c.SECUENCIAL = mv.SECUENCIALCUENTA
		                    INNER JOIN FBS_ORGANIZACIONES.OFICINA o ON o.SECUENCIALDIVISION = c.SECUENCIALOFICINA
		                    INNER JOIN FBS_GENERALES.DIVISION d ON d.SECUENCIAL = o.SECUENCIALDIVISION
		                    INNER JOIN fbs_clientes.CLIENTE c2 ON c2.SECUENCIAL = c.SECUENCIALCLIENTEPRINCIPAL
		                    INNER JOIN FBS_PERSONAS.PERSONA p ON p.SECUENCIAL = c2.SECUENCIALPERSONA
		                    WHERE 
		                        mv.CODIGOUSUARIO = 'canal.digital'
		                        AND mv.VALOR NOT IN (0.36,0.31,0.05)
		                        AND o.SECUENCIALDIVISION NOT IN (69483,69484,69482,69481,69479,69475)
		                        AND mv.FECHAMOVIMIENTO BETWEEN TO_DATE('2025-11-01', 'YYYY-MM-DD')
		                                                   AND TO_DATE('2025-11-30', 'YYYY-MM-DD')
		                        AND NOT EXISTS (
		                            SELECT 1 
		                            FROM FBS_SEGURIDADES.USUARIO_COMPLEMENTO uc 
		                            WHERE uc.SECUENCIALPERSONA = p.SECUENCIAL
		                        )
		                    GROUP BY 
		                        c.SECUENCIALOFICINA, 
		                        c.SECUENCIALCLIENTEPRINCIPAL,
		                        p.NOMBREUNIDO , p.IDENTIFICACION,
		                        c2.NUMEROCLIENTE, 
		                        d.NOMBRE
		                    HAVING 
		                        COUNT(*) > 10
                    ) sub
                    GROUP BY sub.SECUENCIALOFICINA, sub.NOMBRE
                    UNION ALL
                    SELECT 
                         D.SECUENCIAL SECUENCIALDIVISION,
                         (d.NOMBRE || ' ACTIVACIÓN CANALES') AS NOMBRE,
                         COUNT(DISTINCT C2.SECUENCIAL) AS NUMERO_SOCIOS,
                         2 AS TIPO
	                    FROM FBS_CLIENTES.CLIENTEREGISTROCANALESDIGITALES C
	                    INNER JOIN FBS_CLIENTES.CLIENTE C2 ON C2.SECUENCIAL = C.SECUENCIALCLIENTE
	                    INNER JOIN FBS_PERSONAS.PERSONA P ON P.SECUENCIAL = C2.SECUENCIALPERSONA 
	                    LEFT JOIN FBS_INTERNETHUB.USUARIO IU ON IU.IDENTIFICACION = P.IDENTIFICACION 
	                    INNER JOIN FBS_CAPTACIONESVISTA.CUENTAMAESTRO C3 ON C3.SECUENCIALCLIENTEPRINCIPAL = C2.SECUENCIAL 
	                    INNER JOIN FBS_SEGURIDADES.USUARIO U ON U.CODIGO = C.CODIGOUSUARIO 
	                    INNER JOIN FBS_GENERALES.DIVISION D ON D.SECUENCIAL = U.SECUENCIALOFICINA 
	                    WHERE 
	                        (C.ESPARAMOVIL <> 0 OR C.ESPARAWEB <> 0)
	                        AND D.SECUENCIAL NOT IN (69483,69484)
	                        AND C.FECHASISTEMA BETWEEN TO_DATE('2025-11-01', 'YYYY-MM-DD') 
	                                                 AND TO_DATE('2025-11-30', 'YYYY-MM-DD')
                        AND NOT EXISTS (
                            SELECT 1 
                            FROM FBS_SEGURIDADES.USUARIO_COMPLEMENTO UC
                            WHERE UC.SECUENCIALPERSONA = P.SECUENCIAL
                        )
                    GROUP BY 
                        D.NOMBRE, D.SECUENCIAL
                    ORDER BY SECUENCIALDIVISION, TIPO";
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
                cmd.CommandText = $@"SELECT 
                                    c.SECUENCIALOFICINA SUCURSAL,
                                    c.SECUENCIALCLIENTEPRINCIPAL,
                                    p.NOMBREUNIDO SOCIO,
                                    p.IDENTIFICACION CEDULA,
                                    c2.NUMEROCLIENTE NUMSOCIO,
                                    COUNT(*) AS DETALLE,
                                    d.NOMBRE NOMBRESUCURSAL
	                        FROM FBS_CAPTACIONESVISTA.MOVIMIENTOCUENTACOMP_VISTA mv
	                        INNER JOIN FBS_CAPTACIONESVISTA.CUENTAMAESTRO c ON c.SECUENCIAL = mv.SECUENCIALCUENTA
	                        INNER JOIN FBS_ORGANIZACIONES.OFICINA o ON o.SECUENCIALDIVISION = c.SECUENCIALOFICINA
	                        INNER JOIN FBS_GENERALES.DIVISION d ON d.SECUENCIAL = o.SECUENCIALDIVISION
	                        INNER JOIN fbs_clientes.CLIENTE c2 ON c2.SECUENCIAL = c.SECUENCIALCLIENTEPRINCIPAL
	                        INNER JOIN FBS_PERSONAS.PERSONA p ON p.SECUENCIAL = c2.SECUENCIALPERSONA
	                        WHERE 
	                            mv.CODIGOUSUARIO = 'canal.digital'
	                            AND mv.VALOR NOT IN (0.36,0.31,0.05)
	                            AND d.SECUENCIAL NOT IN (69483,69484)
    	                        AND d.SECUENCIAL = {sucursal}
	                            AND mv.FECHAMOVIMIENTO BETWEEN TO_DATE('2025-11-01', 'YYYY-MM-DD')
	                                                       AND TO_DATE('2025-11-30', 'YYYY-MM-DD')
	                            AND NOT EXISTS (
	                                SELECT 1 
	                                FROM FBS_SEGURIDADES.USUARIO_COMPLEMENTO uc 
	                                WHERE uc.SECUENCIALPERSONA = p.SECUENCIAL
	                            )
	                        GROUP BY 
	                            c.SECUENCIALOFICINA, 
	                            c.SECUENCIALCLIENTEPRINCIPAL,
	                            p.NOMBREUNIDO , p.IDENTIFICACION,
	                            c2.NUMEROCLIENTE, 
	                            d.NOMBRE
	                        HAVING 
	                            COUNT(*) > 10";
            }

            if(tipo == 2)
            {
                cmd.CommandText = $@"SELECT 
                                 DISTINCT C2.SECUENCIAL,
                                 p.NOMBREUNIDO SOCIO,
                                 p.IDENTIFICACION CEDULA,
                                 c2.NUMEROCLIENTE NUMSOCIO,
                                 d.SECUENCIAL SUCURSAL,
                                 D.NOMBRE NOMBRESUCURSAL,
                                 C.FECHAMAQUINA DETALLE
                            FROM FBS_CLIENTES.CLIENTEREGISTROCANALESDIGITALES C
                            INNER JOIN FBS_CLIENTES.CLIENTE C2 ON C2.SECUENCIAL = C.SECUENCIALCLIENTE
                            INNER JOIN FBS_PERSONAS.PERSONA P ON P.SECUENCIAL = C2.SECUENCIALPERSONA 
                            LEFT JOIN FBS_INTERNETHUB.USUARIO IU ON IU.IDENTIFICACION = P.IDENTIFICACION 
                            INNER JOIN FBS_CAPTACIONESVISTA.CUENTAMAESTRO C3 ON C3.SECUENCIALCLIENTEPRINCIPAL = C2.SECUENCIAL 
                            INNER JOIN FBS_SEGURIDADES.USUARIO U ON U.CODIGO = C.CODIGOUSUARIO 
                            INNER JOIN FBS_GENERALES.DIVISION D ON D.SECUENCIAL = U.SECUENCIALOFICINA 
                            WHERE 
                                (C.ESPARAMOVIL <> 0 OR C.ESPARAWEB <> 0)
                                AND d.SECUENCIAL NOT IN (69483,69484)
                                AND d.SECUENCIAL = {sucursal}
                                AND C.FECHASISTEMA BETWEEN TO_DATE('2025-11-01', 'YYYY-MM-DD') 
                                                         AND TO_DATE('2025-11-30', 'YYYY-MM-DD')
                                AND NOT EXISTS (
                                    SELECT 1 
                                    FROM FBS_SEGURIDADES.USUARIO_COMPLEMENTO UC
                                    WHERE UC.SECUENCIALPERSONA = P.SECUENCIAL
                                )
                            ORDER BY 
                                c2.SECUENCIAL";
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
