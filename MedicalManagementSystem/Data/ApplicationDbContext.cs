using MedicalManagementSystem.Models;
using MedicalManagementSystem.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace MedicalManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        //Model
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Bill> Bill { get; set; }
        public DbSet<Product> Product { get; set; }

        //ViewModel
        //public DbSet<BillViewModel> BillViewModel { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options) 
        {
        
        }
    }
}
