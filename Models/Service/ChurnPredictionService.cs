using Newtonsoft.Json;
using ChurnPredictionProject.Models.Entity;
using System.Net.Http;
using System.Text;

namespace ChurnPredictionProject.Models.Service
{
    public class ChurnPredictionService
    {
        private readonly HttpClient _httpClient;

        public ChurnPredictionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5000"); // Adjust if needed
        }

        public async Task<double> GetChurnPredictionAsync(Customer customer)
        {
            var client = new HttpClient();

            // Prepare the data for prediction
            var inputData = new
            {
                Age = customer.Age,
                Gender_Male = customer.Gender.Equals("Male") ? 1 : 0,
                MonthlySpending = customer.MonthlySpending,
                TenureMonths = customer.TenureMonths
            };

            var json = JsonConvert.SerializeObject(inputData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("http://127.0.0.1:5000/predict", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, {errorMessage}");
                throw new Exception($"API call failed with status code: {response.StatusCode}");
            }

            var result = await response.Content.ReadAsStringAsync();
            var resultData = JsonConvert.DeserializeObject<Dictionary<string, double>>(result);
            double churnProbability = resultData["churn_probability"];
            return churnProbability;
        }
    }

    public class PredictionResponse
    {
        public double ChurnProbability { get; set; }
    }
}
