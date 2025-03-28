using Dapper;
using Npgsql;
using WPF_MVVM_Demo.Models;

namespace WPF_MVVM_Demo.DataAccess;

public static class DataContext
{
    private const string CONNECTION_STRING =
        "Server=127.0.0.1;Port=5432;Database=users_db;User Id=postgres;Password=1234;";

    static DataContext()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    public static async Task<bool> AddUserAsync(User user)
    {
        await using var db = new NpgsqlConnection(CONNECTION_STRING);
        await db.OpenAsync();
        const string SQL = """
                           INSERT INTO table_users (last_name, first_name, date_of_birth) 
                           VALUES (@LastName, @FirstName, @DateOfBirth)
                           """;
        var result = await db.ExecuteAsync(SQL, user);
        await db.CloseAsync();

        return result > 0;
    }

    public static async Task<bool> DeleteUserAsync(int userId)
    {
        await using var db = new NpgsqlConnection(CONNECTION_STRING);
        await db.OpenAsync();
        const string SQL = "DELETE FROM table_users WHERE id=@UserId";
        var result = await db.ExecuteAsync(SQL, userId);
        await db.CloseAsync();

        return result > 0;
    }

    public static async Task<bool> UpdateUserAsync(User user)
    {
        await using var db = new NpgsqlConnection(CONNECTION_STRING);
        await db.OpenAsync();
        const string SQL = """
                           UPDATE table_users 
                           SET last_name=@LastName, first_name=@FirstName, date_of_birth=@DateOfBirth 
                           WHERE id=@Id
                           """;
        var result = await db.ExecuteAsync(SQL, user);
        await db.CloseAsync();

        return result > 0;
    }

    public static async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        await using var db = new NpgsqlConnection(CONNECTION_STRING);
        await db.OpenAsync();

        const string SQL = "SELECT * FROM table_users";
        var users = await db.QueryAsync<User>(SQL);

        await db.CloseAsync();

        return users;
    }
}