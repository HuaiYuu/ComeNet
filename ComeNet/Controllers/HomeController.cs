using ComeNet.Hubs;
using ComeNet.Models;
using ComeNet.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using WebRTC.Hubs;

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
        public IActionResult home()
        {
            var name = HttpContext.Session.GetString("name");
            var id = HttpContext.Session.GetString("id");

            if(id == null)
            {
                id = "-1";
            }

            ViewBag.Name = name;
            ViewBag.id = id;

            return View();
        }

        public IActionResult ActivityList()
        {
            var name = HttpContext.Session.GetString("name");
            var id = HttpContext.Session.GetString("id");
            ViewBag.Name = name;
            ViewBag.id = id;

            return View();
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
        public IActionResult NearBy()
        {
            var name = HttpContext.Session.GetString("name");
            var id = HttpContext.Session.GetString("id");
            ViewBag.Name = name;
            ViewBag.id = id;
            return View();            
        }
        public IActionResult Chat()
        {
            return View();
        }
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("index", "Home");
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

			var id = HttpContext.Session.GetString("id");

			//return Redirect($"/{Guid.NewGuid()}");
			return Redirect($"/{id}");
		}

		[HttpGet("/{roomId}")]
		public IActionResult Room(string roomId)
		{
			var name = HttpContext.Session.GetString("name");
			var id = HttpContext.Session.GetString("id");
			ViewBag.Name = name;
			ViewBag.id = id;
			ViewBag.roomId = roomId;
			return View();
		}

        public IActionResult StartChat()
        {
            var id = HttpContext.Session.GetString("id");               
            return Redirect($"/ChatRoom/{id}");
        }

        [HttpGet("/ChatRoom/{roomId}")]
        public IActionResult ChatRoom(string roomId)
        {

            var name = HttpContext.Session.GetString("name");
            var id = HttpContext.Session.GetString("id");

            ViewBag.Name = name;
            ViewBag.id = id;
            ViewBag.roomId = roomId;

            return View();           
        }
        

        [HttpGet("Chat/{Id}")]
        public IActionResult Chat(string Id)
        {
            return View();
        }

        public IActionResult Chat2user()
        {
            var name = HttpContext.Session.GetString("name");
            var id = HttpContext.Session.GetString("id");
            ViewBag.Name = name;
            ViewBag.id = id;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}