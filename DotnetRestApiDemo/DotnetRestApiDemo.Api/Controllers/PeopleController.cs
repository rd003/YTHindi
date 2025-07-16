using DotnetRestApiDemo.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotnetRestApiDemo.Api.Controllers
{
    [Route("api/[controller]")] // /api/people
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PeopleController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddPerson(Person person)
        {
            try
            {
                _context.People.Add(person);
                await _context.SaveChangesAsync();
                return CreatedAtRoute("GetPerson", new { id = person.Id }, person); // 201 Created + location: route of get endpoint +  person object (body)
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet] // api/people (get)
        public async Task<IActionResult> GetPeople()
        {
            try
            {
                var people = await _context.People.ToListAsync(); // select Id,FirstName,LastName,Age from People;
                return Ok(people); // 200 + people array
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id:int}", Name = "GetPerson")]  // api/people/1 (get)
        public async Task<IActionResult> GetPerson(int id)
        {
            try
            {
                var person = await _context.People.FindAsync(id);

                if (person is null)
                {
                    return NotFound($"Person with id:{id} not found"); // 404 Not Found + string message
                }

                return Ok(person); // 200 + person object
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{id:int}")]  // api/people/1 (PUT) 
        public async Task<IActionResult> UpdatePerson(int id, [FromBody] Person person)
        {
            try
            {
                if (id != person.Id)
                {
                    return BadRequest("Ids mismatch"); // 400 Bad Request + message
                }

                // If person is exists or not

                if (!await _context.People.AnyAsync(a => a.Id == id))
                {
                    return NotFound($"Person with id:{id} not found"); // 404 Not Found + string message
                }

                _context.People.Update(person);
                await _context.SaveChangesAsync();
                return NoContent();  // 204 No Content + Nothing in body
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{id:int}")]  // api/people/1 (DELETE) 
        public async Task<IActionResult> DeletePerson(int id)
        {
            try
            {
                var person = await _context.People.FindAsync(id);

                // If person is exists or not
                if (person is null)
                {
                    return NotFound($"Person with id:{id} not found"); // 404 Not Found + string message
                }

                _context.People.Remove(person);
                await _context.SaveChangesAsync();
                return NoContent();  // 204 No Content + Nothing in body
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
