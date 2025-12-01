using DataBase.Genericos;
using System.Net.Http.Headers;


namespace DataBase
{
    // Asegúrate de que este Handler esté registrado como 'Transient' en Program.cs
    public class CookieBearerTokenHandler : DelegatingHandler
    {
        private readonly IContextProvider _cookieService;

        public CookieBearerTokenHandler(IContextProvider cookieService)
        {
            _cookieService = cookieService; // El servicio que lee la cookie (vía JS Interop)
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // 1. Obtener el token de la cookie
            // Llama al servicio que utiliza JS Interop para leer 'document.cookie'
            var token = _cookieService.Token;

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

