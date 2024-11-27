using BlazorEbi9.Data;
using BlazorEbi9.Model.Entidades;
using BlazorEbi9.Model.IServices;

namespace MauiAppXalm.Views
{
    [QueryProperty(nameof(UsuarioSet), "UsuarioSet")]
    public partial class DetalleUsuarioPage : ContentPage
    {
        IUsuarioServiceAsync _usuarioService;
        UsuarioSet _usuarioSet;
        bool _isNewItem;

        public UsuarioSet UsuarioSet
        {
            get => _usuarioSet;
            set
            {
                _isNewItem = IsNewItem(value);
                _usuarioSet = value;
                OnPropertyChanged();
            }
        }

        public DetalleUsuarioPage(IUsuarioServiceAsync service)
        {
            InitializeComponent();
            _usuarioService = service;
            BindingContext = this;
        }

        bool IsNewItem(UsuarioSet usuarioSet)
        {
            if (string.IsNullOrWhiteSpace(usuarioSet.UserName) && string.IsNullOrWhiteSpace(usuarioSet.Password))
                return true;
            return false;
        }

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            await _usuarioService.SaveUserAsync(UsuarioSet);
            await Shell.Current.GoToAsync("..");
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            await _usuarioService.DeleteIdAsync(UsuarioSet.Id);
            await Shell.Current.GoToAsync("..");
        }

        async void OnCancelButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
