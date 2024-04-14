using AISummariseApplication.Models;
using AISummariseApplication.Services;
using AISummariseApplication.Services.DatabaseClasses;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Diagnostics;

namespace AISummariseApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MongoDBService _mongoDBService;

        public HomeController(ILogger<HomeController> logger, MongoDBService mongoDB)
        {
            _logger = logger;
            _mongoDBService = mongoDB;
        }

        public async Task<IActionResult> Index()
        {
            /*
            var Database = _mongoDBService.GetDatabase("sample_mflix");
            var Collection = Database.GetCollection<Comment>("comments");
            var Comment = await Collection.Find(x => x.Email.ToString() == "mercedes_tyler@fakegmail.com").FirstOrDefaultAsync();
            _logger.LogInformation(Comment.Name.ToString());
            */

            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


         
    }
}
