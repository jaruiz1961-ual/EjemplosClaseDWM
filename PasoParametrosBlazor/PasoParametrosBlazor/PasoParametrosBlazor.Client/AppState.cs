namespace PasoParametrosBlazor.Client
{
    public class AppState
    {
        private string _valor;
        public string ValorCompartido
        {
            get => _valor;
            set
            {
                if (_valor != value)
                {
                    _valor = value;
                    OnChange?.Invoke();
                }
            }
        }

        // Evento para avisar a los componentes cuando cambia el valor
        public event Action? OnChange;
    }
}
