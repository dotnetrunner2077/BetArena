namespace Applications.Layer.DTOs;

public class PlaceBetResponse
{
    public int BetId { get; set; }
    public int UserId { get; set; }
    public string GameName { get; set; } = string.Empty;
    public decimal Stake { get; set; }
    public decimal WinAmount { get; set; }
    public string Result { get; set; } = string.Empty;
    public decimal GameRtp { get; set; }
}
