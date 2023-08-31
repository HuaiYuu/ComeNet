using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComeNet.Data;
using ComeNet.Models;
using Newtonsoft.Json;
using System.Net.Http;
using AWSWEBAPP.Services;
using Microsoft.AspNetCore.Cors;
using ComeNet.Services;
using ComeNet.Hubs;
using Microsoft.AspNetCore.SignalR;
using NuGet.Common;
using Sprache;
using WebRTC.Hubs;
using User = ComeNet.Models.User;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace ComeNet.Controllers
{
    public class ParasUserSignUp
    {
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }


    }
    public class ParasUserSignIn
    {      
		public string provider { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string access_token { get; set; }
		public string latitude { get; set; }
		public string longitude { get; set; }
	}
    public class ParasCreateActivityPeople
    {
        public string name { get; set; }
        public int id { get; set; }       
    }
    public class ParasCreateActivityPeopleDetail
    {
        public string activityid { get; set; }
        public string name { get; set; }
        public int id { get; set; }
    }
    public class ParasGetActivityList
    {       
        public int id { get; set; }
    }
    public class ParasCreateActivity
    {
        public string date { get; set; }
        public string time { get; set; }
        public List<ParasCreateActivityPeople> people { get; set; }        
        public string location { get; set; }
        public string activityname { get; set; }
        public List<ParasCreateActivityPeople> creater { get; set; }
       
    }
    public class ParasCreateChatRoom
    {
        public string chater1 { get; set; }
        public string chater2 { get; set; }
    }
    public class ParasUserFriendList
    {
        public int userid { get; set; }       
    }
    public class ParasChatContext
    {
        public int roomId { get; set; }
    }
    
    public class Userfriend
	{
		public int id { get; set; }
		public string provider { get; set; }
		public string name { get; set; }
		public string email { get; set; }
		public string picture { get; set; }
		public string password { get; set; }
		public string latitude { get; set; }
		public string longitude { get; set; }
        public double distance { get; set; }
	}

   

    public class ActivitynPeople
    {
        public int Id { get; set; }
        public string activityname { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public string location { get; set; }
        public string creater { get; set; }

        public List<string> people { get; set; }

    }
    public class ResultFriendName
    {        
        public string name { get; set; }

        public string picture { get; set; }

        public string Id { get; set; }
    }
    public class ResultCreateActivity
    {
        public string message { get; set; }
    }
    public class ResultGetActivityList
    {        
        public List<ActivitynPeople> activity { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ComeNetContext _context;
        private readonly HttpClient _httpClient;
        private IPasswordHashService _passwordHashService;
        private IUserService _userService;
        private readonly IHubContext<NotificationUserHub> _notificationUserHubContext;
        private readonly IUserConnectionManager _userConnectionManager;
      

        public UsersController(ComeNetContext context, HttpClient httpClient, IPasswordHashService passwordHashService, IUserService userService, IHubContext<NotificationUserHub> notificationUserHubContext, IUserConnectionManager userConnectionManager)
        {
            _context = context;
            _httpClient = httpClient;
            _passwordHashService = passwordHashService;
            _userService = userService;
            _notificationUserHubContext = notificationUserHubContext;
            _userConnectionManager = userConnectionManager;           
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
          if (_context.User == null)
          {
              return NotFound();
          }
            return await _context.User.ToListAsync();
        }

        [HttpPost("SendToSpecificUser")]
        public async Task<ActionResult> SendToSpecificUser(Article model)
        {
            var connections = _userConnectionManager.GetUserConnections(model.userId);
            if (connections != null && connections.Count > 0)
            {
                foreach (var connectionId in connections)
                {
                    await _notificationUserHubContext.Clients.Client(connectionId).SendAsync("sendToUser", model.articleHeading, model.articleContent);
                }
            }



            return Ok(model);
        }

		[HttpPost("ChatToSpecificUser")]
		public async Task<ActionResult> ChatToSpecificUser(ChatContext model)
		{
            //var connections = _userConnectionManager.GetUserConnections(model.userId);
            //if (connections != null && connections.Count > 0)
            //{
            //	foreach (var connectionId in connections)
            //	{
            //                 //await _notificationUserHubContext.Clients.Client(connectionId).SendAsync("chatToUser", model.name, model.message);
            //               //  await _notificationUserHubContext.Clients.All.SendAsync("chatToUser", model.name, model.message);
            //                 await _notificationUserHubContext.Clients.Group("18to19").SendAsync("chatToUser", model.name, model.message);
            //             }
            //}

            MessageContext context = new MessageContext();
            context.message = model.message;
            context.name = model.name;
            context.datetime = DateTime.Now;
            context.roomId = 19218;


            _context.MessageContext.Add(context);
            await _context.SaveChangesAsync();



            await _notificationUserHubContext.Clients.All.SendAsync("chatToUser", model.name, model.message);
            return Ok(model);
		}

        private static Dictionary<string, List<string>> OnlineUserMap = new Dictionary<string, List<string>>();

        private List<ResultFriendName> users = new List<ResultFriendName>();

        [HttpPost("GetOnlineUser")]
        public async Task<ActionResult> GetOnlineUser(ParasUserFriendList paras)
        {


            var connections = _userConnectionManager.GetAllUsers();           
            foreach (var connectionId in connections)
            {
                
                if(connectionId !=  paras.userid.ToString())
                {
                    var myuser = await _context.User.FindAsync(Convert.ToInt32(connectionId));
                    ResultFriendName resultFriendName = new ResultFriendName();
                    resultFriendName.name = myuser.name;
                    resultFriendName.picture = myuser.picture;
                    resultFriendName.Id = myuser.id.ToString();

                    if (!OnlineUserMap.ContainsKey(myuser.name))
                    {
                        OnlineUserMap[myuser.name] = new List<string>();
                    }
                    OnlineUserMap[myuser.name].Add(connectionId);
                    users.Add(resultFriendName);
                }
            }

            return Ok(users);
        }

        [HttpPost("Signup")]		
		public async Task<ActionResult<IEnumerable<User>>> UserSignup([FromBody] ParasUserSignUp paras)
		{
			string hashedPassword;
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
                
				_context.Add(user);
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = "使用者信箱已被註冊" });
			}
			var token = _userService.Authenticate(paras.email, hashedPassword);

			return Ok(token);
		}

		[HttpPost("Signin")]
        public async Task<ActionResult<IEnumerable<User>>> UserSignin([FromBody] ParasUserSignIn paras)
        {
            if (paras.provider == "native")
            {
                try
                {
                    string hashedPassword = _passwordHashService.HashPassword(paras.password);
                    paras.password = hashedPassword;
                    var token = _userService.Authenticate(paras.email, paras.password);

                    string accesstoken = token.access_token.ToString();
                    string username = token.user.name.ToString();
                    string email = token.user.email.ToString();
                    int id = token.user.id;

                    if (token == null)
                    {
                        return BadRequest(new { nessage = "使用者名稱或密碼不正確" });
                    }

                    HttpContext.Session.SetString("token", accesstoken);
                    HttpContext.Session.SetString("name", username);
                    HttpContext.Session.SetString("id", id.ToString());


                    try
					{
                        User user = await _context.User.FirstOrDefaultAsync(u => u.id == id);							
                        user.latitude= paras.latitude;
						user.longitude= paras.longitude;

						_context.Update(user);
						await _context.SaveChangesAsync();
					}
					catch (Exception ex)
					{
						return BadRequest(new { message = "使用者信箱已被註冊" });
					}

					return Ok(token);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }


            }
            else if (paras.provider == "facebook")
            {
                using (var response = await _httpClient.GetAsync("https://graph.facebook.com/me?fields=id,name,email,picture&access_token=" + paras.access_token))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        UserProfile userProfile = new UserProfile();
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        userProfile = JsonConvert.DeserializeObject<UserProfile>(apiResponse);
                        var token = _userService.Authenticate(userProfile.Email, "facebook");
                        if (token == null)
                        {
                            return BadRequest(new { nessage = "使用者名稱或密碼不正確" });
                        }

                        string accesstoken = token.access_token.ToString();
                        string username = token.user.name.ToString();
                        string email = token.user.email.ToString();

                        HttpContext.Session.SetString("token", accesstoken);
                        HttpContext.Session.SetString("name", username);
                        HttpContext.Session.SetString("email", email);

                        return Ok(token);
                    }
                    else
                    {
                        return BadRequest(new { message = "登入失敗" });
                    }
                }
            }
            return BadRequest(new { message = "登入失敗" });
        }

        [HttpPost("CreateActivity")]
        public async Task<ActionResult<IEnumerable<ActivityList>>> CreateActivity(ParasCreateActivity paras)
        {

            ActivityList user = new ActivityList();
            user.date = paras.date;
            user.time = paras.time;
            user.location = paras.location;
            user.activityname = paras.activityname;
            foreach (var person in paras.creater)
            {
                user.creater = person.name;
            }
               
            _context.ActivityList.Add(user);
            await _context.SaveChangesAsync();

            var getactivityid = await _context.ActivityList
           .Where(f => f.date == paras.date && f.time == paras.time && f.location == paras.location && f.location == paras.location)
           .Select(f => f.Id)
           .ToListAsync();

            ActivityDetail activityDetail = new ActivityDetail();
            activityDetail.activityId = getactivityid[0];            

            foreach (var person in paras.people)
            {

                //activityDetail.username = person.name;
                //activityDetail.userId = person.id.ToString();
                //_context.ActivityDetail.Add(activityDetail);
                //await _context.SaveChangesAsync();
                try
                {
                    var connections = _userConnectionManager.GetUserConnections(person.id.ToString());
                    if (connections != null && connections.Count > 0)
                    {
                        foreach (var connectionId in connections)
                        {
                            await _notificationUserHubContext.Clients.Client(connectionId).SendAsync("activityinvitation", paras.activityname, paras.location + "," + paras.date + " " + paras.time, getactivityid[0]);
                        }
                    }
                }
                catch (Exception ex) 
                {

                    ///save into datebase
                }                
            }

            ResultCreateActivity result = new ResultCreateActivity();
            result.message = "ok";

             return Ok(result);           
        }

        [HttpPost("CreateActivityPeople")]
        public async Task<ActionResult<IEnumerable<ActivityDetail>>> CreateActivityPeople(ParasCreateActivityPeopleDetail paras)
        {

            ActivityDetail activityDetail = new ActivityDetail();
            activityDetail.activityId = Convert.ToInt32(paras.activityid);
            activityDetail.username = paras.name;
            activityDetail.userId = paras.id.ToString();
         

            _context.ActivityDetail.Add(activityDetail);
            await _context.SaveChangesAsync();

            ResultCreateActivity result = new ResultCreateActivity();
            result.message = "ok";

            return Ok(result);
        }

        [HttpPost("CreateChatRoom")]
        public async Task<ActionResult<IEnumerable<ActivityDetail>>> CreateChatRoom(ParasCreateChatRoom paras)
        {

            string roomid = "";
            string chater1 = paras.chater1;
            string chater2 = paras.chater2;

            if ( Convert.ToUInt16(chater1) > Convert.ToUInt16(chater2))
            {
                roomid = chater1 + "2" + chater2;
            }
            else
            {
                roomid = chater2 + "2" + chater1;
            }

            ResultCreateActivity result = new ResultCreateActivity();
            result.message = roomid;

            return Ok(result);
        }

        [HttpPost("CreateChatContext")]
        public async Task<ActionResult<IEnumerable<MessageContext>>> CreateChatContext(MessageContext paras)
        {

            MessageContext context = new MessageContext();
            context.message = paras.message;
            context.name= paras.name;
            context.datetime = DateTime.Now;
            context.roomId = paras.roomId;


            _context.MessageContext.Add(context);
            await _context.SaveChangesAsync();


            return Ok();
        }

        [HttpPost("Friendlist")]
        public async Task<ActionResult<IEnumerable<Userfriend>>> UserFriendList(ParasUserFriendList paras)
        {
            var friendIds = await _context.Friendlist
            .Where(f => f.userid == paras.userid)
            .Select(f => f.friendid)
            .ToListAsync();


			var myuser = await _context.User.FindAsync(paras.userid);


			GeoCoordinate point1 = new GeoCoordinate();
            point1.Latitude = Convert.ToDouble( myuser.latitude);
			point1.Longitude = Convert.ToDouble(myuser.longitude);



			List<Userfriend> userlist = new List<Userfriend>();

			foreach (var userid in friendIds)
            {
				Userfriend userfriend = new Userfriend();

				var users = await _context.User.FindAsync(userid);
				
				GeoCoordinate point2 = new GeoCoordinate();
				point2.Latitude = Convert.ToDouble(users.latitude);
				point2.Longitude = Convert.ToDouble(users.longitude);

				double distance = GeoCalculator.CalculateDistance(point1, point2);

				string formattedDistance = string.Format("{0:F1}", distance);

                distance = Convert.ToDouble(formattedDistance);

				userfriend.distance = distance;
                userfriend.id = userid;
                userfriend.email=users.email;
                userfriend.name = users.name;
                userfriend.picture = users.picture;
                

				
				if (users == null )
                {
                    return NotFound();
                }

                userlist.Add(userfriend);

            }

            return userlist;
        }

        [HttpPost("GetNearbyUser")]
        public async Task<ActionResult<IEnumerable<Userfriend>>> GetNearbyUser(ParasUserFriendList paras)
        {
            //var friendIds = await _context.Friendlist
            //.Where(f => f.userid == paras.userid)
            //.Select(f => f.friendid)
            //.ToListAsync();

            var friendIds = await _context.User
            .Where(f => f.id != paras.userid)
            .Select(f => f.id)
            .ToListAsync();


            var myuser = await _context.User.FindAsync(paras.userid);


            GeoCoordinate point1 = new GeoCoordinate();
            point1.Latitude = Convert.ToDouble(myuser.latitude);
            point1.Longitude = Convert.ToDouble(myuser.longitude);



            List<Userfriend> userlist = new List<Userfriend>();

            foreach (var userid in friendIds)
            {
                Userfriend userfriend = new Userfriend();

                var users = await _context.User.FindAsync(userid);

                GeoCoordinate point2 = new GeoCoordinate();
                point2.Latitude = Convert.ToDouble(users.latitude);
                point2.Longitude = Convert.ToDouble(users.longitude);

                double distance = GeoCalculator.CalculateDistance(point1, point2);

                string formattedDistance = string.Format("{0:F1}", distance);

                distance = Convert.ToDouble(formattedDistance);

                userfriend.distance = distance;
                userfriend.id = userid;
                userfriend.email = users.email;
                userfriend.name = users.name;
                userfriend.picture = users.picture;
                userfriend.distance = distance;



                if (users == null)
                {
                    return NotFound();
                }
                if(distance<5)
                {
                    userlist.Add(userfriend);
                }
            }

            return userlist;
        }

        [HttpPost("GetActivityList")]
        public async Task<ActionResult<IEnumerable<ResultGetActivityList>>> GetActivityList(ParasGetActivityList paras)
        {
            ResultGetActivityList resultGetActivityList = new ResultGetActivityList();
            List<ActivitynPeople> activityLists = new List<ActivitynPeople>();
            var activitylist = await _context.ActivityList
            .ToListAsync();
            foreach (var activity in activitylist)
            {
                ActivitynPeople activitynPeople = new ActivitynPeople();
                activitynPeople.Id = activity.Id;
                activitynPeople.date=activity.date.Trim();
                activitynPeople.time=activity.time.Trim();
                activitynPeople.location=activity.location.Trim();
                activitynPeople.activityname = activity.activityname.Trim();
                activitynPeople.creater = activity.creater.Trim();

                var people = await _context.ActivityDetail
                .Where(f => f.activityId == activity.Id)
                .Select(f => f.username)
                .ToListAsync();

                List<string> peopleList = new List<string>();

                foreach (var person in people)
                {
                    peopleList.Add(person.Trim());
                }

                activitynPeople.people = peopleList;

                activityLists.Add(activitynPeople);
            }

            resultGetActivityList.activity = activityLists;

            return Ok(activityLists);
        }

        [HttpPost("GetChatContext")]
        public async Task<ActionResult<IEnumerable<MessageContext>>> GetChatContext(int paras)
        {
            string user1 = "Demo";
            string user2 = "Demo1";
            string chatcontext = "";

            

            StringBuilder chatHtml = new StringBuilder();

            var message = await _context.MessageContext
            .Where(f => f.roomId == 19218)           
            .ToListAsync();

            foreach (var messages in message)
            {
                if (messages.name.Trim()==user1)
                {
                     chatcontext = "<p class=\"sender-demo\">"+messages.name+":"+messages.message + "</p>";                 
                } 
                else if (messages.name.Trim() == user2)
                {
                     chatcontext = "<p class=\"sender-demo1\">" + messages.name + ":" + messages.message + "</p>";
                }

                chatHtml.Append(chatcontext);
            }
            string chatHtmlString = chatHtml.ToString();

            ResultCreateActivity result = new ResultCreateActivity();
            result.message = chatHtmlString;


            return Ok(result);
        }

    }
}
