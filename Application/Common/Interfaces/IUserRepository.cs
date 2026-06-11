using GestaoPedidosAPI.Domain.Entities;

namespace GestaoPedidosAPI.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
