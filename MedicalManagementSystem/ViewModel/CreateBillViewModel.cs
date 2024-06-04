using System.ComponentModel.DataAnnotations;

namespace MedicalManagementSystem.ViewModel
{
    public class CreateBillViewModel
    {
        [Required(ErrorMessage = "Invoice Number is required")]
        public string InvoiceNumber { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Customer is required")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "MR Name is required")]
        public string MRName { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
