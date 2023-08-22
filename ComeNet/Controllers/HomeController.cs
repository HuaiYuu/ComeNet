using ComeNet.Hubs;
using ComeNet.Models;
using ComeNet.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace ComeNet.Controllers
{

	public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

		
		private readonly IHubContext<NotificationUserHub> _notificationUserHubContext;
		private readonly IUserConnectionManager _userConnectionManager;

		public HomeController(ILogger<HomeController> logger,IHubContext<NotificationUserHub> notificationUserHubContext, IUserConnectionManager userConnectionManager)
        {
            _logger = logger;

			
			_notificationUserHubContext = notificationUserHubContext;
			_userConnectionManager = userConnectionManager;
		}

        public IActionResult Index()
        {
            var name = HttpContext.Session.GetString("name");
            var id = HttpContext.Session.GetString("id");
            ViewBag.Name = name;
            ViewBag.id = id;

            return View();
        }

		public IActionResult Friends()
		{
            var name = HttpContext.Session.GetString("name");
            var id = HttpContext.Session.GetString("id");
            ViewBag.Name = name;
            ViewBag.id = id;
            return View();
		}

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Signup()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult User()
        {
            return View();
        }


        public IActionResult SendToSpecificUser()
		{
			
			return View();
		}


		


		public IActionResult StartVideoChat()
		{
			return Redirect($"/{Guid.NewGuid()}");
		}

		[HttpGet("/{roomId}")]
		public IActionResult Room(string roomId)
		{
			ViewBag.roomId = roomId;
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}