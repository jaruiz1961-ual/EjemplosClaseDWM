using BlazorEbi7.Model.Entidades;
using TodoREST.Models;

namespace TodoREST.Services
{
    public interface IRestService
    {
        Task<List<UsuarioSet>> RefreshDataAsync();

        Task SaveTodoItemAsync(UsuarioSet item, bool isNewItem);

        Task DeleteTodoItemAsync(string id);
    }
}
