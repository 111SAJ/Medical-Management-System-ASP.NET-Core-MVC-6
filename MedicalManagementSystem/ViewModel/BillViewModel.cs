
namespace MedicalManagementSystem.ViewModel
{
    public class BillViewModel
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime Date { get; set; }
        public string CustomerName { get; set; }
        public string MRName { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
