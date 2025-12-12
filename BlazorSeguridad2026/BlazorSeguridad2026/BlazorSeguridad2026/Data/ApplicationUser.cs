using Microsoft.AspNetCore.Identity;
using Shares.Modelo;

namespace BlazorSeguridad2026.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser, ITenantEntity
    {
        public int? TenantId { get; set; }
        public string? DbKey { get; set; }
        public string? AppState { get; set; }
    }

}
