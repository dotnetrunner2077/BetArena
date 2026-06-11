namespace Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;

    public ICollection<Bet> Bets { get; set; } = new List<Bet>();
}
