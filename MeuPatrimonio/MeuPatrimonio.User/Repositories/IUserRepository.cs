namespace MeuPatrimonio.User.Repositories;

public interface IUserRepository
{
    Task<Entities.User?> GetByUsernameAsync(string username);
    Task AddAsync(Entities.User user);
}