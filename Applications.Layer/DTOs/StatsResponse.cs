namespace Applications.Layer.DTOs;

public class StatsResponse
{
    public List<GameStats> Games { get; set; } = new();
    public List<UserStats> Users { get; set; } = new();
}

public class GameStats
{
    public string Game { get; set; } = string.Empty;
    public decimal Rtp { get; set; }
}

public class UserStats
{
    public int UserId { get; set; }
    public decimal TotalStake { get; set; }
    public decimal TotalWin { get; set; }
}
