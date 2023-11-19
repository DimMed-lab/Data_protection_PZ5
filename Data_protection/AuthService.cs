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
        // Генерация случайной соли
        string salt = GenerateSalt();
        // Генерация случайной соли
        // Хешируем пароль перед сохранением в базу данных
        string passwordHash = HashPassword(password, salt);
        // Хешируем имя пользователя перед сохранением в базу данных
        string usernameHash = HashUsername(username);

        // Создаем нового пользователя
        try
        {
            var user = new User { Username = usernameHash, PasswordHash = passwordHash, Salt = salt, Role = role };
            // Сохраняем пользователя в базу данных
            userRepository.CreateUser(user);
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }
    }

    private string GenerateSalt()
    {
        // Генерация случайной строки
        return Guid.NewGuid().ToString();
    }

    public bool AuthenticateUser(string username, string password, string requiredRole = null)
    {
        // Получаем пользователя из базы данных по имени пользователя
        var user = userRepository.GetUserByUsername(HashUsername(username));

        if (user != null && VerifyPassword(password, user.PasswordHash, user.Salt))
        {
            // Проверяем, совпадает ли роль пользователя с требуемой ролью (если требуется)
            if (requiredRole == null || string.Equals(user.Role, requiredRole, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private string HashPassword(string password, string salt)
    {
        using (var sha256 = SHA256.Create())
        {
            // Конкатенация пароля и соли перед хешированием
            byte[] saltedPasswordBytes = Encoding.UTF8.GetBytes(password + salt);
            // Хешируем пароль с использованием SHA-256
            byte[] hashedBytes = sha256.ComputeHash(saltedPasswordBytes);

            // Преобразуем байты в строку для хранения в базе данных
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }

    private string HashUsername(string username)
    {
        username = username.ToLower();
        using (var md5 = MD5.Create())
        {
            byte[] saltedUsernameBytes = Encoding.UTF8.GetBytes(username);

            // Хеширование имени пользователя с использованием SHA-256
            byte[] hashedBytes = md5.ComputeHash(saltedUsernameBytes);

            // Преобразование байтов в строку для хранения в базе данных
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }

    private bool VerifyPassword(string password, string storedHash, string salt)
    {
        // Проверяем, совпадает ли хеш введенного пароля с хешем из базы данных
        return string.Equals(HashPassword(password, salt), storedHash, StringComparison.OrdinalIgnoreCase);
    }
}