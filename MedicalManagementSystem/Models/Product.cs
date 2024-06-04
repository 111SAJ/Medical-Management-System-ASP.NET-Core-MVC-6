using System.ComponentModel.DataAnnotations;

namespace MedicalManagementSystem.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Invoice Number is required")]
        public string InvoiceNumber { get; set; }
        [Required(ErrorMessage = "Item Name is required")]
        public string ItemName { get; set; }
        [Required(ErrorMessage = "Pack is required")]
        public string Pack { get; set; }
        public string BatchNumber { get; set; }
        [Required(ErrorMessage = "Expiry is required")]
        public DateTime Expiry { get; set; }
        [Required(ErrorMessage = "MRP is required")]
        public string MRP { get; set; }
        [Required(ErrorMessage = "Quantity is required")]
        public string QTY { get; set; }
        [Required(ErrorMessage = "Rate is required")]
        public string Rate { get; set; }
        [Required(ErrorMessage = "Discount is required")]
        public string DS { get; set; }
        public string GST { get; set; }
        public string? SubTotal { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public void CalculateSubTotal()
        {
            if (decimal.TryParse(Rate, out decimal rate) &&
                decimal.TryParse(QTY, out decimal qty) &&
                decimal.TryParse(DS, out decimal discount))
            {
                decimal total = rate * qty;
                total -= discount;
                total += total * 0.18m; // Adding 18% GST
                SubTotal = total.ToString("0.00");
            }
            else
            {
                SubTotal = "0.00";
            }
        }

    }
}
