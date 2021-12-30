using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class PlayerController : ControllerBase
{
    private readonly DataContext _dataContext;

    public PlayerController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    [HttpGet(Name = "GetStatus")]
    public async Task<PlayerStatus> GetPlayerStatus(int id)
    {
        var playerStatus = await _dataContext.PlayerStatuses.FirstAsync(i => i.Id == id);

        return playerStatus;
        
    }
}