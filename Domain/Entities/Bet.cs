namespace Domain.Entities;

public class Bet
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public decimal WinAmount { get; set; }
    public string Result { get; set; } = "Pending";

    public Game Game { get; set; } = null!;
    public User User { get; set; } = null!;
}
