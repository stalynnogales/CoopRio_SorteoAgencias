namespace Web_Sorteo_Agencias.Models
{
    public class Sucursal
    {
        public int? SUCURSAL { get; set; }
        public string? DESCRIPCION { get; set; }
        public int ESTADO { get; set; }
        public int NUMERO { get; set; }

        public Sucursal()
        {

        }
        public Sucursal(int sucursal, string descripcion, int estado, int numerosoc)
        {
            this.SUCURSAL = sucursal;
            this.DESCRIPCION = descripcion;
            this.ESTADO = estado;
            this.NUMERO = numerosoc;
        }
    }
}
