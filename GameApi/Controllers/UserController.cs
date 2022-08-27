using GameApi.Model;
using GameApi.Dto;
using GameApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly SqlDbContext _context;

    public UsersController(SqlDbContext context)
    {
        _context = context;
    }

    // GET: api/Users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameUser>>> GetUser()
    {
        if (_context.GameUser == null)
        {
            return NotFound();
        }
        return await _context.GameUser.ToListAsync();
    }

    // GET: api/Users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GameUser>> GetUser(int id)
    {
        if (_context.GameUser == null)
        {
            return NotFound();
        }
        var user = await _context.GameUser.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    // PUT: api/Users/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(int id, GameUser user)
    {
        if (id != user.UserId)
        {
            return BadRequest();
        }

        _context.Entry(user).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Users
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<GameUser>> PostUser(UserDTO userDTO)
    {
        if (_context.GameUser == null)
        {
            return Problem("Entity set 'Game2048Context.User'  is null.");
        }

        var user = new GameUser
        {
            Username = userDTO.Username,
            Email = userDTO.Email,
            Password = userDTO.Password
        };

        _context.GameUser.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetUser", new { id = user.UserId }, userDTO);
    }

    // DELETE: api/Users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        if (_context.GameUser == null)
        {
            return NotFound();
        }
        var user = await _context.GameUser.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _context.GameUser.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool UserExists(int id)
    {
        return (_context.GameUser?.Any(e => e.UserId == id)).GetValueOrDefault();
    }
}