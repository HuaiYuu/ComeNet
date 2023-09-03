using AWSWEBAPP.Services;
using ComeNet.Data;
using ComeNet.Hubs;
using ComeNet.Models;
using ComeNet.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Sprache;
using System.Diagnostics;
using System.Drawing;
using WebRTC.Hubs;
using User = ComeNet.Models.User;

namespace ComeNet.Controllers
{

	public class HomeController : Controller
    {
        private readonly ComeNetContext _context;
        private readonly ILogger<HomeController> _logger;
        private IUserService _userService;
        private readonly IHubContext<NotificationUserHub> _notificationUserHubContext;
		private readonly IUserConnectionManager _userConnectionManager;
        private IPasswordHashService _passwordHashService;
        public HomeController(ILogger<HomeController> logger,IHubContext<NotificationUserHub> notificationUserHubContext, IUserService userService, IUserConnectionManager userConnectionManager, ComeNetContext context, IPasswordHashService passwordHashService)
        {
            _context = context;
            _logger = logger;
			_notificationUserHubContext = notificationUserHubContext;
			_userConnectionManager = userConnectionManager;
            _userService = userService;
            _passwordHashService = passwordHashService;
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
        public IActionResult test()
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
        public IActionResult Suggestion()
        {
            var name = HttpContext.Session.GetString("name");
            var id = HttpContext.Session.GetString("id");
            ViewBag.Name = name;
            ViewBag.id = id;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup(User paras, IFormFile myimg)
        {
            string hashedPassword;
            string targetFolderPath = @"\upload";
            string fileFolderPath = @"wwwroot\upload";

            try
            {
                //save user profile
                User user = new User();
                user.name = paras.name;
                user.email = paras.email;
                hashedPassword = _passwordHashService.HashPassword(paras.password);
                user.password = hashedPassword;
                user.provider = "native";
                user.picture = "https://schoolvoyage.ga/images/123498.png";
                user.age = paras.age;
                user.horoscope = paras.horoscope;
                user.answer = paras.answer;
                user.question = paras.question;
                user.gender = paras.gender;
                user.interest = paras.interest;
                user.longitude = "default";
                user.latitude = "default";

                if (myimg != null)
                {
                    string fileName = myimg.FileName;
                    string filePath = Path.Combine(targetFolderPath, fileName);
                    user.picture = filePath;
                    string filelocation= Path.Combine(fileFolderPath, fileName);
                    using (var stream = new FileStream(filelocation, FileMode.Create))
                    {
                        await myimg.CopyToAsync(stream);
                    }
                }

                _context.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "使用者信箱已被註冊" });
            }
            var token = _userService.Authenticate(paras.email, hashedPassword);

            TempData["SuccessMessage"] = "註冊成功";

            return RedirectToAction("index", "Home");
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