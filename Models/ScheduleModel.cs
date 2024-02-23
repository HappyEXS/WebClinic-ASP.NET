using System.ComponentModel.DataAnnotations;

namespace przychodnia.Models
{
    public class ScheduleModel
    {
        [Key]
        public int ScheduleID { get; set; }
        public int DoctorID { get; set; }

        public virtual DoctorModel Doctor { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EndTime { get; set; } = DateTime.Now.AddHours(1);


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

        public string getEndHour()
        {
            return EndTime.ToString("H:mm");
        }
    }
}
