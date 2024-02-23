using System.ComponentModel.DataAnnotations;

namespace przychodnia.Models
{
    public class SpecialityModel
    {
        [Key]
        public int SpecialityID { get; set; }
        public string Name { get; set; }

        public virtual ICollection<DoctorModel> Doctors { get; set; }
    }
}
