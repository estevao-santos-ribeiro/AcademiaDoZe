using AcademiaDoZe.Presentation.AppMaui.Message;
using CommunityToolkit.Mvvm.Messaging;

namespace AcademiaDoZe.Presentation.AppMaui
{
    public partial class App : Microsoft.Maui.Controls.Application
    {
        public App()
        {
            InitializeComponent();
            AplicarTema();

            WeakReferenceMessenger.Default.Register<TemaPreferencesUpdatedMessage>(this, (recipient, message) =>
            {
                AplicarTema();
            });
        }

        private void AplicarTema()
        {
            string tema = Preferences.Get("Tema", "system");
            UserAppTheme = tema switch
            {
                "light" => AppTheme.Light,
                "dark" => AppTheme.Dark,
                _ => AppTheme.Unspecified
            };
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}
