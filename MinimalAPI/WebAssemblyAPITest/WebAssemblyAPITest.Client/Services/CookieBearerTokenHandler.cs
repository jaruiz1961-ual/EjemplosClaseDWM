 using System.Net.Http.Headers;
 using WebAssemblyAPITest.Client;

namespace WebAssemblyAPITest.Client.Services
{
    // Asegúrate de que este Handler esté registrado como 'Transient' en Program.cs
    public class CookieBearerTokenHandler : DelegatingHandler
    {
        private readonly CookieService _cookieService;

        public CookieBearerTokenHandler(CookieService cookieService)
        {
            _cookieService = cookieService; // El servicio que lee la cookie (vía JS Interop)
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // 1. Obtener el token de la cookie
            // Llama al servicio que utiliza JS Interop para leer 'document.cookie'
            var token = await _cookieService.GetTokenAsync();

            if (!string.IsNullOrEmpty(token))
            {
                // 2. Adjuntar el token al encabezado de la solicitud
                // Esto crea el encabezado: Authorization: Bearer [EL_TOKEN_JWT]
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // 3. Devolver el control: permite que la solicitud se envíe a la API
            return await base.SendAsync(request, cancellationToken);
        }
    }
}

