using System.ComponentModel.DataAnnotations;

namespace SportsBookingSystem.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Please enter your name.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public List<int> SelectedSports { get; set; } = new List<int>();
    }
}
