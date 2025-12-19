using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Abstractions;
using BlazorSeguridad2026.Data;

namespace BlazorSeguridad2026.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser<int>, ITenantEntity, IEntity
    {
        public int? TenantId { get; set; }
        public string? DbKey { get; set; }
        public string? AppState { get; set; }

        public static List<ApplicationUser> Convert(List<Users> source)
    => source.Select(MapToApplicationUser).ToList();

        private static ApplicationUser MapToApplicationUser(Users u) =>
    new ApplicationUser
    {
        Id = u.Id,
        UserName = u.UserName,
        NormalizedUserName = u.NormalizedUserName,
        Email = u.Email,
        NormalizedEmail = u.NormalizedEmail,
        EmailConfirmed = u.EmailConfirmed,
        PasswordHash = u.PasswordHash,
        SecurityStamp = u.SecurityStamp,
        ConcurrencyStamp = u.ConcurrencyStamp,
        PhoneNumber = u.PhoneNumber,
        PhoneNumberConfirmed = u.PhoneNumberConfirmed,
        TwoFactorEnabled = u.TwoFactorEnabled,
        LockoutEnd = u.LockoutEnd,
        LockoutEnabled = u.LockoutEnabled,
        AccessFailedCount = u.AccessFailedCount,
        TenantId = u.TenantId,
        DbKey = u.DbKey,
        AppState = u.AppState
    };
    }

    public class ApplicationRole : IdentityRole<int>, ITenantEntity, IEntity
    {
        public int? TenantId { get; set; }
        public string? DbKey { get; set; }
    }

    public class Users : IdentityUser<int>, ITenantEntity, IEntity
    {
        public int? TenantId { get; set; }
        public string? DbKey { get; set; }
        public string? AppState { get; set; }
    }

    public class Roles : IdentityRole<int>, ITenantEntity, IEntity
    {
        public int? TenantId { get; set; }
        public string? DbKey { get; set; }
    }

}

    
