namespace ecommerce_user.Entities;

public class User(string username, string email, string passwordHash)
{
    public int Id { get; private set; }
    public string Username { get; private set; } = username;
    public string Email { get; private set; } = email;
    public string PasswordHash { get; private set; } = passwordHash;
}