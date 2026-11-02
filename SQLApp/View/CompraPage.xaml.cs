using SQLApp.Model;
using SQLApp.ModelView;

namespace SQLApp.View;

public partial class CompraPage : ContentPage
{
    int _codigo = 0;

    List<CCliente> _clientes = new List<CCliente>();
    decimal _valorTotal = 0;

    public CompraPage()
    {
        InitializeComponent();
        dpData.Date = DateTime.Today;
    }

    protected override async void OnAppearing()
    {
        _clientes = await CDatabase.Listar<CCliente>();
        pkCliente.ItemsSource = _clientes;
        await CarregarLista();
    }

    private async Task CarregarLista()
    {
        lvLista.ItemsSource = await CDatabase.Listar<CCompra>();
    }
    private async Task<CCompra?> LerFormulario()
    {
        if (pkCliente.SelectedItem is not CCliente cliente)
        {
            await DisplayAlertAsync("Erro", "Selecione o cliente.", "OK");
            return null;
        }

        DateTime data = dpData.Date is DateTime d ? d.Date : DateTime.Today;

        if (data > DateTime.Today)
        {
            await DisplayAlertAsync("Erro", "A data da compra não pode ser futura.", "OK");
            return null;
        }

        return new CCompra
        {
            CodigoCompra = _codigo,
            CodigoCliente = cliente.CodigoCliente,
            DataCompra = data,
            ValorTotal = _valorTotal
        };
    }

    private async void btIncluir_Clicked(object sender, EventArgs e)
    {
        CCompra? c = await LerFormulario();
        if (c == null)
            return;

        c.ValorTotal = 0;
        await CDatabase.Inserir(c);
        await DisplayAlertAsync("Sucesso",
            $"Compra {c.CodigoCompra} cadastrada! Adicione os itens na tela Itens da Compra.", "OK");
        btLimpar_Clicked(sender, e);
        await CarregarLista();
    }

    private async void btAlterar_Clicked(object sender, EventArgs e)
    {
        if (_codigo == 0)
        {
            await DisplayAlertAsync("Erro", "Selecione uma compra na lista.", "OK");
            return;
        }

        CCompra? c = await LerFormulario();
        if (c == null)
            return;

        await CDatabase.Alterar(c);
        await CDatabase.AtualizarValorTotal(c.CodigoCompra);
        await DisplayAlertAsync("Sucesso", "Compra alterada!", "OK");
        btLimpar_Clicked(sender, e);
        await CarregarLista();
    }

    private async void btExcluir_Clicked(object sender, EventArgs e)
    {
        if (_codigo == 0)
        {
            await DisplayAlertAsync("Erro", "Selecione uma compra na lista.", "OK");
            return;
        }

        await CDatabase.ExcluirItensDaCompra(_codigo);
        await CDatabase.Excluir<CCompra>(_codigo);
        await DisplayAlertAsync("Sucesso", "Compra e seus itens excluídos!", "OK");
        btLimpar_Clicked(sender, e);
        await CarregarLista();
    }

    private void btLimpar_Clicked(object sender, EventArgs e)
    {
        _codigo = 0;
        pkCliente.SelectedIndex = -1;
        dpData.Date = DateTime.Today;
        txValorTotal.Text = string.Empty;
        _valorTotal = 0;
    }

    private void lvLista_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is CCompra c)
        {
            _codigo = c.CodigoCompra;
            pkCliente.SelectedIndex = _clientes.FindIndex(x => x.CodigoCliente == c.CodigoCliente);
            dpData.Date = c.DataCompra;
            txValorTotal.Text = c.ValorTotal.ToString("C");
            _valorTotal = c.ValorTotal;
            lvLista.SelectedItem = null;
        }
    }
}
