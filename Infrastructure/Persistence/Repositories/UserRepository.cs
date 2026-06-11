using GestaoPedidosAPI.Application.Common.Interfaces;
using GestaoPedidosAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestaoPedidosAPI.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
}
