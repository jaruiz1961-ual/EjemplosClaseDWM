using BlazorEbi9.Model.Entidades;

namespace BlazorEbi9.Model.IServices
{
    public interface IUsuarioServiceAsync
    {
        Task<List<UsuarioSet>> FindAllAsync();
        Task<UsuarioSet> SaveUserAsync(UsuarioSet user);
        Task<UsuarioSet> FindIdAsync(int id);
        Task<bool> DeleteIdAsync(int id);
    }

}
