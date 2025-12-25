using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Abstractions;


namespace BlazorSeguridad2026.Base.Modelo
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser<int>, ITenantEntity, IEntity
    {
        public int? TenantId { get; set; }
        public string? DbKey { get; set; }
        public string? AppState { get; set; }
    }


    public class ApplicationRole : IdentityRole<int>, ITenantEntity, IEntity
    {
        public int? TenantId { get; set; }
        public string? DbKey { get; set; }
    }


}

    
