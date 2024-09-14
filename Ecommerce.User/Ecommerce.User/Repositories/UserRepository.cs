using ecommerce_user.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_user.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _context;

    public UserRepository(UserDbContext context)
    {
        _context = context;
    }

    public async Task<Entities.User?> GetByUsernameAsync(string username) =>
        await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

    public async Task AddAsync(Entities.User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}