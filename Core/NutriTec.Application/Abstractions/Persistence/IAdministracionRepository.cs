using NutriTec.Domain.Productos;

namespace NutriTec.Application.Abstractions.Persistence;

public interface IAdministracionRepository
{
    Task<IReadOnlyCollection<Producto>> ListarProductosPendientesAsync(CancellationToken cancellationToken);

    Task<bool> AprobarProductoAsync(Guid idProducto, CancellationToken cancellationToken);
}
