namespace Web_Sorteo_Agencias.Models
{
    public class Sucursal
    {
        public int? SUCURSAL { get; set; }
        public string? DESCRIPCION { get; set; }
        public int ESTADO { get; set; }
        public int NUMERO { get; set; }
        public int TIPO { get; set; }

        public Sucursal()
        {

        }
        public Sucursal(int sucursal, string descripcion, int estado, int numsoc, int tipo)
        {
            this.SUCURSAL = sucursal;
            this.DESCRIPCION = descripcion;
            this.ESTADO = estado;
            this.NUMERO = numsoc;
            this.TIPO = tipo;
        }
    }
}
