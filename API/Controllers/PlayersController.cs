using System.Linq;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class PlayersController : ControllerBase
{
    private readonly DataContext _dataContext;

    public PlayersController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    [HttpGet("User")]
    public async Task<PlayerStatus> GetUserPlayer(int userId)
    {
        var userPlayer = new PlayerStatus();
        try
        {
            userPlayer = await _dataContext.PlayerStatuses.FirstAsync(i => i.Id == userId);
        } catch (InvalidOperationException)
        {
            Console.WriteLine("Received invalid user player request.");
        }
        return userPlayer;
    }
    
    [HttpGet("Online")]
    public async Task<List<PlayerStatus>> GetOnlinePlayers(int userId)
    {
        var onlinePlayers = await _dataContext.PlayerStatuses
                                                            .Where(user => user.Id != userId)
                                                            .ToListAsync();
        return onlinePlayers;
    }
    
    [HttpPut("User")]
    public async Task<ActionResult<Boolean>> PutUserPlayer(PlayerStatus playerStatus)
    {
        var result = false;
        
        
        var returnStatus = _dataContext.PlayerStatuses.Update(playerStatus);
        try
        {
            result = await _dataContext.SaveChangesAsync() > 0;
        } catch (DbUpdateConcurrencyException)
        {
            Console.WriteLine("Received invalid user player request.");
            return BadRequest(false);
        }

        return Ok(result);
    }
}