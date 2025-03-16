using System.ComponentModel.DataAnnotations;

namespace TestTask.Models
{
    public class OrganizationViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Имя обязательно")]
        public required string Name { get; set; }
        [Required(ErrorMessage = "ИНН обязателен")]
        [StringLength(10, ErrorMessage = "ИНН должен содержать 10 цифр", MinimumLength = 10)]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "ИНН должен содержать только цифры")]
        public required string INN { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool IsSelected { get; set; }
    }
}
