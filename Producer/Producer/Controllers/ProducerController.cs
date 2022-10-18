using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Producer.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProducerController : ControllerBase
    {

        private readonly HttpClient _httpClient;
        private static readonly string[] CoffeeList = { "Arabica", "Robusta", "Latte", "Cappuccino", "Americano", "Espresso", "Doppio" }; 
        private static readonly string[] TeaList = { "Black", "Green", "Oolong", "White", "Pu-erh", "Earl", "Jasmine" }; 
        private static readonly string[] JuiceList = { "Apple", "Been", "Bluberry", "Grape", "Orange", "Carrot", "Jasmine" }; 
        private static readonly string[] BeerList = { "Ale", "Lager", "Pale", "Stout", "Pisner", "Lager", "Brown" }; 
        private static readonly string[] WineList = { "Red", "White", "Rose", "Sparkling", "Merlot", "Barbera", "Cabernet" }; 
        private const string ConsumerUri = "https://localhost:7096/api/consumer/consume";
        public ProducerController(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient();
        }

        [HttpPost]
        public async Task Start(CancellationToken ct)
        {
            Thread thr1 = new Thread(async () => await SendHttpRequestAsync(1, CoffeeList, ct));
            Thread thr2 = new Thread(async () => await SendHttpRequestAsync(2, TeaList, ct));
            Thread thr3 = new Thread(async () => await SendHttpRequestAsync(3, BeerList, ct));
            Thread thr4 = new Thread(async () => await SendHttpRequestAsync(4, JuiceList, ct));
            Thread thr5 = new Thread(async () => await SendHttpRequestAsync(5, WineList, ct));
            Thread thr6 = new Thread(async () => await SendHttpRequestAsync(6, CoffeeList, ct));

            thr1.Start();
            Thread.Sleep(500);
            thr2.Start();
            Thread.Sleep(500);
            thr3.Start();
            Thread.Sleep(500);
            thr4.Start();
            Thread.Sleep(500);
            thr5.Start();
            Thread.Sleep(500);
            thr6.Start();
        }

        private async Task SendHttpRequestAsync(int nr, string[] drinks, CancellationToken ct)
        {
            var rnd = new Random();
            while (true)
            {
                Thread.Sleep(1000);
                ct.ThrowIfCancellationRequested();
                var drink = "{" + "\"drink\":" + drinks[rnd.Next(0, CoffeeList.Length)]+"}";
                var message = new HttpRequestMessage();
                message.Method = HttpMethod.Post;
                message.RequestUri = new Uri(ConsumerUri);
                message.Content = new StringContent(drink, Encoding.UTF8, "application/json");
                Console.WriteLine($"Send message from thread: {nr}");
                await _httpClient.SendAsync(message, ct);
            }
        }
    }
}