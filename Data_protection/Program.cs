namespace Data_protection
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var dbPath = "D:\\My_programms\\C#\\Data_protection/database.db";
            var userRepository = new UserRepository(dbPath);
            var authService = new AuthService(userRepository);

            // Пример регистрации пользователя
            authService.RegisterUser("Dmitry_Medvedev", "admin_password", "admin");
            authService.RegisterUser("Dmitry_Medvedev", "admin_password", "admin");
            authService.RegisterUser("Ivan", "admin_password", "User");

            // Пример аутентификации пользователя
            bool isAuthenticated = authService.AuthenticateUser("ivan", "admin_password", "User");

            if (isAuthenticated)
            {
                Console.WriteLine($"Пользователь успешно аутентифицирован.");
            }
            else
            {
                Console.WriteLine("Ошибка аутентификации. Пользователь не найден или введен неверный пароль.");
            }
        }
    }
}