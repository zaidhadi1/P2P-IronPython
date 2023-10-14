using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using WebGui.Models;

namespace WebGui.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "Home";
            return View();
        }

        [HttpGet]
        public IActionResult GetJobs()
        {
            RestClient APIClient = new RestClient("http://localhost:65228/");
            RestRequest request = new RestRequest("api/Clients");
            RestResponse listOfClients = APIClient.Get(request);
            List<Client> loClient = JsonConvert.DeserializeObject<List<Client>>(listOfClients.Content);

            if (loClient != null)
            {
                if (loClient.Count > 0)
                {
                    return Ok(loClient);
                }
                else
                {
                    return BadRequest();
                }

            }
            else
            {
                return BadRequest();
            }
        }
    }
}
