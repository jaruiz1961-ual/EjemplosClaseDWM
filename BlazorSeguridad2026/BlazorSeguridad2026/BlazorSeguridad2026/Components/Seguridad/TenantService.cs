using BlazorSeguridad2026.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

public interface ITenantService
{
    Task SetCurrentTenantAsync(int tenantId);
}

public class TenantService : ITenantService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task SetCurrentTenantAsync(int tenantId)
    {
        var http = _httpContextAccessor.HttpContext!;
        var user = await _userManager.GetUserAsync(http.User);
        if (user is null) return;

        // 1. Actualizar claim de tenant
        var claims = await _userManager.GetClaimsAsync(user);
        var oldTenantClaim = claims.FirstOrDefault(c => c.Type == "TenantId");
        if (oldTenantClaim is not null)
            await _userManager.RemoveClaimAsync(user, oldTenantClaim);

        await _userManager.AddClaimAsync(user, new Claim("TenantId", tenantId.ToString()));

        // 2. Refrescar cookie de autenticación
        await _signInManager.RefreshSignInAsync(user);
    }
}
