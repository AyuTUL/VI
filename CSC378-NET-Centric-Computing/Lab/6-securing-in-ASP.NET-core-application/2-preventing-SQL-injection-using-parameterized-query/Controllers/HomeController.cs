using _2_preventing_SQL_injection_using_parameterized_query.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace _2_preventing_SQL_injection_using_parameterized_query.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View(new SqlInjectionDemoViewModel
    {
        Users = GetUsers()
    });

    [HttpPost]
    public IActionResult RawLogin(SqlInjectionDemoViewModel model)
    {
        using var db = CreateDatabase();
        using var command = db.CreateCommand();
        command.CommandText = $"SELECT COUNT(*) FROM Users WHERE Username = '{model.Username}' AND Password = '{model.Password}'";
        model.LoginType = "Raw string concatenation";
        model.LoginSucceeded = Convert.ToInt32(command.ExecuteScalar()) > 0;
        model.Users = model.LoginSucceeded ? GetUsers(db) : [];
        return PartialView("_LoginResult", model);
    }

    [HttpPost]
    public IActionResult ParameterizedLogin(SqlInjectionDemoViewModel model)
    {
        using var db = CreateDatabase();
        using var command = db.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = $username AND Password = $password";
        command.Parameters.AddWithValue("$username", model.Username);
        command.Parameters.AddWithValue("$password", model.Password);
        model.LoginType = "Parameterized query";
        model.LoginSucceeded = Convert.ToInt32(command.ExecuteScalar()) > 0;
        model.Users = model.LoginSucceeded ? GetUsers(db) : [];
        return PartialView("_LoginResult", model);
    }

    private static List<UserRecord> GetUsers(SqliteConnection? existingConnection = null)
    {
        using var db = existingConnection ?? CreateDatabase();
        using var command = db.CreateCommand();
        command.CommandText = "SELECT Id, Username, Password FROM Users";
        using var reader = command.ExecuteReader();
        var users = new List<UserRecord>();
        while (reader.Read())
        {
            users.Add(new UserRecord
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2)
            });
        }
        return users;
    }

    private static SqliteConnection CreateDatabase()
    {
        var db = new SqliteConnection("Data Source=:memory:");
        db.Open();
        using var setup = db.CreateCommand();
        setup.CommandText = "CREATE TABLE Users (Id INTEGER PRIMARY KEY, Username TEXT, Password TEXT); INSERT INTO Users (Username, Password) VALUES ('admin', 'secret123'), ('yurisha', 'pass123');";
        setup.ExecuteNonQuery();
        return db;
    }
}
