using AWSWEBAPP.Services;
using Azure.Core;
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
using Newtonsoft.Json;
using NuGet.Protocol;
using Sprache;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Net.Http;
using WebRTC.Hubs;
using User = ComeNet.Models.User;

namespace ComeNet.Controllers
{

    public class FBAccessToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string auth_type { get; set; }        
    }



    public class HomeController : Controller
    {
        private readonly ComeNetContext _context;
        private readonly ILogger<HomeController> _logger;
        private IUserService _userService;
        private readonly HttpClient _httpClient;
        private readonly IHubContext<NotificationUserHub> _notificationUserHubContext;
		private readonly IUserConnectionManager _userConnectionManager;
        private IPasswordHashService _passwordHashService;
        private IQueueService _queueService;
        public HomeController(ILogger<HomeController> logger,IHubContext<NotificationUserHub> notificationUserHubContext, IUserService userService, HttpClient httpClient, IQueueService queueService, IUserConnectionManager userConnectionManager, ComeNetContext context, IPasswordHashService passwordHashService)
        {
            _context = context;
            _logger = logger;
			_notificationUserHubContext = notificationUserHubContext;
			_userConnectionManager = userConnectionManager;
            _userService = userService;
            _httpClient = httpClient;
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

            if(id == null)
            {
                return RedirectToAction("home", "Home");
            }

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
            if (id == null)
            {
                return RedirectToAction("home", "Home");
            }
            ViewBag.Name = name;
            ViewBag.id = id;
            return View();
		}       
        public async Task<IActionResult> Login(string code)
        {
            string AppId = Environment.GetEnvironmentVariable("AppId");
            string AppSecret = Environment.GetEnvironmentVariable("AppSecret");

            FBAccessToken accessToken = new FBAccessToken();
            UserProfile userProfile = new UserProfile();
            User user = new User();
            if (code == null) return View();


            try
            {
                var data = new
                {
                    client_id = AppId,
                    client_secret = AppSecret,
                    redirect_uri = "https://localhost:7136/Home/LOGIN",
                    code = code,
                };
                var content = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");
                using (var response = await _httpClient.PostAsync("https://graph.facebook.com/v17.0/oauth/access_token", content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        accessToken = JsonConvert.DeserializeObject<FBAccessToken>(apiResponse);
                    }
                    else
                    {
                        return null;
                    }
                }
                using (var response = await _httpClient.GetAsync("https://graph.facebook.com/me?fields=id,name,email,picture&access_token=" + accessToken.access_token))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        userProfile = JsonConvert.DeserializeObject<UserProfile>(apiResponse);

                        var userdata = await _context.User.Where(p => p.email == userProfile.Email).ToListAsync();
                        if (userdata.Count == 0)
                        {
                            User userbyfacebook = new User();
                            userbyfacebook.provider = "facebook";
                            userbyfacebook.name = userProfile.Name;
                            userbyfacebook.email = userProfile.Email;
                            userbyfacebook.password = "*****";
                            userbyfacebook.picture = userProfile.Picture.data.Url;
                            userbyfacebook.age = 0;
                            userbyfacebook.job = "";
                            userbyfacebook.latitude = "default";
                            userbyfacebook.longitude = "default";
                            userbyfacebook.answer = "default";
                            userbyfacebook.question = "default";
                            userbyfacebook.interest = "default";
                            userbyfacebook.gender = "default";
                            userbyfacebook.horoscope = "default";
                          
                            _context.Add(userbyfacebook);
                            await _context.SaveChangesAsync();

                            return RedirectToAction("profile", "home");
                        }
                        else
                        {
                            var token = _userService.Authenticate(userProfile.Email, "*****");

                            string accesstoken = token.access_token.ToString();
                            string username = token.user.name.ToString();
                            string email = token.user.email.ToString();

                            if (token == null)
                            {
                                return BadRequest(new { nessage = "使用者名稱或密碼不正確" });
                            }

                            HttpContext.Session.SetString("token", accesstoken);
                            HttpContext.Session.SetString("name", username);
                            HttpContext.Session.SetString("email", email);

                        }

                        return RedirectToAction("index", "home");
                    }
                    else
                    {
                        return BadRequest(new { message = "" });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return View();
        }      
        public IActionResult NearBy()
        {
            var name = HttpContext.Session.GetString("name");
            var id = HttpContext.Session.GetString("id");
            if (id == null)
            {
                return RedirectToAction("home", "Home");
            }
            ViewBag.Name = name;
            ViewBag.id = id;
            return View();            
        }
        public IActionResult Chat()
        {
            return View();
        }
        public IActionResult Message()
        {
            var name = HttpContext.Session.GetString("name");
            var id = HttpContext.Session.GetString("id");
            if (id == null)
            {
                return RedirectToAction("home", "Home");
            }
            ViewBag.Name = name;
            ViewBag.id = id;
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
            var name = HttpContext.Session.GetString("name");
            var id = HttpContext.Session.GetString("id");
            HttpContext.Session.Clear();

            var connections = _userConnectionManager.GetUserConnections(id);

            if(connections != null)
            {
                _userConnectionManager.RemoveUserConnectionStatus(id);
            }

            return RedirectToAction("home", "Home");
        }
        public IActionResult Signup()
        {
            return View();
        }
        public async Task<IActionResult> Profile()
        {
            var name = HttpContext.Session.GetString("name");
            var id = HttpContext.Session.GetString("id");
            ViewBag.Name = name;
            ViewBag.id = id;
            ViewBag.message = "load";

            var product = await _context.User.Where(f=>f.id== Convert.ToInt32(id)).FirstOrDefaultAsync();
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }            
        public IActionResult Privacy()
        {
            var name = HttpContext.Session.GetString("name");
            var id = HttpContext.Session.GetString("id");
            ViewBag.Name = name;
            ViewBag.id = id;
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

		[HttpGet("/VideoRoom/{roomId}")]
		public IActionResult VideoRoom(string roomId)
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
                user.job=paras.job;
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
                return BadRequest(new { message = ex.Message });
            }
            var token = _userService.Authenticate(paras.email, hashedPassword);

            TempData["SuccessMessage"] = "註冊成功";

            return RedirectToAction("index", "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(User paras)
        {

            var name = HttpContext.Session.GetString("name");
            var id = HttpContext.Session.GetString("id");

            ViewBag.Name = name;
            ViewBag.id = id;

            var product = await _context.User.Where(f => f.id == Convert.ToInt32(paras.id)).FirstOrDefaultAsync();

            product.email=paras.email;
            product.age = paras.age;
            product.job = paras.job;
            product.horoscope = paras.horoscope;
            product.gender = paras.gender;
            product.name = paras.name;
            product.interest = paras.interest;
            


            try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    ViewBag.message = "修改成功";
            }
                catch (DbUpdateConcurrencyException)
                {
                ViewBag.message = "修改失敗";
            }
              
         
            return View(product);
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

                    using (SqlTransaction transaction = conn.BeginTransaction(System.Data.IsolationLevel.Serializable))
                    {

                        var id = HttpContext.Session.GetString("id");
                        int newTicketCount = 0;
                        int currentTicketCount = 0;
                            try
                            {
                                // 選擇並鎖定特定行
                                string selectSql = string.Format("SELECT number FROM Toollist WITH (UPDLOCK,HOLDLOCK) WHERE  id=1;");
                                SqlCommand selectCmd = new SqlCommand(selectSql, conn, transaction);
                                SqlDataReader reader = selectCmd.ExecuteReader();

                            //if (id == "18")
                            //{
                            //    Thread.Sleep(5000);
                            //}

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
                            string updateSql = string.Format("UPDATE Toollist WITH (ROWLOCK,UPDLOCK) SET number = {1} WHERE Toolname=N'{0}' and id=1;", paras.toolname, newTicketCount);
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

                            result.message = "成功";

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