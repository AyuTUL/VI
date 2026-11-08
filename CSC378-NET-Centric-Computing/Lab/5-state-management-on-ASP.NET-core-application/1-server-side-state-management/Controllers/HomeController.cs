using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        // Store data in Session
        HttpContext.Session.SetString("Name", "Heung-Min Son");
        return View();
    }

    public IActionResult Show()
    {
        // Retrieve data from Session
        string? name = HttpContext.Session.GetString("Name");
        ViewBag.Name = name;
        return View();
    }
}