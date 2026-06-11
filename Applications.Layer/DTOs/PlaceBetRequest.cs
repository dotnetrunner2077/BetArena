namespace Applications.Layer.DTOs;

public class PlaceBetRequest
{
    public int UserId { get; set; }
    public string GameName { get; set; } = string.Empty;
    public decimal Stake { get; set; }
    public decimal WinAmount { get; set; }
}
