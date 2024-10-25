using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Ensure this is included
using ChurnPredictionProject.Data; // Adjust to your actual namespace
using ChurnPredictionProject.Models.Service; // Adjust to your actual namespace
using ChurnPredictionProject.Models; // Include your models namespace
using System.Threading.Tasks;
using System.Linq;
using ChurnPredictionProject.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace ChurnPredictionProject.Controllers
{
    public class ChurnController : Controller
    {
        private readonly ILogger<ChurnController> _logger;
        private readonly MyDbContext _db;
        private readonly ChurnPredictionService _predictionService;

        public ChurnController(MyDbContext db, ILogger<ChurnController> logger, ChurnPredictionService predictionService)
        {
            _db = db;
            _logger = logger;
            _predictionService = predictionService;
        }

        // Default action for the Churn page
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View(new Customer());
        }
        public IActionResult Results()
        {
            return View(); // Pass the forecast results to the view
        }
        // GET: Import customer data
        public IActionResult Import()
        {
            return View();
        }
        // POST: Import customer data
        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.ImportResult = "Please upload a valid file.";
                return View();
            }

            var customers = new List<Customer>();

            using (var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
            {
                // Skip header line if your CSV has headers
                await reader.ReadLineAsync();

                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    var values = line.Split(',');

                    // Ensure there are enough values in the line
                    if (values.Length < 5) continue; // Skip if not enough columns

                    // Try parsing each value and handle any format exceptions
                    try
                    {
                        var customer = new Customer
                        {
                            Name = values[0].Trim(),  // Assuming the first column is Name
                            Age = int.Parse(values[1].Trim(), CultureInfo.InvariantCulture), // Age in the second column
                            Gender = values[2].Trim(), // Gender in the third column
                            MonthlySpending = double.Parse(values[3].Trim(), CultureInfo.InvariantCulture), // Monthly Spending
                            TenureMonths = int.Parse(values[4].Trim(), CultureInfo.InvariantCulture) // Tenure in the fifth column
                        };

                        customers.Add(customer);
                    }
                    catch (FormatException ex)
                    {
                        // Log the error (if you have logging set up)
                        Console.WriteLine($"Error parsing line: {line}. Exception: {ex.Message}");
                        // Optionally, provide feedback for the user
                        ViewBag.ImportResult += $"Error parsing line: {line}. Check the format. ";
                    }
                    catch (Exception ex)
                    {
                        // Handle any other exceptions as needed
                        Console.WriteLine($"Unexpected error: {ex.Message}");
                    }
                }
            }

            // Add all customers to the database
            if (customers.Count > 0)
            {
                await _db.Customers.AddRangeAsync(customers);
                await _db.SaveChangesAsync();
                ViewBag.ImportResult += $"{customers.Count} customers imported successfully.";
            }
            else
            {
                ViewBag.ImportResult = "No valid customers were imported.";
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PredictChurn(Customer customer)
        {
            if (ModelState.IsValid)
            {
                // Check if the customer already exists in the database
                if (customer.CustomerId == 0)
                {
                    // Insert new customer and retrieve the ID
                    _db.Customers.Add(customer);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    // Check if the customer actually exists in the database
                    var existingCustomer = await _db.Customers.FindAsync(customer.CustomerId);
                    if (existingCustomer == null)
                    {
                        ModelState.AddModelError("", "Customer not found.");
                        return View("Create", customer); // Show input form again if invalid
                    }
                }

                // Perform churn prediction
                var churnProbability = await _predictionService.GetChurnPredictionAsync(customer);

                // Create a new ChurnPrediction entry
                var prediction = new ChurnPrediction
                {
                    CustomerId = customer.CustomerId,
                    ChurnProbability = churnProbability,
                    PredictionDate = DateTime.Now
                };

                _db.ChurnPredictions.Add(prediction);
                await _db.SaveChangesAsync();

                // Set the prediction result for display
                ViewBag.ChurnProbability = churnProbability;
                return View("Results", customer); // Pass customer model to results view
            }

            return View("Create", customer); // Show input form again if invalid
        }
    }
}

