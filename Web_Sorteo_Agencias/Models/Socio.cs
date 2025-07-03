namespace Web_Sorteo_Agencias.Models
{
    public class Socio
    {
        public string NumeroSocio { get; set; }
        public string NombreSocio { get; set; }
        public string NombreCuenta { get; set; }
        public string Cedula { get; set; }
        public string FechaActualizacion { get; set; }
        public string Estado { get; set; }
        public int Sucursal { get; set; }

        public Socio()
        {

        }
        public Socio(string numsoc, string soc, string ced, string fecact, string est, int suc)
        {
            this.NumeroSocio = numsoc;
            this.NombreSocio = soc;
            this.Cedula = ced;
            this.FechaActualizacion = fecact;
            this.Estado = est;
            this.Sucursal = suc;
        }
        public Socio(string numsoc, string soc, string nomcuenta, string ced, string est)
        {
            this.NumeroSocio = numsoc;
            this.NombreSocio = soc;
            this.NombreCuenta = nomcuenta;
            this.Cedula = ced;
            this.Estado = est;
        }
        public Socio(string numsoc, string soc, string nomcuenta, string ced, int suc)
        {
            this.NumeroSocio = numsoc;
            this.NombreSocio = soc;
            this.NombreCuenta = nomcuenta;
            this.Cedula = ced;
            this.Sucursal = suc;
        }
    }
}
