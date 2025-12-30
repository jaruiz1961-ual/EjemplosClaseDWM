using BlazorSeguridad2026.Base;
using BlazorSeguridad2026.Base.Modelo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSeguridad2026.Data.Modelo
{
    using BibliotecaContextosDb.Resources;
    using System.ComponentModel.DataAnnotations;
    using System.Linq.Expressions;
  

    public class UserModel
    {
        [Display(
            Name = "User_Codigo_Label",
            ResourceType = typeof(UsuarioResources))]
        [Required(
            ErrorMessageResourceName = "User_Codigo_Required",
            ErrorMessageResourceType = typeof(UsuarioResources))]
        [StringLength(20,
            MinimumLength = 3,
            ErrorMessageResourceName = "User_Codigo_Length",
            ErrorMessageResourceType = typeof(UsuarioResources))]
        public string Codigo { get; set; } = string.Empty;

        [Display(
            Name = "User_UserName_Label",
            ResourceType = typeof(UsuarioResources))]
        [Required(
            ErrorMessageResourceName = "User_UserName_Required",
            ErrorMessageResourceType = typeof(UsuarioResources))]
        [StringLength(50,
            MinimumLength = 3,
            ErrorMessageResourceName = "User_UserName_Length",
            ErrorMessageResourceType = typeof(UsuarioResources))]
        public string UserName { get; set; } = string.Empty;

        [Display(
            Name = "User_NivelAcceso_Label",
            ResourceType = typeof(UsuarioResources))]
        [Required(
            ErrorMessageResourceName = "User_NivelAcceso_Required",
            ErrorMessageResourceType = typeof(UsuarioResources))]
        public string Contexto { get; set; } = string.Empty;

        [Display(
            Name = "User_Password_Label",
            ResourceType = typeof(UsuarioResources))]
        [Required(
            ErrorMessageResourceName = "User_Password_Required",
            ErrorMessageResourceType = typeof(UsuarioResources))]
        [StringLength(100,
            MinimumLength = 6,
            ErrorMessageResourceName = "User_Password_Length",
            ErrorMessageResourceType = typeof(UsuarioResources))]
        public string Password { get; set; } = string.Empty;

        [Display(
            Name = "User_Email_Label",
            ResourceType = typeof(UsuarioResources))]
        [Required(
            ErrorMessageResourceName = "User_Email_Required",
            ErrorMessageResourceType = typeof(UsuarioResources))]
        [EmailAddress(
            ErrorMessageResourceName = "User_Email_Invalid",
            ErrorMessageResourceType = typeof(UsuarioResources))]
        public string Email { get; set; } = string.Empty;

        [Display(
            Name = "User_Tenant_Label",
            ResourceType = typeof(UsuarioResources))]
        [Range(1, int.MaxValue,
            ErrorMessageResourceName = "User_Tenant_Range",
            ErrorMessageResourceType = typeof(UsuarioResources))]
        public int TenantId { get; set; }

        // Campos solo de UI (opcionalmente)
        [Display(
            Name = "User_PasswordConfirm_Label",
            ResourceType = typeof(UsuarioResources))]
        [Compare("Password",
            ErrorMessageResourceName = "User_PasswordConfirm_Compare",
            ErrorMessageResourceType = typeof(UsuarioResources))]
        public string? ConfirmPassword { get; set; }


    }
}
