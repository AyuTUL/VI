using FifaSquadBuilder.Services.Player;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FifaSquadBuilder.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "AdminOnly")]
public class ImportController : Controller
{
    private readonly IPlayerImportService _importService;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public ImportController(IPlayerImportService importService, IConfiguration configuration, IWebHostEnvironment environment)
    {
        _importService = importService;
        _configuration = configuration;
        _environment = environment;
    }

    // GET /Admin/Import
    public IActionResult Index()
    {
        return View();
    }

    // POST /Admin/Import/Run
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Run()
    {
        var playersRelative = _configuration["DatasetPaths:PlayersCsv"]
            ?? "App_Data/dataset/players_fifa23.csv";
        var teamsRelative = _configuration["DatasetPaths:TeamsCsv"]
            ?? "App_Data/dataset/teams_fifa23.csv";

        var playersPath = Path.Combine(_environment.ContentRootPath, playersRelative);
        var teamsPath = Path.Combine(_environment.ContentRootPath, teamsRelative);

        try
        {
            var result = await _importService.ImportAsync(playersPath, teamsPath);
            TempData["ImportSummary"] =
                $"Players: {result.PlayersCreated} created, {result.PlayersUpdated} updated, {result.RowsSkipped} skipped. " +
                $"Card images: {result.CardImagesAssigned} assigned. " +
                $"Nations: {result.NationsCreated} created. Leagues: {result.LeaguesCreated} created. Clubs: {result.ClubsCreated} created. " +
                $"Errors/warnings: {result.Errors.Count}.";
            TempData["ImportErrors"] = string.Join("\n", result.Errors.Take(50));
        }
        catch (FileNotFoundException ex)
        {
            TempData["ImportSummary"] = $"Import failed: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
}
