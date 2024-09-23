using System.ComponentModel.DataAnnotations;

namespace Seat_Reservation_System.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        public string? TraineeName { get; set; }
        public string? TraineeNIC { get; set; }
        public int LeaveCount { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        //public virtual ICollection<Inquiry> Inquiries { get; set; }
    }


    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string? TraineeName { get; set; }

        [Required(ErrorMessage = "NIC number is required")]
        public string? TraineeNIC { get; set; }

    }

    public class AdminLoginViewModel
    {
       
        [Required(ErrorMessage = "Username is required")]
        public string? AdminUsername { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string? AdminPassword { get; set; }
    }

  

}
