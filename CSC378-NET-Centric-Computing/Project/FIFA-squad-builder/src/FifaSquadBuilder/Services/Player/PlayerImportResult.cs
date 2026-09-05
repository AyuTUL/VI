namespace FifaSquadBuilder.Services.Player;

public class PlayerImportResult
{
    public int NationsCreated { get; set; }
    public int LeaguesCreated { get; set; }
    public int ClubsCreated { get; set; }
    public int PlayersCreated { get; set; }
    public int PlayersUpdated { get; set; }
    public int CardImagesAssigned { get; set; }
    public int RowsSkipped { get; set; }
    public List<string> Errors { get; set; } = new();
}
