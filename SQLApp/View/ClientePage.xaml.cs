using SQLApp.Model;
using SQLApp.ModelView;

namespace SQLApp.View;

public partial class ClientePage : ContentPage
{
    int _codigo = 0;

    public ClientePage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        await CarregarLista();
    }

    private async Task CarregarLista()
    {
        lvLista.ItemsSource = await CDatabase.Listar<CCliente>();
    }
    private async Task<CCliente?> LerFormulario()
    {
        if (string.IsNullOrWhiteSpace(txNome.Text) ||
            string.IsNullOrWhiteSpace(txRG.Text) ||
            string.IsNullOrWhiteSpace(txTelefone.Text))
        {
            await DisplayAlertAsync("Erro", "Preencha todos os campos.", "OK");
            return null;
        }

        if (!long.TryParse(txRG.Text, out long rg) || rg <= 0)
        {
            await DisplayAlertAsync("Erro", "O RG deve conter somente números.", "OK");
            return null;
        }

        return new CCliente
        {
            CodigoCliente = _codigo,
            Nome = txNome.Text.Trim(),
            RG = rg,
            Telefone = txTelefone.Text.Trim()
        };
    }

    private async void btIncluir_Clicked(object sender, EventArgs e)
    {
        CCliente? c = await LerFormulario();
        if (c == null)
            return;

        await CDatabase.Inserir(c);
        await DisplayAlertAsync("Sucesso", "Cliente cadastrado!", "OK");
        btLimpar_Clicked(sender, e);
        await CarregarLista();
    }

    private async void btAlterar_Clicked(object sender, EventArgs e)
    {
        if (_codigo == 0)
        {
            await DisplayAlertAsync("Erro", "Selecione um cliente na lista.", "OK");
            return;
        }

        CCliente? c = await LerFormulario();
        if (c == null)
            return;

        await CDatabase.Alterar(c);
        await DisplayAlertAsync("Sucesso", "Cliente alterado!", "OK");
        btLimpar_Clicked(sender, e);
        await CarregarLista();
    }

    private async void btExcluir_Clicked(object sender, EventArgs e)
    {
        if (_codigo == 0)
        {
            await DisplayAlertAsync("Erro", "Selecione um cliente na lista.", "OK");
            return;
        }

        if (await CDatabase.ContarComprasDoCliente(_codigo) > 0)
        {
            await DisplayAlertAsync("Erro", "Este cliente possui compras e não pode ser excluído.", "OK");
            return;
        }

        await CDatabase.Excluir<CCliente>(_codigo);
        await DisplayAlertAsync("Sucesso", "Cliente excluído!", "OK");
        btLimpar_Clicked(sender, e);
        await CarregarLista();
    }

    private void btLimpar_Clicked(object sender, EventArgs e)
    {
        _codigo = 0;
        txNome.Text = string.Empty;
        txRG.Text = string.Empty;
        txTelefone.Text = string.Empty;
    }

    private void lvLista_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is CCliente c)
        {
            _codigo = c.CodigoCliente;
            txNome.Text = c.Nome;
            txRG.Text = c.RG.ToString();
            txTelefone.Text = c.Telefone;
            lvLista.SelectedItem = null;
        }
    }
}
