using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortal.Models
{
    public class Department
    {
        [Key]
        [StringLength(50, ErrorMessage = "Department Code cannot exceed 50 characters.")]
        public string? DepartmentCode { get; set; }

        [Required]
        public string? DepartmentName { get; set; }

        [Required]
        [ForeignKey(nameof(DepartmentHead))]
        public int? DepartmentHeadId { get; set; }
        public Teacher? DepartmentHead { get; set; }

        [NotMapped]
        public string? DepartmentHeadName => DepartmentHead?.User != null
            ? $"{DepartmentHead.User.Name} {DepartmentHead.User.Surname}"
            : null;

        [NotMapped]
        public string? Email => DepartmentHead?.User?.Email;

        [Required]
        [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string? Phone { get; set; }
    }
}
