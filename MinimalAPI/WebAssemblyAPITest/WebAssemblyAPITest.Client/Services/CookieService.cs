

using Microsoft.JSInterop;

namespace WebAssemblyAPITest.Client.Services
{
    public class CookieService
    {
        private readonly IJSRuntime _jsRuntime;
        private const string TokenKey = "jwt_auth_token"; // Nombre de tu cookie

        public CookieService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        /// <summary>
        /// Obtiene el valor de la cookie JWT.
        /// </summary>
        public async ValueTask<string> GetTokenAsync()
        {
            // Llama a la función JavaScript para leer la cookie
            return await _jsRuntime.InvokeAsync<string>("getCookie", TokenKey);
        }

        // Aunque el servidor la establece, este método podría ser útil para el logout o manejo.
        /// <summary>
        /// Establece el valor de la cookie (opcional si el servidor ya la establece).
        /// </summary>
        public async ValueTask SetTokenAsync(string token, int days)
        {
            // Llama a la función JavaScript para escribir la cookie
            await _jsRuntime.InvokeVoidAsync("setCookie", TokenKey, token, days);
        }

        /// <summary>
        /// Elimina la cookie (usado para el logout).
        /// </summary>
        public async ValueTask DeleteTokenAsync()
        {
            // Llama a la función JavaScript para borrar la cookie
            await _jsRuntime.InvokeVoidAsync("deleteCookie", TokenKey);
        }
    }
}

