using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class PlayerController : ControllerBase
{
    [HttpGet(Name = "GetStatus")]
    public PlayerStatus GetPlayerStatus()
    {
        return new PlayerStatus()
            {
                Id = 1,
                UserName = "Alex",
                PosX = 12.3,
                PosY = 11.7,
                Angle = 30.2,
                Health = 88,
                Score = 100
            };
    }
}