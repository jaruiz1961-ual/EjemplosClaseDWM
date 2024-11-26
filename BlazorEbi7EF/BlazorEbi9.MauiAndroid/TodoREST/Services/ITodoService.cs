using BlazorEbi7.Model.Entidades;
using TodoREST.Models;

namespace TodoREST.Services
{
    public interface ITodoService
    {
        Task<List<UsuarioSet>> GetTasksAsync();
        Task SaveTaskAsync(UsuarioSet item, bool isNewItem);
        Task DeleteTaskAsync(UsuarioSet item);
    }
}
