using AcademiaDoZe.Presentation.AppMaui.Message;
using CommunityToolkit.Mvvm.Messaging;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class ConfigPage : ContentPage
{
    public ConfigPage()
    {
        InitializeComponent();
        CarregarTema();
        CarregarBanco();
    }

    private void CarregarTema()
    {
        string tema = Preferences.Get("Tema", "system");
        AtualizarBotoesTema(tema);
    }

    private void OnTemaClaroClicked(object? sender, EventArgs e) => SelecionarTema("light");

    private void OnTemaEscuroClicked(object? sender, EventArgs e) => SelecionarTema("dark");

    private void OnTemaSistemaClicked(object? sender, EventArgs e) => SelecionarTema("system");

    private void SelecionarTema(string tema)
    {
        Preferences.Set("Tema", tema);
        AtualizarBotoesTema(tema);
        WeakReferenceMessenger.Default.Send(new TemaPreferencesUpdatedMessage("TemaAlterado"));
    }

    private void AtualizarBotoesTema(string tema)
    {
        var corAtiva = Color.FromArgb("#EA580C");
        var corInativaTexto = Color.FromArgb("#64748B");
        var corInativaBorda = Color.FromArgb("#CBD5E1");

        ConfigurarBotaoTema(BtnTemaClaro, IconTemaClaro, LabelTemaClaro, tema == "light", corAtiva, corInativaTexto, corInativaBorda);
        ConfigurarBotaoTema(BtnTemaEscuro, IconTemaEscuro, LabelTemaEscuro, tema == "dark", corAtiva, corInativaTexto, corInativaBorda);
        ConfigurarBotaoTema(BtnTemaSistema, IconTemaSistema, LabelTemaSistema, tema == "system", corAtiva, corInativaTexto, corInativaBorda);
    }

    private static void ConfigurarBotaoTema(Border btn, Label icon, Label label, bool isAtivo, Color corAtiva, Color corInativaTexto, Color corInativaBorda)
    {
        if (isAtivo)
        {
            btn.BackgroundColor = corAtiva;
            btn.Stroke = corAtiva;
            icon.TextColor = Colors.White;
            label.TextColor = Colors.White;
        }
        else
        {
            btn.BackgroundColor = Colors.Transparent;
            btn.Stroke = corInativaBorda;
            icon.TextColor = corInativaTexto;
            label.TextColor = corInativaTexto;
        }
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
