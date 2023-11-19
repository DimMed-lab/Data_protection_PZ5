using System;
using System.Data.SQLite;
using Dapper;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Salt {  get; set; }
    public string Role {  get; set; }
}

internal class UserRepository
{
    private readonly string connectionString;

    public UserRepository(string dbPath)
    {
        connectionString = $"Data Source={dbPath};Version=3;";
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            // Создаем таблицу пользователей, если она не существует
            connection.Execute(@"
                CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL UNIQUE,
                PasswordHash TEXT NOT NULL,
                Salt TEXT NOT NULL,
                Role TEXT NOT NULL DEFAULT 'User' -- Добавлено поле для роли с значением 'User' по умолчанию
            )");
        }
    }

    public void CreateUser(User user)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            // Вставляем нового пользователя в базу данных
            connection.Execute("INSERT INTO Users (Username, PasswordHash, Salt, Role) VALUES (@Username, @PasswordHash, @Salt,  @Role)", user);
        }
    }

    public User GetUserByUsername(string username)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            // Получаем пользователя по его имени пользователя
            return connection.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE Username = @Username", new { Username = username });
        }
    }
}

