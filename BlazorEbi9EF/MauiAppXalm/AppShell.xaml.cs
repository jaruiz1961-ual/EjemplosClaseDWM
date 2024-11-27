using MauiAppXalm.Views;
namespace MauiAppXalm
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(DetalleUsuarioPage), typeof(DetalleUsuarioPage));
        }
    }
}
