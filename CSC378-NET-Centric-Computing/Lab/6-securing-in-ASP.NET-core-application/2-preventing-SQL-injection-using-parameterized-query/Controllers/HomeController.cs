using _2_preventing_SQL_injection_using_parameterized_query.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Diagnostics;

namespace _2_preventing_SQL_injection_using_parameterized_query.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            using var db = new SqliteConnection("Data Source=:memory:");
            db.Open();
            using var setup = db.CreateCommand();
            setup.CommandText = "CREATE TABLE Users (Username TEXT, Password TEXT); INSERT INTO Users VALUES ('admin', '1234');";
            setup.ExecuteNonQuery();

            var username = "' OR '1'='1";
            using var unsafeCommand = db.CreateCommand();
            unsafeCommand.CommandText = $"SELECT COUNT(*) FROM Users WHERE Username = '{username}' AND Password = 'wrong'";

            using var safeCommand = db.CreateCommand();
            safeCommand.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = $username AND Password = $password";
            safeCommand.Parameters.AddWithValue("$username", username);
            safeCommand.Parameters.AddWithValue("$password", "wrong");

            ViewBag.UnsafeResult = unsafeCommand.ExecuteScalar();
            ViewBag.SafeResult = safeCommand.ExecuteScalar();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
