using System.ComponentModel.DataAnnotations;

namespace MedicalManagementSystem.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        [Required(ErrorMessage = "Customer name is required")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "Customer mobile is required")]
        [MinLength(10, ErrorMessage = "Mobile number should be 10 digits exactly")]
        [MaxLength(10, ErrorMessage = "Mobile number should be 10 digits exactly")]
        public string CustomerMobile { get; set; }
        [Required(ErrorMessage = "Customer address is required")]
        public string CustomerAddress { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
