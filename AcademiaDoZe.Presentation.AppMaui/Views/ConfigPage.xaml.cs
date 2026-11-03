using AcademiaDoZe.Presentation.AppMaui.Message;
using CommunityToolkit.Mvvm.Messaging;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class ConfigPage : ContentPage
{
    private bool _isCarregando = true;

    public ConfigPage()
    {
        InitializeComponent();
        CarregarTema();
        CarregarBanco();
        TemaPicker.SelectedIndexChanged += OnTemaSelectedIndexChanged;
        _isCarregando = false;
    }

    private void CarregarTema()
    {
        string tema = Preferences.Get("Tema", "system");
        TemaPicker.SelectedItem = tema;
    }

    private void CarregarBanco()
    {
        TipoBancoPicker.SelectedIndex = Preferences.Get("BancoTipo", 0);
        ServidorEntry.Text = Preferences.Get("BancoServidor", "10.30.21.16");
        BancoEntry.Text = Preferences.Get("BancoNome", "db_academia_do_ze");
        UsuarioEntry.Text = Preferences.Get("BancoUsuario", "root");
        SenhaEntry.Text = Preferences.Get("BancoSenha", "abcBolinhas12345");
        ComplementoEntry.Text = Preferences.Get("BancoComplemento", string.Empty);
    }

    private void OnTemaSelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_isCarregando || TemaPicker.SelectedIndex < 0)
            return;

        string selectedTheme = TemaPicker.SelectedIndex switch
        {
            0 => "light",
            1 => "dark",
            _ => "system"
        };

        Preferences.Set("Tema", selectedTheme);

        // Disparar mensagem para uso na recarga dinâmica imediata
        WeakReferenceMessenger.Default.Send(new TemaPreferencesUpdatedMessage("TemaAlterado"));
    }

    private async void OnSalvarBancoClicked(object? sender, EventArgs e)
    {
        Preferences.Set("BancoTipo", TipoBancoPicker.SelectedIndex);
        Preferences.Set("BancoServidor", ServidorEntry.Text ?? string.Empty);
        Preferences.Set("BancoNome", BancoEntry.Text ?? string.Empty);
        Preferences.Set("BancoUsuario", UsuarioEntry.Text ?? string.Empty);
        Preferences.Set("BancoSenha", SenhaEntry.Text ?? string.Empty);
        Preferences.Set("BancoComplemento", ComplementoEntry.Text ?? string.Empty);

        await Shell.Current.DisplayAlertAsync("Sucesso", "Dados salvos com sucesso!", "OK");
    }

    private async void OnCancelarClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}

