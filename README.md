# E-commerce User Service

## Purpose

This service handles user authentication, registration, and login for the e-commerce platform. It uses JWT tokens for user authentication and Serilog for logging.

## APIs

- **POST /api/auth/register**

  - Registers a new user.
  - Payload:
    ```json
    {
      "username": "string",
      "email": "string",
      "password": "string"
    }
    ```

- **POST /api/auth/login**
  - Logs in a user and returns a JWT token.
  - Payload:
    ```json
    {
      "username": "string",
      "password": "string"
    }
    ```

## How to Run

1. Install dependencies using the provided bash script: `bash bash_install.sh`
2. Run the application using Docker or with `dotnet run`.

## Testing

Unit and integration tests are provided in the `ecommerce.User.UnitTests` and `ecommerce.User.IntegrationTests` projects. Use `dotnet test` to run the tests.

## Security

- JWT tokens are used for stateless authentication.
- Sensitive data like passwords are encrypted using BCrypt.
- Input validation is enforced to prevent SQL injection and XSS attacks.
- Secrets are stored securely and not hardcoded in the source code.
