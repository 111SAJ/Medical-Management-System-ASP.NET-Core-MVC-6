using MedicalManagementSystem.Data;
using MedicalManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalManagementSystem.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 5; // Number of items per page

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Product List with Pagination
        [HttpGet]
        public IActionResult Index(string searchString, int page = 1)
        {
            // Query to filter customers based on search string
            var productQuery = _context.Product.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                DateTime searchDate; //Dates are typically not stored in a human-readable format in the database
                bool isDate = DateTime.TryParse(searchString, out searchDate);

                productQuery = productQuery.Where(c =>
                    c.ProductId.ToString().Contains(searchString) ||
                    c.InvoiceNumber.Contains(searchString) ||
                    c.MRP.Contains(searchString) ||
                    c.BatchNumber.Contains(searchString) ||
                    c.ItemName.Contains(searchString) ||
                    (isDate && c.CreatedOn.Date == searchDate.Date) ||
                    (isDate && c.Expiry.Date == searchDate.Date)
                );
            }

            // Pagination
            var productList = productQuery
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // Calculate total number of pages
            var totalCustomers = productQuery.Count();
            var totalPages = (int)Math.Ceiling(totalCustomers / (double)PageSize);

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = PageSize;
            ViewBag.SearchString = searchString;

            return View(productList);
        }

        //Create Product
        [HttpGet]
        public IActionResult Create()
        {
            var newProduct = new Product { GST = "18" };
            return View(newProduct);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                // Check if the InvoiceNumber exists in the Bill table
                var existbill = await _context.Bill.FirstOrDefaultAsync(b => b.InvoiceNumber == product.InvoiceNumber);

                if (existbill != null)
                {
                    product.CalculateSubTotal(); // Calculate SubTotal before saving

                    _context.Product.Add(product);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("InvoiceNumber", "Invoice Number does not match any record.");
                    return View(product);
                }
            }
            return View(product);
        }

        //Edit Product
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var existProduct = _context.Product.Find(id);

            if (existProduct == null)
            {
                return NotFound();
            }

            return View(existProduct);

        }

        [HttpPost]
        public IActionResult Edit(int id, Product product)
        {
            if (ModelState.IsValid)
            {
                // Check if the InvoiceNumber exists in the Bill table
                var existbill = _context.Bill.FirstOrDefault(b => b.InvoiceNumber == product.InvoiceNumber);

                if (existbill != null)
                {
                    product.CalculateSubTotal(); // Calculate SubTotal before saving

                    _context.Update(product);
                    _context.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("InvoiceNumber", "Invoice Number does not match any record.");
                    return View(product);
                }
            }

            return View(product);
        }

        //Remove Product
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var existProduct = _context.Product.Find(id);

            if (existProduct == null)
            {
                return NotFound();
            }

            return View(existProduct);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var existProduct = _context.Product.Find(id);

            if (existProduct != null)
            {
                _context.Product.Remove(existProduct);
                _context.SaveChanges();

            }
            return RedirectToAction("Index");
        }
    }
}
