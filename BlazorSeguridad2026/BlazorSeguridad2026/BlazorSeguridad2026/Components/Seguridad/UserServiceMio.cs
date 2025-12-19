using BlazorSeguridad2026.Data;
using Microsoft.AspNetCore.Identity;
using Shares.Genericos;
using Shares.Seguridad;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlazorSeguridad2026.Components.Seguridad
{

    public class UserServiceMio : IUserService
    {
        IContextProvider _contextProvider;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private IUnitOfWorkAsync uow;
        bool reload = true;
        private readonly UserManager<ApplicationUser> _userManager;


        public UserServiceMio(UserManager<ApplicationUser> userManager,IContextProvider contextKeyProvider, IUnitOfWorkFactory uowFactory)
        {
            _contextProvider = contextKeyProvider;
            _unitOfWorkFactory = uowFactory;
            _userManager = userManager;

        }

        public async Task<List<ApplicationUser>> GetAllAsync()
        {
            if (uow == null)
                uow = _unitOfWorkFactory.Create(_contextProvider);

            var repo = uow.GetRepository<Users>(reload);

            var allEntities = await repo.GetAllAsync(reload); // IEnumerable<ApplicationUser> o similar 
            var lista = ApplicationUser.Convert(allEntities.ToList());
            return lista;                 
        }


        public async Task<ApplicationUser?> GetByIdAsync(int id) 
            {
                        if (uow == null)
                uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<Users>(reload);
       
            var entity = await repo.GetByIdAsync(id, reload);
            var entidad = ApplicationUser.Convert(new List<Users> { entity }).FirstOrDefault();
            return entidad;
            
            }

        public async Task<IdentityResult> CreateAsync(string email, string password, int? tenantId, string keyDb)
        {
            // 1) Crear la entidad de Identity
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                TenantId = tenantId,
                DbKey = keyDb
            };

            // 2) Crear el usuario con Identity (valida password, etc.)
            var identityResult = await _userManager.CreateAsync(user, password);
            if (!identityResult.Succeeded)
                return identityResult;

            // 3) Si quieres que pase también por tu UoW/Repositorio (opcional, normalmente con Identity no hace falta):
            if (uow == null)
                uow = _unitOfWorkFactory.Create(_contextProvider);

            var repo = uow.GetRepository<ApplicationUser>(reload: true);
            await repo.Add(user, reload: true);
            await uow.SaveChangesAsync();

            return IdentityResult.Success;
        }


        public async Task<IdentityResult> UpdateUserAsync(int id, Action<ApplicationUser> updateAction)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user is null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            updateAction(user);
            return await _userManager.UpdateAsync(user);
        }
        

       

        public async Task<IdentityResult> DeleteAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user is null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            return await _userManager.DeleteAsync(user);
        }

        public async Task<IList<string>> GetUserRolesAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user is null
                ? Array.Empty<string>()
                : await _userManager.GetRolesAsync(user);
        }

        public async Task<IdentityResult> SetUserRolesAsync(int userId, IEnumerable<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            var currentRoles = await _userManager.GetRolesAsync(user);

            var toRemove = currentRoles.Except(roles).ToArray();
            var toAdd = roles.Except(currentRoles).ToArray();

            if (toRemove.Length > 0)
            {
                var r1 = await _userManager.RemoveFromRolesAsync(user, toRemove);
                if (!r1.Succeeded) return r1;
            }

            if (toAdd.Length > 0)
            {
                var r2 = await _userManager.AddToRolesAsync(user, toAdd);
                if (!r2.Succeeded) return r2;
            }

            return IdentityResult.Success;
        }
    }
}

