using GameApi.Dto;
using GameApi.GameLogic;
using GameApi.Infrastructure;
using GameApi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GameApi.Controllers;
[Route("api/game")]
[ApiController]
public class GameInstancesController : ControllerBase
{
    private readonly SqlDbContext _context;
    private readonly IMemoryCache _memoryCache;

    public GameInstancesController(IMemoryCache memoryCache, SqlDbContext context)
    {
        _memoryCache = memoryCache;
        _context = context;
    }

    // GET: api/game
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameInstance>>> GetGameInstance()
    {
        var cacheKey = "userList";
        //checks if cache entries exists
        if (!_memoryCache.TryGetValue(cacheKey, out List<GameInstance> instanceList))
        {
            //calling the server
            instanceList = await _context.GameInstance.ToListAsync();

            //setting up cache options
            var cacheExpiryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(50),
                Priority = CacheItemPriority.High,
                SlidingExpiration = TimeSpan.FromDays(1)
            };
            //setting cache entries
            _memoryCache.Set(cacheKey, instanceList, cacheExpiryOptions);
        }
        return Ok(instanceList);

        //if (_context.GameInstance == null)
        //{
        //    return NotFound();
        //}
        //return await _context.GameInstance.ToListAsync();
    }

    // GET: api/game/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GameInstance>> GetGameInstance(int id)
    {
        var cacheKey = "gameInstance";
        //checks if cache entries exists
        if (!_memoryCache.TryGetValue(cacheKey, out GameInstance instance))
        {
            //calling the server
            instance = await _context.GameInstance.FindAsync(id);

            //setting up cache options
            var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(3));

            //setting cache entries
            _memoryCache.Set(cacheKey, instance, cacheEntryOptions);
        }
        if (instance == null)
        {
            return NotFound();
        }
        return Ok(instance);

        //if (_context.GameInstance == null)
        //{
        //    return NotFound();
        //}
        //var gameInstance = await _context.GameInstance.FindAsync(id);

        //if (gameInstance == null)
        //{
        //    return NotFound();
        //}

        //return gameInstance;
    }

    // POST: api/game
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<GameInstance>> PostGameInstance(GameInstanceDTO gameInstanceDto)
    {
        if (_context.GameInstance == null || _context.GameStep == null || _context.User == null)
        {
            return Problem("Entity set 'Game2048Context.GameInstance' or 'Game2048Context.GameStep' or ''Game2048Context.User' is null.");
        }

        GameBoard gameBoard;
        Tile newTile;

        try
        {
            gameBoard = new GameBoard(gameInstanceDto.ColumnSize, gameInstanceDto.RowSize);
            newTile = gameBoard.AddRandomTile();
        }
        catch
        {
            return BadRequest("Unable to initialize game board");
        }

        try
        {
            _context.User.Where(user => user.UserId == gameInstanceDto.UserId).First();
        }
        catch
        {
            return BadRequest("User doesn't exist");
        }


        var gameInstance = new GameInstance
        {
            UserId = gameInstanceDto.UserId,
            GameActive = true,
            Score = 0,
            RowSize = gameInstanceDto.RowSize,
            ColumnSize = gameInstanceDto.ColumnSize,
            LatestGrid = gameBoard.ToSerializedString()
        };

        _context.GameInstance.Add(gameInstance);
        await _context.SaveChangesAsync();

        var gameStep = new GameStep
        {
            GameInstanceId = gameInstance.GameInstanceId,
            NewTileRow = newTile.Row,
            NewTileColumn = newTile.Column,
            NewTileExponent = newTile.Exponent,
            LastMove = MoveType.System
        };
        _context.GameStep.Add(gameStep);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetGameInstance", gameInstance);
    }

    [HttpPost]
    [Route("new-move")]

    public async Task<ActionResult<GameInstance>> PostGameStep(GameStepDTO gameStepDto)
    {
        if (_context.GameStep == null || _context.GameInstance == null)
        {
            return Problem("Entity set 'Game2048Context.GameStep' or 'Game2048Context.GameInstance'  is null.");
        }

        var gameInstance = _context.GameInstance.Where(instance => instance.GameInstanceId == gameStepDto.GameInstanceId).FirstOrDefault();

        if (gameInstance == null || !gameInstance.GameActive)
        {
            return BadRequest("Unable to find the game, or that the game has ended");
        }

        GameBoard board = new GameBoard(gameInstance.LatestGrid);

        var pointsFromMove = 0;
        MoveType moveType;
        Tile newTile;
        try
        {
            moveType = getMoveType(gameStepDto.Move);
            pointsFromMove = board.TryToMove(gameStepDto.Move);
            newTile = board.AddRandomTile();
        }
        catch
        {
            return BadRequest("Unable to make move");
        }

        gameInstance.Score = gameInstance.Score + pointsFromMove;

        GameStep gameStep = new GameStep
        {
            GameInstanceId = gameInstance.GameInstanceId,
            NewTileColumn = newTile.Column,
            NewTileRow = newTile.Row,
            NewTileExponent = newTile.Exponent,
            LastMove = moveType
        };

        _context.GameStep.Add(gameStep);
        gameInstance.LatestGrid = board.ToSerializedString();
        gameInstance.GameActive = board.IsGameWon();
        await _context.SaveChangesAsync();

        return Ok(gameInstance);
    }

    [HttpPost]
    [Route("end-game")]

    public async Task<ActionResult<GameInstance>> PostGameEnd(int gameInstanceId)
    {
        if (_context.GameInstance == null || _context.Leaderboard == null)
        {
            return Problem("Entity set 'Game2048Context.GameInstance' or 'Game2048Context.Leaderboard' is null.");
        }

        var gameInstance = this.GetGameInstance(gameInstanceId).Result.Value;

        if (gameInstance == null)
        {
            return BadRequest("Unable to find the game");
        }

        gameInstance.GameActive = false;

        _context.Leaderboard.Add(new Leaderboard
        {
            GameInstanceId = gameInstance.GameInstanceId,
            UserId = gameInstance.UserId,
            Score = gameInstance.Score,
        });

        await _context.SaveChangesAsync();

        return Ok(gameInstance);
    }

    private MoveType getMoveType(string moveType)
    {
        switch (moveType.ToLower())
        {
            case "up":
                return MoveType.Up;
            case "down":
                return MoveType.Down;
            case "left":
                return MoveType.Left;
            case "right":
                return MoveType.Right;
            default:
                throw new Exception("Invalid move type");
        }
    }

    // DELETE: api/game/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGameInstance(int id)
    {
        if (_context.GameInstance == null)
        {
            return NotFound();
        }
        var gameInstance = await _context.GameInstance.FindAsync(id);
        if (gameInstance == null)
        {
            return NotFound();
        }

        _context.GameInstance.Remove(gameInstance);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool GameInstanceExists(int id)
    {
        return (_context.GameInstance?.Any(e => e.GameInstanceId == id)).GetValueOrDefault();
    }
}
