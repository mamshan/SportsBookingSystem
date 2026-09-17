using System.ComponentModel.DataAnnotations;

namespace SportsBookingSystem.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Please enter your name.")]
        [StringLength(255)]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(255)]
        public string Email { get; set; } = "";

        [Required, StringLength(255)]
        public string Phone { get; set; } = "";

        [Required, StringLength(255)]
        public string Address { get; set; } = "";

        [Required(ErrorMessage = "Please enter a password.")]
        [DataType(DataType.Password)]
        [StringLength(255, MinimumLength = 6)]
        public string Password { get; set; } = "";

        public List<int> SelectedSports { get; set; } = new List<int>();
    }
}
