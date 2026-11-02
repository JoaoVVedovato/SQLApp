using SQLite;
using SQLiteNetExtensions.Attributes;

namespace SQLApp.Model
{
    public class CItemCompra
    {
        [PrimaryKey, AutoIncrement]
        public int CodigoItem { get; set; }

        [ForeignKey(typeof(CProduto))]
        public int CodigoProduto { get; set; }

        [ForeignKey(typeof(CCompra))]
        public int CodigoCompra { get; set; }

        public int Quantidade { get; set; }

        public decimal ValorParcial { get; set; }
    }
}
