using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace przychodnia.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class PatientModel
    {
        [Key]
        public int PatientID { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }
        
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Surname { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [StringLength(30, MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsActive { get; set; } = false;

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public string getBirthDate()
        {
            return this.DateOfBirth.ToString("dd-MM-yyyy");
        }

    }

}
