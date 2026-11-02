using SQLApp.Model;
using SQLApp.ModelView;

namespace SQLApp.View;

public partial class ItemCompraPage : ContentPage
{
    int _codigo = 0;

    List<CCompra> _compras = new List<CCompra>();
    List<CProduto> _produtos = new List<CProduto>();

    int _compraAnterior = 0;

    public ItemCompraPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        _compras = await CDatabase.Listar<CCompra>();
        _produtos = await CDatabase.Listar<CProduto>();
        pkCompra.ItemsSource = _compras;
        pkProduto.ItemsSource = _produtos;
        await CarregarLista();
    }

    private async Task CarregarLista()
    {
        lvLista.ItemsSource = await CDatabase.Listar<CItemCompra>();
    }

    private async Task<CItemCompra?> LerFormulario()
    {
        if (pkCompra.SelectedItem is not CCompra compra)
        {
            await DisplayAlertAsync("Erro", "Selecione a compra.", "OK");
            return null;
        }

        if (pkProduto.SelectedItem is not CProduto produto)
        {
            await DisplayAlertAsync("Erro", "Selecione o produto.", "OK");
            return null;
        }

        if (!int.TryParse(txQuantidade.Text, out int quantidade) || quantidade <= 0)
        {
            await DisplayAlertAsync("Erro", "Informe uma quantidade maior que zero.", "OK");
            return null;
        }

        return new CItemCompra
        {
            CodigoItem = _codigo,
            CodigoCompra = compra.CodigoCompra,
            CodigoProduto = produto.CodigoProduto,
            Quantidade = quantidade,
            ValorParcial = produto.Preco * quantidade
        };
    }

    private async void btIncluir_Clicked(object sender, EventArgs e)
    {
        CItemCompra? item = await LerFormulario();
        if (item == null)
            return;

        await CDatabase.Inserir(item);
        await CDatabase.AtualizarValorTotal(item.CodigoCompra);
        await DisplayAlertAsync("Sucesso", "Item cadastrado!", "OK");
        btLimpar_Clicked(sender, e);
        await CarregarLista();
    }

    private async void btAlterar_Clicked(object sender, EventArgs e)
    {
        if (_codigo == 0)
        {
            await DisplayAlertAsync("Erro", "Selecione um item na lista.", "OK");
            return;
        }

        CItemCompra? item = await LerFormulario();
        if (item == null)
            return;

        await CDatabase.Alterar(item);

        await CDatabase.AtualizarValorTotal(item.CodigoCompra);
        if (_compraAnterior != item.CodigoCompra)
            await CDatabase.AtualizarValorTotal(_compraAnterior);

        await DisplayAlertAsync("Sucesso", "Item alterado!", "OK");
        btLimpar_Clicked(sender, e);
        await CarregarLista();
    }

    private async void btExcluir_Clicked(object sender, EventArgs e)
    {
        if (_codigo == 0)
        {
            await DisplayAlertAsync("Erro", "Selecione um item na lista.", "OK");
            return;
        }

        await CDatabase.Excluir<CItemCompra>(_codigo);
        await CDatabase.AtualizarValorTotal(_compraAnterior);
        await DisplayAlertAsync("Sucesso", "Item excluído!", "OK");
        btLimpar_Clicked(sender, e);
        await CarregarLista();
    }

    private void btLimpar_Clicked(object sender, EventArgs e)
    {
        _codigo = 0;
        pkCompra.SelectedIndex = -1;
        pkProduto.SelectedIndex = -1;
        txQuantidade.Text = string.Empty;
        _compraAnterior = 0;
    }

    private void lvLista_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is CItemCompra item)
        {
            _codigo = item.CodigoItem;
            pkCompra.SelectedIndex = _compras.FindIndex(x => x.CodigoCompra == item.CodigoCompra);
            pkProduto.SelectedIndex = _produtos.FindIndex(x => x.CodigoProduto == item.CodigoProduto);
            txQuantidade.Text = item.Quantidade.ToString();
            _compraAnterior = item.CodigoCompra;
            lvLista.SelectedItem = null;
        }
    }
}
