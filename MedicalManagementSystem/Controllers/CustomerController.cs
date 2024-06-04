using MedicalManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using MedicalManagementSystem.Models;

namespace MedicalManagementSystem.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 5; // Number of items per page

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Customer List with Pagination
        [HttpGet]
        public IActionResult Index(string searchString, int page = 1)
        {
            // Query to filter customers based on search string
            var customerQuery = _context.Customer.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                DateTime searchDate; //Dates are typically not stored in a human-readable format in the database
                bool isDate = DateTime.TryParse(searchString, out searchDate);

                customerQuery = customerQuery.Where(c =>
                    c.CustomerId.ToString().Contains(searchString) ||
                    c.CustomerName.Contains(searchString) ||
                    c.CustomerMobile.Contains(searchString) ||
                    (isDate && c.CreatedOn.Date == searchDate.Date) ||
                    c.CustomerAddress.ToString().Contains(searchString)
                );
            }

            // Pagination
            var customerList = customerQuery
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // Calculate total number of pages
            var totalCustomers = customerQuery.Count();
            var totalPages = (int)Math.Ceiling(totalCustomers / (double)PageSize);

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = PageSize;
            ViewBag.SearchString = searchString;

            return View(customerList);
        }



        //Customer Create
        [HttpGet]
        public IActionResult Create()
        {
            var newCustomer = new Customer();
            return View(newCustomer);
        }

        [HttpPost]
        public IActionResult Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                var existCustomer = _context.Customer.FirstOrDefault(c => c.CustomerName == customer.CustomerName);
                if (existCustomer != null)
                {
                    ModelState.AddModelError("CustomerName", "Customer already exist");
                    return View(customer);
                }

                _context.Customer.Add(customer);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(customer);
        }

        // Customer Edit
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var existCustomer = _context.Customer.Find(id);

            if (existCustomer == null)
            {
                return NotFound();
            }

            return View(existCustomer);

        }

        [HttpPost]
        public IActionResult Edit(int id, Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Update(customer);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(customer);
        }

        // Customer Delete
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var existCustomer = _context.Customer.Find(id);

            if (existCustomer == null)
            {
                return NotFound();
            }

            return View(existCustomer);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var existCustomer = _context.Customer.Find(id);

            if (existCustomer != null)
            {
                _context.Customer.Remove(existCustomer);
                _context.SaveChanges();

            }
            return RedirectToAction("Index");
        }

    }
}
