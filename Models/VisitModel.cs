using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace przychodnia.Models
{
    [Index(nameof(ScheduleID), nameof(StartTime), IsUnique = true)]
    public class VisitModel
    {
        [Key]
        public int VisitID { get; set; }

        public int ScheduleID { get; set; }
        public virtual ScheduleModel Schedule { get; set; }
        public int? PatientID { get; set; }
        public virtual PatientModel Patient { get; set; }


        [Required]
        public DateTime StartTime { get; set; }

        public bool Available { get; set; } = true;

        [StringLength(500)]
        public string Description { get; set; } = "";

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public string getDate()
        {
            return StartTime.ToString("dd-MM-yyyy");
        }
        public string getDayOfTheWeek()
        {
            return StartTime.DayOfWeek.ToString();
        }
        public string getStartHour()
        {
            return StartTime.ToString("H:mm");
        }
    }
}
