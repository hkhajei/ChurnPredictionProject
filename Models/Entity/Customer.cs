using System.ComponentModel.DataAnnotations;

namespace ChurnPredictionProject.Models.Entity
{
    public class Customer
    {
        [Key]
        public long CustomerId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public double MonthlySpending { get; set; }
        public int TenureMonths { get; set; }
        public bool IsChurned { get; set; } 
    }
}
