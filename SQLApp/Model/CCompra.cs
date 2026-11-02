using SQLite;
using SQLiteNetExtensions.Attributes;

namespace SQLApp.Model
{
    public class CCompra
    {
        [PrimaryKey, AutoIncrement]
        public int CodigoCompra { get; set; }

        public DateTime DataCompra { get; set; }

        public decimal ValorTotal { get; set; }

        [ForeignKey(typeof(CCliente))]
        public int CodigoCliente { get; set; }

        [Ignore]
        public string Descricao => $"Compra {CodigoCompra} - {DataCompra:dd/MM/yyyy}";
    }
}
