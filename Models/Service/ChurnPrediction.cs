using ChurnPredictionProject.Models.Entity;
using System.ComponentModel.DataAnnotations;

namespace ChurnPredictionProject.Models.Service
{
    public class ChurnPrediction
    {
        [Key] 
        public long PredictionId { get; set; }
        public long CustomerId { get; set; }
        public double ChurnProbability { get; set; } // e.g., 0.85 for 85%
        public DateTime PredictionDate { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
