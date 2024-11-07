using MeuPatrimonio.User.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MeuPatrimonio.User.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _context;

    public UserRepository(UserDbContext context)
    {
        _context = context;
    }

    public async Task<Entities.User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
    }

    public async Task AddAsync(Entities.User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}