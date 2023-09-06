using AWSWEBAPP.Services;
using ComeNet.Data;
using ComeNet.Hubs;
using ComeNet.Models;
using ComeNet.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using Sprache;
using System.Data;
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
        private IQueueService _queueService;
        public HomeController(ILogger<HomeController> logger,IHubContext<NotificationUserHub> notificationUserHubContext, IUserService userService, IQueueService queueService, IUserConnectionManager userConnectionManager, ComeNetContext context, IPasswordHashService passwordHashService)
        {
            _context = context;
            _logger = logger;
			_notificationUserHubContext = notificationUserHubContext;
			_userConnectionManager = userConnectionManager;
            _userService = userService;
            _passwordHashService = passwordHashService;
            _queueService = queueService;
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
        public IActionResult Card()
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
        public IActionResult LimitedTimeEvent()
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
        public async Task<IActionResult> ChatRoom(string roomId)
        {

            var name = HttpContext.Session.GetString("name");
            var id = HttpContext.Session.GetString("id");

            ViewBag.Name = name;
            ViewBag.id = id;
            ViewBag.roomId = roomId;

            string[] users = roomId.Split("8507");
            string notifyid = "";

            if (id == users[0])
            {
                notifyid = users[1];
            }
            else if (id == users[1])
            {
                notifyid = users[0];
            }


            var userprofile = _context.User.Where(f => f.id == Convert.ToInt16(notifyid)).FirstOrDefault();

            ViewBag.notifyname = userprofile.name;
            ViewBag.picture = userprofile.picture;

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


        private static readonly Queue<ToolRequest> requestQueue = new Queue<ToolRequest>();

        [HttpPost]
        public IActionResult LimitedTimeEvent([FromBody] ParasLimitedTimeEvent paras)
        {
            DotNetEnv.Env.Load();
            string connString = Environment.GetEnvironmentVariable("CONNECTION_STRING"); ;

            ResultCreateActivity result = new ResultCreateActivity();

            int num = requestQueue.Count();

            if(num < 0) 
            {
                result.message = "額滿";                
            }
            else
            {


                //ToolRequest toolRequest = new ToolRequest();
                //toolRequest.ToolName = paras.toolname;
                //toolRequest.ReceiverUserId = paras.userid;
                //requestQueue.Enqueue(toolRequest);
                //result.message = "成功";

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {

                        var id = HttpContext.Session.GetString("id");
                        int newTicketCount = 0;
                        int currentTicketCount = 0;
                            try
                            {
                                // 選擇並鎖定特定行
                                string selectSql = string.Format("SELECT * FROM Toollist WITH (UPDLOCK) WHERE Toolname=N'{0}';", paras.toolname);
                                SqlCommand selectCmd = new SqlCommand(selectSql, conn, transaction);
                                SqlDataReader reader = selectCmd.ExecuteReader();

                            if (id == "18")
                            {
                                Thread.Sleep(10000);
                            }

                            while (reader.Read())
                                {
                                    currentTicketCount = (int)reader["number"];                                   
                                    newTicketCount = currentTicketCount - 1;
                                }
                                reader.Close();
                            if (currentTicketCount < 1)
                            {
                                transaction.Rollback();
                                result.message = "已額滿";
                                return Ok(result);
                            }
                            string updateSql = string.Format("UPDATE Toollist SET number = {1} WHERE Toolname=N'{0}';", paras.toolname, newTicketCount);
                                SqlCommand updateCmd = new SqlCommand(updateSql, conn, transaction);
                                updateCmd.ExecuteNonQuery();
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                               // return RedirectToAction("Login", "Home");
                                throw ex;
                            }

                       
                       
                    }
                }
            }

            //while (true)
            //{

            //    ToolRequest Request = requestQueue.Dequeue();

            //    if (Request != null)
            //    {
            //        // 處理 ToolRequest，例如執行工具操作
            //        Console.WriteLine($"處理 ToolRequest - 工具名稱：{Request.ToolName}，接收者用戶ID：{Request.ReceiverUserId}");
            //    }
            //    result.message = num.ToString();

            //    Thread.Sleep(1000); 
            //    return Ok(result);
            //}
            return Ok(result);
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