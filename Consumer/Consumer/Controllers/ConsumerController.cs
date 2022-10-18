using Microsoft.AspNetCore.Mvc;

namespace Consumer.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ConsumerController : ControllerBase
    {

        public ConsumerController()
        {
        }

        [HttpPost]
        public void Consume(DrinkModel model)
        {
            Console.WriteLine($"Consume drink: {model.Drink}");
            return;
        }
    }
}