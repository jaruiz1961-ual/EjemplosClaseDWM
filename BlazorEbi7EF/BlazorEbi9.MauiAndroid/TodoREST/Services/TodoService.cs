using BlazorEbi7.Model.Entidades;
using TodoREST.Models;

namespace TodoREST.Services
{
    public class TodoService : ITodoService
    {
        IRestService _restService;

        public TodoService(IRestService service)
        {
            _restService = service;
        }

        public Task<List<UsuarioSet>> GetTasksAsync()
        {
            return _restService.RefreshDataAsync();
        }

        public Task SaveTaskAsync(UsuarioSet item, bool isNewItem = false)
        {
            return _restService.SaveTodoItemAsync(item, isNewItem);
        }

        public Task DeleteTaskAsync(UsuarioSet item)
        {
            dynamic id = item.Id;
            return _restService.DeleteTodoItemAsync(id);
        }
    }
}
