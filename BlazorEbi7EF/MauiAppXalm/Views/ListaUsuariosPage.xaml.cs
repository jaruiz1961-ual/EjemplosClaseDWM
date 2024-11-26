using BlazorEbi7.Data;
using BlazorEbi7.Model.Entidades;
using BlazorEbi7.Model.IServices;

namespace MauiAppXalm.Views
{
    public partial class ListaUsuariosPage : ContentPage
    {
        IUsuarioServiceAsync _listaService;

        public ListaUsuariosPage(IUsuarioServiceAsync service)
        {
            InitializeComponent();
            _listaService = service;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            collectionView.ItemsSource = await _listaService.FindAllAsync();
        }

        async void OnAddItemClicked(object sender, EventArgs e)
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { nameof(UsuarioSet), new UsuarioSet() }
            };
            await Shell.Current.GoToAsync(nameof(DetalleUsuarioPage), navigationParameter);
        }

        async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { nameof(UsuarioSet), e.CurrentSelection.FirstOrDefault() as UsuarioSet }
            };
            await Shell.Current.GoToAsync(nameof(DetalleUsuarioPage), navigationParameter);
        }
    }
}
