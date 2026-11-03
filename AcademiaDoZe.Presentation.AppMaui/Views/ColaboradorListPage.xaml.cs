using AcademiaDoZe.Presentation.AppMaui.ViewModels;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class ColaboradorListPage : ContentPage
{
    private CancellationTokenSource? _searchCts;

    public ColaboradorListPage(ColaboradorListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ColaboradorListViewModel viewModel)
        {
            await viewModel.LoadColaboradoresCommand.ExecuteAsync(null);
        }
    }

    // Debounce de 300ms na digitação para disparar busca automática
    private async void OnSearchDebounceTextChanged(object? sender, TextChangedEventArgs e)
    {
        try
        {
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            // espera curta (300ms)
            await Task.Delay(300, token);
            if (token.IsCancellationRequested) return;

            if (BindingContext is ColaboradorListViewModel vm)
            {
                await vm.SearchColaboradoresCommand.ExecuteAsync(null);
            }
        }
        catch (TaskCanceledException) { /* ignorar */ }
    }
}

