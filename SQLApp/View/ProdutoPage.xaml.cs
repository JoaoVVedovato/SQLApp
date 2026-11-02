using SQLApp.Model;
using SQLApp.ModelView;
using System.Globalization;

namespace SQLApp.View;

public partial class ProdutoPage : ContentPage
{
    int _codigo = 0;

    public ProdutoPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        await CarregarLista();
    }

    private async Task CarregarLista()
    {
        lvLista.ItemsSource = await CDatabase.Listar<CProduto>();
    }

    private async Task<CProduto?> LerFormulario()
    {
        if (string.IsNullOrWhiteSpace(txNome.Text) ||
            string.IsNullOrWhiteSpace(txTipo.Text) ||
            string.IsNullOrWhiteSpace(txPreco.Text) ||
            string.IsNullOrWhiteSpace(txEstoque.Text))
        {
            await DisplayAlertAsync("Erro", "Preencha todos os campos.", "OK");
            return null;
        }

        if (!decimal.TryParse(txPreco.Text.Replace(',', '.'), NumberStyles.Number,
                CultureInfo.InvariantCulture, out decimal preco) || preco <= 0)
        {
            await DisplayAlertAsync("Erro", "Informe um preço válido maior que zero.", "OK");
            return null;
        }

        if (!int.TryParse(txEstoque.Text, out int estoque) || estoque < 0)
        {
            await DisplayAlertAsync("Erro", "Informe uma quantidade em estoque válida.", "OK");
            return null;
        }

        return new CProduto
        {
            CodigoProduto = _codigo,
            NomeProduto = txNome.Text.Trim(),
            Tipo = txTipo.Text.Trim(),
            Preco = preco,
            QtdEstoque = estoque
        };
    }

    private async void btIncluir_Clicked(object sender, EventArgs e)
    {
        CProduto? p = await LerFormulario();
        if (p == null)
            return;

        await CDatabase.Inserir(p);
        await DisplayAlertAsync("Sucesso", "Produto cadastrado!", "OK");
        btLimpar_Clicked(sender, e);
        await CarregarLista();
    }

    private async void btAlterar_Clicked(object sender, EventArgs e)
    {
        if (_codigo == 0)
        {
            await DisplayAlertAsync("Erro", "Selecione um produto na lista.", "OK");
            return;
        }

        CProduto? p = await LerFormulario();
        if (p == null)
            return;

        await CDatabase.Alterar(p);
        await DisplayAlertAsync("Sucesso", "Produto alterado!", "OK");
        btLimpar_Clicked(sender, e);
        await CarregarLista();
    }

    private async void btExcluir_Clicked(object sender, EventArgs e)
    {
        if (_codigo == 0)
        {
            await DisplayAlertAsync("Erro", "Selecione um produto na lista.", "OK");
            return;
        }

        if (await CDatabase.ContarItensDoProduto(_codigo) > 0)
        {
            await DisplayAlertAsync("Erro", "Este produto está em uma compra e não pode ser excluído.", "OK");
            return;
        }

        await CDatabase.Excluir<CProduto>(_codigo);
        await DisplayAlertAsync("Sucesso", "Produto excluído!", "OK");
        btLimpar_Clicked(sender, e);
        await CarregarLista();
    }

    private void btLimpar_Clicked(object sender, EventArgs e)
    {
        _codigo = 0;
        txNome.Text = string.Empty;
        txTipo.Text = string.Empty;
        txPreco.Text = string.Empty;
        txEstoque.Text = string.Empty;
    }

    private void lvLista_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is CProduto p)
        {
            _codigo = p.CodigoProduto;
            txNome.Text = p.NomeProduto;
            txTipo.Text = p.Tipo;
            txPreco.Text = p.Preco.ToString("0.00");
            txEstoque.Text = p.QtdEstoque.ToString();
            lvLista.SelectedItem = null;
        }
    }
}
