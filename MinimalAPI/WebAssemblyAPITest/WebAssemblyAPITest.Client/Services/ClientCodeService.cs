namespace WebAssemblyAPITest.Client.Services
{
    // 🎯 Proyecto Client/Services/ClientCodeService.cs

    public class ClientCodeService
    {
        private readonly CookieService _cookieService;
        private string? _currentCode;

        // 📢 Evento para notificar a los componentes suscritos
        public event Action? OnChange;

        public ClientCodeService(CookieService cookieService)
        {
            _cookieService = cookieService;
        }

        // 1. Método para obtener el código
        public async ValueTask<string?> GetCodeAsync()
        {
            // Si ya está en memoria, lo devolvemos inmediatamente
            if (_currentCode != null)
            {
                return _currentCode;
            }

            // Si no está, lo cargamos desde la persistencia (Cookie)
            _currentCode = await _cookieService.GetTokenAsync(); // Asumiendo que usa el mismo storage
            return _currentCode;
        }

        // 2. Método para establecer y persistir el código
        public async Task SetCodeAsync(string newCode)
        {
            _currentCode = newCode;

            // Persistir en la Cookie para que el servidor lo pueda ver al iniciar la página
            await _cookieService.SetTokenAsync(newCode, 1); // 1 día de expiración

            // Notificar a todos los componentes que escuchan
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
