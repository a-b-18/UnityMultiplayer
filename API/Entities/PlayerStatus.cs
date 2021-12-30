namespace API.Entities;

public class PlayerStatus
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public double PosX { get; set; }
    public double PosY { get; set; }
    public double Angle { get; set; }
    public int Health { get; set; }
    public int Score { get; set; }
}