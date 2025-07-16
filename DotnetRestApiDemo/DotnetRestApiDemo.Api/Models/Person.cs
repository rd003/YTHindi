using System.ComponentModel.DataAnnotations;

namespace DotnetRestApiDemo.Api.Models;

public class Person
{
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public int Age { get; set; }
}
