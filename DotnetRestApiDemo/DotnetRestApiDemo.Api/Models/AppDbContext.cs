using Microsoft.EntityFrameworkCore;

namespace DotnetRestApiDemo.Api.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {

    }

    public DbSet<Person> People { get; set; }
}
