namespace SQLApp.View;

public partial class MenuPage : ContentPage
{
    public MenuPage()
    {
        InitializeComponent();
    }

    private async void btClientes_Clicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//ClientePage");

    private async void btProdutos_Clicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//ProdutoPage");

    private async void btCompras_Clicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//CompraPage");

    private async void btItens_Clicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//ItemCompraPage");
}
