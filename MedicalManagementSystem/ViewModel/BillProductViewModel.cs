using MedicalManagementSystem.Models;

namespace MedicalManagementSystem.ViewModel
{
    public class BillProductViewModel
    {
        public Bill Bill { get; set; }
        public List<Product> Product { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
