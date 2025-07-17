using Dapper;
using DotnetApiDapper.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DotnetApiDapper.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PeopleController : ControllerBase
{
    private readonly string _connectionString;
    private readonly IConfiguration _configuration;

    public PeopleController(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("Default");
    }

    [HttpGet] // api/controller (GET)
    public async Task<IActionResult> GetPeople()
    {
        try
        {
            using IDbConnection connection = new SqlConnection(_connectionString);
            string sql = @"select 
                           Id,
                           FirstName,
                           LastName
                           from Person";
            var people = await connection.QueryAsync<Person>(sql);
            return Ok(people);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,ex.Message); // 500 internal server error + ex.message
        }
    }


    [HttpGet("{id:int}",Name ="GetPerson")] // api/controller/1 (GET)
    public async Task<IActionResult> GetPeopleById(int id)
    {
        try
        {
            using IDbConnection connection = new SqlConnection(_connectionString);
            string sql = @"select 
                           Id,
                           FirstName,
                           LastName
                           from Person
                           where Id = @id";
            Person? person = await connection.QueryFirstOrDefaultAsync<Person>(sql, new {id});
            if(person is null )
            {
                return NotFound($"person with id : {id} not found");
            }
            return Ok(person);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); // 500 internal server error + ex.message
        }
    }


    [HttpPost] // api/controller (POST)
    public async Task<IActionResult> CreatePerson(Person person)
    {
        try
        {
            using IDbConnection connection = new SqlConnection(_connectionString);
            string sql = @"insert into Person (FirstName,LastName)
                          values (@FirstName,@LastName);
                          
                          select scope_identity()";
            int createId= await connection.ExecuteScalarAsync<int>(sql, person);

            person.Id = createId;

            return CreatedAtRoute("GetPerson", new { id = person.Id},person); // 201 Created + location: 'https://localhost:5001/api/people/get' + object
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); // 500 internal server error + ex.message
        }
    }

    [HttpPut("{id:int}")] // api/people/1 (PUT)
    public async Task<IActionResult> UpdatePerson(int id,[FromBody]Person person)
    {
        try
        {
            if (id != person.Id)
            {
                return BadRequest("Id from querystring and body does not match"); //400
            }
            using IDbConnection connection = new SqlConnection(_connectionString);

            string existingPersonSql = @"select Id from Person where Id = @id";

            int? existringPersonId = await connection.QuerySingleOrDefaultAsync<int?>(existingPersonSql, new { id });

            if (existringPersonId is null)
            {
                return NotFound($"person with id : {id} not found");
            }

            string sql = @"update Person 
                            set FirstName=@FirstName,
                            LastName=@LastName
                            where Id=@Id
                            ";
            await connection.ExecuteAsync(sql, person);
            return NoContent(); //204 No Content
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); // 500 internal server error + ex.message
        }
    }


    [HttpDelete("{id:int}")] // api/people/1 (DELETE)
    public async Task<IActionResult> DeletePerson(int id)
    {
        try
        {
            using IDbConnection connection = new SqlConnection(_connectionString);

            string existingPersonSql = @"select Id from Person where Id = @id";

            int? existringPersonId = await connection.QuerySingleOrDefaultAsync<int?>(existingPersonSql, new { id });

            if (existringPersonId is null)
            {
                return NotFound($"person with id : {id} not found");
            }

            string sql = @"delete from Person where Id=@id";
            await connection.ExecuteAsync(sql, new { id});
            return NoContent(); //204 No Content
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); // 500 internal server error + ex.message
        }
    }
}
