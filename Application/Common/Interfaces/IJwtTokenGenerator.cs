using GestaoPedidosAPI.Domain.Entities;

namespace GestaoPedidosAPI.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
