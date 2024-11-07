namespace MeuPatrimonio.User.Entities;

public class User(string username, string email, string passwordHash)
{
    public int Id { get; }
    public string Username { get; private set; } = username;
    public string Email { get; private set; } = email;
    public string PasswordHash { get; private set; } = passwordHash;
}