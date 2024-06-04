using MedicalManagementSystem.Data;
using MedicalManagementSystem.Models;
using MedicalManagementSystem.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MedicalManagementSystem.Controllers
{
    public class BillController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 5; // Number of items per page
        public BillController(ApplicationDbContext context)
        {
            _context = context;
        }

        // List Bill with Search and Pagination
        [HttpGet]
        public IActionResult Index(string searchString, int page = 1)
        {
            // Query to filter bills based on search string
            var billQuery = _context.Bill.Include(b => b.Customer).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                DateTime searchDate;
                bool isDate = DateTime.TryParse(searchString, out searchDate);

                billQuery = billQuery.Where(b =>
                    b.InvoiceNumber.Contains(searchString) ||
                    b.Customer.CustomerName.Contains(searchString) ||
                    b.MRName.Contains(searchString) ||
                    (isDate && b.Date.Date == searchDate.Date) ||
                    (isDate && b.DueDate.Date == searchDate.Date) ||
                    (isDate && b.CreatedOn.Date == searchDate.Date)
                );
            }

            // Pagination
            var billList = billQuery
                .OrderByDescending(b => b.CreatedOn)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(b => new BillViewModel
                {
                    InvoiceId = b.InvoiceId,
                    InvoiceNumber = b.InvoiceNumber,
                    Date = b.Date,
                    CustomerName = b.Customer.CustomerName,
                    MRName = b.MRName,
                    DueDate = b.DueDate,
                    CreatedOn = b.CreatedOn
                })
                .ToList();

            // Calculate total number of pages
            var totalBills = billQuery.Count();
            var totalPages = (int)Math.Ceiling(totalBills / (double)PageSize);

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = PageSize;
            ViewBag.SearchString = searchString;

            return View(billList);
        }

        //Create Bill
        [HttpGet]
        public IActionResult Create()
        {
            var newBill = new Bill
            {
                InvoiceNumber = GenerateUniqueInvoiceNumber(),
            };

            ViewBag.Customers = new SelectList(_context.Customer.ToList(), "CustomerId", "CustomerName");
            return View(newBill);
        }

        [HttpPost]
        public IActionResult Create(CreateBillViewModel createBillViewModel)
        {
            if (ModelState.IsValid)
            {
                // Check if the provided CustomerId exists in the Customer table
                var existCustomer = _context.Customer.Any(c => c.CustomerId == createBillViewModel.CustomerId);
                if (!existCustomer)
                {
                    ModelState.AddModelError("CustomerId", "Customer does not exist.");
                    return View(createBillViewModel);
                }

                // Generate a unique 6-digit InvoiceNumber (if not provided, just a safeguard)
                if (string.IsNullOrEmpty(createBillViewModel.InvoiceNumber))
                {
                    createBillViewModel.InvoiceNumber = GenerateUniqueInvoiceNumber();
                }

                // Create a new Bill object
                var bill = new Bill
                {
                    InvoiceNumber = createBillViewModel.InvoiceNumber,
                    Date = createBillViewModel.Date,
                    CustomerId = createBillViewModel.CustomerId,
                    MRName = createBillViewModel.MRName,
                    DueDate = createBillViewModel.DueDate,
                    CreatedOn = createBillViewModel.CreatedOn
                };

                _context.Bill.Add(bill);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.Customers = new SelectList(_context.Customer.ToList(), "CustomerId", "CustomerName");
            return View(createBillViewModel);

        }

        //Edit Bill
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var existBill = _context.Bill.Find(id);

            if (existBill == null)
            {
                return NotFound();
            }

            ViewBag.Customers = new SelectList(_context.Customer.ToList(), "CustomerId", "CustomerName");
            return View(existBill);

        }

        [HttpPost]
        public IActionResult Edit(int id, CreateBillViewModel createBillViewModel) //Using CreateBillViewModel for update also as the fields as same as CreateBillViewModel
        {
            if (ModelState.IsValid)
            {
                var existBill = _context.Bill.Find(id);
                if (existBill == null)
                {
                    return NotFound();
                }

                //Update the existing Bill
                existBill.Date = createBillViewModel.Date;
                existBill.CustomerId = createBillViewModel.CustomerId;
                existBill.MRName = createBillViewModel.MRName;
                existBill.DueDate = createBillViewModel.DueDate;
                existBill.CreatedOn = createBillViewModel.CreatedOn;

                _context.Bill.Update(existBill);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.Customers = new SelectList(_context.Customer.ToList(), "CustomerId", "CustomerName");
            return View(createBillViewModel);
        }

        // Bill Delete
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var existBill = _context.Bill.Find(id);

            if (existBill == null)
            {
                return NotFound();
            }

            return View(existBill);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var existBill = _context.Bill.Find(id);

            if (existBill != null)
            {
                _context.Bill.Remove(existBill);
                _context.SaveChanges();

            }
            return RedirectToAction("Index");
        }

        //Preview Bill with Items
        [HttpGet]
        public IActionResult Preview(int id)
        {
            var existBill = _context.Bill.Include(b => b.Customer).FirstOrDefault(b => b.InvoiceId == id);

            if (existBill == null)
            {
                return NotFound();
            }

            var product = _context.Product.Where(p => p.InvoiceNumber == existBill.InvoiceNumber).ToList();
            decimal totalAmount = product.Sum(p => decimal.TryParse(p.SubTotal, out var subTotal) ? subTotal : 0);

            var viewModel = new BillProductViewModel
            {
                Bill = existBill,
                Product = product,
                TotalAmount = totalAmount
            };

            return View(viewModel);
        }


        // Function to generate unique InvoiceNumber
        private string GenerateUniqueInvoiceNumber()
        {
            string invoiceNumber = new Random().Next(100000, 999999).ToString();

            var existInvoiceNumber = _context.Bill.Any(b => b.InvoiceNumber == invoiceNumber);
            if (existInvoiceNumber)
            {
                return GenerateUniqueInvoiceNumber();
            }

            return invoiceNumber;
        }
    }
}
