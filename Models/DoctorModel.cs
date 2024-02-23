using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace przychodnia.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class DoctorModel
    {
        [Key]
        public int DoctorID { get; set; }
        
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
        public bool IsDirector { get; set; } = false;
        public int SpecialityID { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual SpecialityModel Speciality { get; set; }

        public virtual ICollection<ScheduleModel> Schedules { get; set; }

        public string getBirthDate()
        {
            return this.DateOfBirth.ToString("dd-MM-yyyy");
        }
        public DateTime getLastMonday()
        {
            var currentDate = DateTime.Now.Date;
            while ( currentDate.DayOfWeek.ToString() != "Monday")
            {
                currentDate = currentDate.AddDays(-1);
            }
            return currentDate;
        }
    }
}
