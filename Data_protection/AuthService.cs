using System;
using System.Security.Cryptography;
using System.Text;

internal class AuthService
{
    private readonly UserRepository userRepository;

    public AuthService(UserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public void RegisterUser(string username, string password, string role)
    {
        // Хешируем пароль перед сохранением в базу данных
        string passwordHash = HashPassword(password);

        // Создаем нового пользователя
        var user = new User { Username = username, PasswordHash = passwordHash , Role = role};

        // Сохраняем пользователя в базу данных
        userRepository.CreateUser(user);
    }

    public bool AuthenticateUser(string username, string password, string requiredRole = null)
    {
        // Получаем пользователя из базы данных по имени пользователя
        var user = userRepository.GetUserByUsername(username);

        if (user != null && VerifyPassword(password, user.PasswordHash))
        {
            // Проверяем, совпадает ли роль пользователя с требуемой ролью (если требуется)
            if (requiredRole == null || string.Equals(user.Role, requiredRole, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            // Хешируем пароль с использованием SHA-256
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            // Преобразуем байты в строку для хранения в базе данных
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }

    private bool VerifyPassword(string password, string storedHash)
    {
        // Проверяем, совпадает ли хеш введенного пароля с хешем из базы данных
        return string.Equals(HashPassword(password), storedHash, StringComparison.OrdinalIgnoreCase);
    }
}