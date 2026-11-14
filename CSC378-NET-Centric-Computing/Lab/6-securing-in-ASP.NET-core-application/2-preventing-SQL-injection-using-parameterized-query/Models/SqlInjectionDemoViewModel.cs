namespace _2_preventing_SQL_injection_using_parameterized_query.Models;

public class SqlInjectionDemoViewModel
{
    public string Username { get; set; } = "' OR '1'='1' --";
    public string Password { get; set; } = "anything";
    public string? LoginType { get; set;}
    public bool LoginSucceeded { get; set; }
    public List<UserRecord> Users { get; set; } = [];
}

public class UserRecord
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}
