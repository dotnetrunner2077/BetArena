namespace Domain.Entities;

public class Game
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Rtp { get; set; }

    public ICollection<Bet> Bets { get; set; } = new List<Bet>();
}
