using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;

namespace Consumer.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ConsumerController : ControllerBase
    {
        private static ConcurrentStack<DrinkModel> drinkStack = new ConcurrentStack<DrinkModel>();

        private const string ProducerUri = "https://localhost:7015/api/Producer/Consume";
        private static bool StartConsumtion = false;
        private readonly HttpClient _httpClient;
        public ConsumerController(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient();
        }

        [HttpPost]
        public void Consume(DrinkModel model)
        {
            Thread.Sleep(500);//Simulate consume process
            Console.WriteLine($"Consume drink: {model.Drink}");
            drinkStack.Push(model);
            if (!StartConsumtion)
            {
                StartConsumtion = true;
                Thread thr1 = new Thread(async () => await SendHttpRequestAsync(1));
                Thread thr2 = new Thread(async () => await SendHttpRequestAsync(2));
                Thread thr3 = new Thread(async () => await SendHttpRequestAsync(3));/*
                Thread thr4 = new Thread(async () => await SendHttpRequestAsync(4));
                Thread thr5 = new Thread(async () => await SendHttpRequestAsync(5));*/

                thr1.Start();
                Thread.Sleep(500);
                thr2.Start();
                Thread.Sleep(500);
                thr3.Start();/*
                Thread.Sleep(500);
                thr4.Start();
                Thread.Sleep(500);
                thr5.Start();*/
            }
            
        }

        private async Task SendHttpRequestAsync(int nr)
        {
            var rnd = new Random();
            while (true)
            {
                Thread.Sleep(500);
                var drinkModel = new DrinkModel();
                if (drinkStack.TryPop(out drinkModel))
                {
                    var drink = "{" + "\"drink\":\"" + drinkModel.Drink +"\"}";
                    var message = new HttpRequestMessage();
                    message.Method = HttpMethod.Post;
                    message.RequestUri = new Uri(ProducerUri);
                    message.Content = new StringContent(drink, Encoding.UTF8, "application/json");
                    Console.WriteLine($"Send message from thread: {nr}");
                    await _httpClient.SendAsync(message);
                }
            }
        }
    }
}