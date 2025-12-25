using BlazorSeguridad2026.Base.Seguridad;
using BlazorSeguridad2026.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;


public static class TenantInterop
{
    //hay que regisgtrar ServiceLocator en blazor
    public static class ServiceLocator
    {
        public static IServiceProvider? Services { get; set; }
    }

    [JSInvokable("SetTenantId")]
    public static async Task SetTenantId(int tenantId)
    {
        var root = ServiceLocator.Services
                   ?? throw new InvalidOperationException("Service provider not initialized.");

        // crear un scope
        using var scope = root.GetRequiredService<IServiceScopeFactory>().CreateScope();

        var contextProvider = scope.ServiceProvider.GetRequiredService<ContextProvider>();

        contextProvider._AppState.TenantId = tenantId;
        // si ContextProvider escribe algo en DB, aquí puedes await-ar métodos async
        await contextProvider.SaveAllContextAsync(contextProvider);

        await Task.CompletedTask;
    }
}