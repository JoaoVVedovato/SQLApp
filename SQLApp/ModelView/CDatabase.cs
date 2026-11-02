using SQLite;
using SQLApp.Model;

namespace SQLApp.ModelView
{
    // Acesso ao banco de dados SQLite, usado por todas as páginas
    public static class CDatabase
    {
        private static SQLiteAsyncConnection? _db;

        // Abre o banco e cria as 4 tabelas (só na primeira vez)
        private static async Task<SQLiteAsyncConnection> Conexao()
        {
            if (_db != null)
                return _db;

            string caminho = Path.Combine(FileSystem.AppDataDirectory, "loja.db3");
            _db = new SQLiteAsyncConnection(caminho);

            await _db.CreateTableAsync<CCliente>();
            await _db.CreateTableAsync<CProduto>();
            await _db.CreateTableAsync<CCompra>();
            await _db.CreateTableAsync<CItemCompra>();

            return _db;
        }

        // CRUD genérico: serve para as 4 tabelas
        public static async Task<int> Inserir<T>(T obj) => await (await Conexao()).InsertAsync(obj);

        public static async Task<int> Alterar<T>(T obj) => await (await Conexao()).UpdateAsync(obj);

        public static async Task<int> Excluir<T>(int codigo) => await (await Conexao()).DeleteAsync<T>(codigo);

        public static async Task<List<T>> Listar<T>() where T : new()
            => await (await Conexao()).Table<T>().ToListAsync();

        public static async Task<T?> Buscar<T>(int codigo) where T : new()
            => await (await Conexao()).FindAsync<T>(codigo);

        // ---- Consultas usadas para manter o relacionamento entre as tabelas ----

        public static async Task<int> ContarComprasDoCliente(int codigoCliente)
            => await (await Conexao()).Table<CCompra>()
                .Where(c => c.CodigoCliente == codigoCliente).CountAsync();

        public static async Task<int> ContarItensDoProduto(int codigoProduto)
            => await (await Conexao()).Table<CItemCompra>()
                .Where(i => i.CodigoProduto == codigoProduto).CountAsync();

        public static async Task ExcluirItensDaCompra(int codigoCompra)
            => await (await Conexao()).ExecuteAsync(
                "DELETE FROM CItemCompra WHERE CodigoCompra = ?", codigoCompra);
        public static async Task AtualizarValorTotal(int codigoCompra)
        {
            var db = await Conexao();

            CCompra? compra = await db.FindAsync<CCompra>(codigoCompra);
            if (compra == null)
                return;

            List<CItemCompra> itens = await db.Table<CItemCompra>()
                .Where(i => i.CodigoCompra == codigoCompra).ToListAsync();

            compra.ValorTotal = itens.Sum(i => i.ValorParcial);
            await db.UpdateAsync(compra);
        }
    }
}
