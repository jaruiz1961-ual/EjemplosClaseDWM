using BlazorSeguridad2026.Data;
using Microsoft.AspNetCore.Identity;
using Shares.Genericos;
using Shares.Seguridad;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace BlazorSeguridad2026.Components.Seguridad
{
    public interface IRoleService
    {
        Task<List<ApplicationRole>> GetAllAsync();
        Task<IdentityResult> CreateAsync(string name);
        Task<ApplicationRole?> GetByIdAsync(int id);
        Task<IdentityResult> UpdateRoleAsync(int id, string newName);
        Task<IdentityResult> DeleteAsync(int id);
    }
    public class RoleServiceMio : IRoleService
    {
        IContextProvider _contextProvider;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private IUnitOfWorkAsync uow;
        bool reload = true;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private int? _tenantId;
        private string _dbKey;
        bool EsWasm => RuntimeInformation.IsOSPlatform(OSPlatform.Create("Browser"));

        public RoleServiceMio(RoleManager<ApplicationRole> roleManager, IContextProvider contextKeyProvider, IUnitOfWorkFactory uowFactory)
        {
            _contextProvider = contextKeyProvider.Copia();
            _contextProvider._AppState.ApplyTenantFilter = true;
            _contextProvider._AppState.DbKey = "Application";
            if (EsWasm) _contextProvider._AppState.ConnectionMode = "Api";  // Establece el contexto adecuado para la base de datos de usuarios en WASM
            else _contextProvider._AppState.ConnectionMode = "Ef";
            _contextProvider._AppState.DbKey = "Application"; // Establece el contexto adecuado para la base de datos de usuarios
            _unitOfWorkFactory = uowFactory;
   

            //original
            _roleManager = roleManager;
        }

        //original
    //    public Task<List<ApplicationRole>> GetAllAsync() =>
    //Task.FromResult(_roleManager.Roles.ToList());
        public async Task<List<ApplicationRole>> GetAllAsync()
        {
            if (uow == null)
                uow = _unitOfWorkFactory.Create(_contextProvider);

            var repo = uow.GetRepository<ApplicationRole>(reload);

            var allEntities = await repo.GetAllAsync(reload); // IEnumerable<ApplicationUser> o similar 
            if (allEntities == null) return null;
            var lista = allEntities.ToList();
            return lista;
        }



        public async Task<IdentityResult> CreateAsync(string name)
        {
            var role = new ApplicationRole
            {
                Name = name.Trim(),
                NormalizedName = name.Trim().ToUpperInvariant(),
                TenantId = _tenantId,
                DbKey = _dbKey
            };

            return await _roleManager.CreateAsync(role);
        }

        //original
        //public Task<ApplicationRole?> GetByIdAsync(int id) =>
    //Task.FromResult(_roleManager.Roles.FirstOrDefault(r => r.Id == id));

        public async Task<ApplicationRole?> GetByIdAsync(int id)
        {
            if (uow == null)
                uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<ApplicationRole>(reload);
            if (repo == null) return null;

            var entity = await repo.GetByIdAsync(id, reload);

            return entity;
            
        }

        public async Task<IdentityResult> UpdateRoleAsync(int id, string newName)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            // O mejor: _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == id)
            if (role is null)
                return IdentityResult.Failed(new IdentityError { Description = "Role not found." });

            role.Name = newName.Trim();
            role.NormalizedName = role.Name.ToUpperInvariant();
            role.TenantId = _tenantId;
            role.DbKey = _dbKey;
            return await _roleManager.UpdateAsync(role);
        }

        public async Task<IdentityResult> DeleteAsync(int id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role is null)
                return IdentityResult.Failed(new IdentityError { Description = "Role not found." });

            return await _roleManager.DeleteAsync(role);
        }
    }
}
