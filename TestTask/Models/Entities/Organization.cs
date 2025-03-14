using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TestTask.Models.Entities
{
    [Index(nameof(INN), IsUnique = true)]
    public class Organization
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }

        [Length(10, 10)]
        public required string INN { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
