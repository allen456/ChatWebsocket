using System.Linq;

using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

using ChatApp.Models;
using ChatApp.Data;

namespace ChatApp.Controllers
{
    public class ChatController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ChatContext _context;

        public ChatController(ChatContext context, IConfiguration config)
        {
            _config = config;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        
        public IActionResult About()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetClientList()
        {
            return Json(_context.ChatClients.ToList());
        }

        [HttpGet]
        public JsonResult GetWebSocketURL()
        {
            return Json($"{this.Request.Host}{this.Request.PathBase}");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string type, string url)
        {
            return View(new ErrorViewModel { RequestId = "Error", ErrorType = type, ReturnUrl = url });
        }
    }
}
