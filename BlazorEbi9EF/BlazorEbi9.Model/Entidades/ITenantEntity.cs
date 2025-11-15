
namespace BlazorEbi9.Model.Entidades
{
    // Marca las entidades que deben recibir TenantId automático.
    public interface ITenantEntity
    {
        int TenantId { get; set; }
    }
}