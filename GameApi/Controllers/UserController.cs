using FluentValidation.Results;
using GameApi.Dto;
using GameApi.Infrastructure;
using GameApi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GameApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly SqlDbContext _context;
    private readonly IMemoryCache _memoryCache;
    public UsersController(IMemoryCache memoryCache, SqlDbContext context)
    {
        _memoryCache = memoryCache;
        _context = context;
    }

    // GET: api/Users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUser()
    {
        var cacheKey = "userList";
        //checks if cache entries exists
        if (!_memoryCache.TryGetValue(cacheKey, out List<User> userList))
        {
            //calling the server
            userList = await _context.User.ToListAsync();

            //setting up cache options
            var cacheExpiryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(50),
                Priority = CacheItemPriority.High,
                SlidingExpiration = TimeSpan.FromSeconds(20)
            };
            //setting cache entries
            _memoryCache.Set(cacheKey, userList, cacheExpiryOptions);
        }
        return Ok(userList);

        //if (_context.User == null)
        //{
        //    return NotFound();
        //}
        //return await _context.User.ToListAsync();
    }

    // GET: api/Users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var cacheKey = "user";
        //checks if cache entries exists
        if (!_memoryCache.TryGetValue(cacheKey, out User user))
        {
            //calling the server
            user = await _context.User.FindAsync(id);

            //setting up cache options
            var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(3));

            //setting cache entries
            _memoryCache.Set(cacheKey, user, cacheEntryOptions);
        }
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);

        //if (_context.User == null)
        //{
        //    return NotFound();
        //}
        //var user = await _context.User.FindAsync(id);

        //if (user == null)
        //{
        //    return NotFound();
        //}

        //return user;
    }

    // PUT: api/Users/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(int id, User user)
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
    public async Task<ActionResult<User>> PostUser(UserDTO userDTO)
    {
        if (_context.User == null)
        {
            return Problem("Entity set 'Game2048Context.User' is null.");
        }

        var user = new User
        {
            Username = userDTO.Username,
            Email = userDTO.Email,
            Password = userDTO.Password
        };

        UserValidator validator = new UserValidator();
        ValidationResult results = validator.Validate(user);

        if (!results.IsValid)
        {
            foreach (var failure in results.Errors)
            {
                Console.WriteLine("Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage);
            }
            return BadRequest();
        }

        _context.User.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetUser", new { id = user.UserId }, userDTO);
    }

    // DELETE: api/Users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        if (_context.User == null)
        {
            return NotFound();
        }
        var user = await _context.User.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _context.User.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool UserExists(int id)
    {
        return (_context.User?.Any(e => e.UserId == id)).GetValueOrDefault();
    }
}