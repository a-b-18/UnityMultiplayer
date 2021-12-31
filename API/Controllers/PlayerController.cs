using System.Linq;
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

    [HttpGet("Single", Name = "ReadStatus")]
    public async Task<PlayerStatus> GetPlayerStatus(int id)
    {
        var playerStatus = await _dataContext.PlayerStatuses.FirstAsync(i => i.Id == id);

        return playerStatus;
    }
    
    [HttpGet("List", Name = "ReadStatuses")]
    public async Task<PlayerStatus[]> GetPlayerStatuses()
    {
        var playerStatus = await _dataContext.PlayerStatuses.ToArrayAsync();

        return playerStatus;
    }
    
    [HttpPut(Name = "WriteStatus")]
    public async Task<Boolean> WritePlayerStatus(PlayerStatus playerStatus)
    {
        var returnStatus = _dataContext.PlayerStatuses.Update(playerStatus);
        
        return await _dataContext.SaveChangesAsync() > 0;
    }
}