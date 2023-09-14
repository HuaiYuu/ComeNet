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
using NuGet.Protocol;
using Newtonsoft.Json.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol.Plugins;
using System.Reflection;

namespace ComeNet.Controllers
{
    public class ParasUserSignUp
    {
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string gender { get; set; }
        public string interest { get; set; }
        public int age { get; set; }
        public string horoscope { get; set; }
        public string answer { get; set; }
        public string question { get; set; }        
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
        public int sender { get; set; }
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

        public string chater { get; set; }
        public string chater1 { get; set; }
        public string chater2 { get; set; }

      
    }
    public class ParasUserFriendList
    {
        public int userid { get; set; }       
    }
    public class ParasUserRejectList
    {
        public int userid { get; set; }
        public int rejectid { get; set; }
    }
    public class ParasUserAgreeList
    {
        public int userid { get; set; }
        public int agreeid { get; set; }
    }
    public class ParasLimitedTimeEvent
    {
        public string toolname { get; set; }
        public int userid { get; set; }
    }
    public class ParasChatContext
    {
        public int roomId { get; set; }
    }    
    public class ParasCreateToollist
    {       
        public int userid { get; set; }
        public string toolname { get; set; }
        public int number { get; set; }
    }
    public class ParasUseTool
    {
        public int userid { get; set; }
        public string toolname { get; set; }        
    }
    public class ParasGetVideoUser
    {
        public string connectionId { get; set; }
    }
    public class Userfriend
	{
		public int id { get; set; }
		public string provider { get; set; }
		public string name { get; set; }
		public string email { get; set; }
		public string picture { get; set; }				
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
    public class ResultGetOnlineUser
    {
        public List<ResultFriendName> onlineuser = new List<ResultFriendName>();

        public List<ResultFriendName> offlineuser = new List<ResultFriendName>();

    }
    public class ResultFriendName
    {        
        public string name { get; set; }

        public string picture { get; set; }

        public string Id { get; set; }
    }
    public class ResultSuggestionFriend
    {
        public int id { get; set; }
        public string name { get; set; }
        public string picture { get; set; }
        public string gender { get; set; }
        public string interest { get; set; }
        public int age { get; set; }
        public string horoscope { get; set; }
        public string answer { get; set; }
        public string question { get; set; }

        public string message { get; set; }


    }
    public class ResultCreateActivity
    {
        public string message { get; set; }
    }
    public class ResultGetActivityList
    {        
        public List<ActivitynPeople> activity { get; set; }
    }
    public class ResultGetUserToolList
    {
        public List<UserToollist> tools { get; set; }
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
        private IQueueService _queueService;


        public UsersController(ComeNetContext context, HttpClient httpClient, IPasswordHashService passwordHashService, IUserService userService, IQueueService queueService, IHubContext<NotificationUserHub> notificationUserHubContext, IUserConnectionManager userConnectionManager)
        {
            _context = context;
            _httpClient = httpClient;
            _passwordHashService = passwordHashService;
            _userService = userService;
            _notificationUserHubContext = notificationUserHubContext;
            _userConnectionManager = userConnectionManager;    
            _queueService = queueService;
        }

        [HttpPost("GetUser")]
        public async Task<ActionResult<IEnumerable<User>>> GetUser(ParasUserFriendList paras)
        {
            var userprofile = await _context.User.FirstOrDefaultAsync(f => f.id == paras.userid);

            if (userprofile == null)
            {
                return NotFound();
            }

            return Ok(userprofile);
        }

        [HttpPost("SendToSpecificUser")]
        public async Task<ActionResult> SendToSpecificUser(Article model)
        {
            var id = HttpContext.Session.GetString("id");

            ResultCreateActivity resultmsg = new ResultCreateActivity();
            try
            {
                var connections = _userConnectionManager.GetUserConnections(model.userId);
                if (connections != null && connections.Count > 0)
                {
                    foreach (var connectionId in connections)
                    {
                        await _notificationUserHubContext.Clients.Client(connectionId).SendAsync("sendToUser", model.articleHeading, model.articleContent);
                    }
                    resultmsg.message = "ok";
                }
            }
            catch
            {
                resultmsg.message = "用戶不在線上";
            }


                var username = await _context.User
               .Where(f => f.id == Convert.ToInt16(id))
               .Select(f => f.name)
               .FirstAsync();


            UserMessage message = new UserMessage();
            message.name = username;
            message.title = model.articleHeading;
            message.message = model.articleContent;
            message.datetime = DateTime.Now;
            message.receiverid = Convert.ToInt16(model.userId);

            _context.UserMessage.Add(message);
            await _context.SaveChangesAsync();

            return Ok(resultmsg);
        }

		[HttpPost("ChatToSpecificUser")]
		public async Task<ActionResult> ChatToSpecificUser(ChatContext model)
		{
            

            MessageContext context = new MessageContext();
            context.message = model.message;
            context.name = model.name;
            context.datetime = DateTime.Now;
            context.roomId = Convert.ToInt32(model.roomId);

            _context.MessageContext.Add(context);
            await _context.SaveChangesAsync();


            string notifyid = "";
            string[] users = model.roomId.ToString().Split("8507");

            if (model.userId == users[0])
            {
                notifyid = users[1];
            }
            else if(model.userId == users[1])
            {
                notifyid = users[0];
            }

            for (int i = 0;i<users.Count();i++)
            {
                var username = await _context.User
                .Where(f => f.id == Convert.ToInt16(users[i]))
                .Select(f => f.name)
                .FirstAsync();
                users[i]= username;
            }

            string useraim = users[0];
            string usermain = users[1];

            await _notificationUserHubContext.Clients.Group(model.roomId).SendAsync("chatToUser", model.name, model.message, useraim, usermain);

            var connections = _userConnectionManager.GetUserConnections(notifyid);
            if (connections != null && connections.Count > 0)
            {
                foreach (var connectionId in connections)
                {
                    await _notificationUserHubContext.Clients.Client(connectionId).SendAsync("chatnotification", model.name, model.message);
                }
            }
            
            




            return Ok(model);
		}

        private static Dictionary<string, List<string>> OnlineUserMap = new Dictionary<string, List<string>>();

        private List<ResultFriendName> users = new List<ResultFriendName>();

        [HttpPost("GetOnlineUser")]
        public async Task<ActionResult> GetOnlineUser(ParasUserFriendList paras)
        {

            ResultGetOnlineUser resultGetOnlineUser = new ResultGetOnlineUser();

            List<ResultFriendName> onlineusers = new List<ResultFriendName>();

            List<ResultFriendName> offlineusers = new List<ResultFriendName>();

            var friendIds = await _context.Friendlist
            .Where(f => f.userid == paras.userid)
            .Select(f => f.friendid)
            .ToListAsync();

            var connections = _userConnectionManager.GetAllUsers();

            foreach (var userid in friendIds)
            {

                if (connections.Contains(userid.ToString()))
                {                    
                        var myuser = await _context.User.FindAsync(Convert.ToInt32(userid));
                        ResultFriendName resultFriendName = new ResultFriendName();
                        resultFriendName.name = myuser.name;
                        resultFriendName.picture = myuser.picture;
                        resultFriendName.Id = myuser.id.ToString();

                        if (!OnlineUserMap.ContainsKey(myuser.name))
                        {
                            OnlineUserMap[myuser.name] = new List<string>();
                        }
                        OnlineUserMap[myuser.name].Add(userid.ToString());
                        onlineusers.Add(resultFriendName);                    
                }
                else
                {

                    var myuser = await _context.User.FindAsync(Convert.ToInt32(userid));
                    ResultFriendName resultFriendName = new ResultFriendName();
                    resultFriendName.name = myuser.name;
                    resultFriendName.picture = myuser.picture;
                    resultFriendName.Id = myuser.id.ToString();

                    offlineusers.Add(resultFriendName);
                }
            }
            resultGetOnlineUser.onlineuser = onlineusers;
            resultGetOnlineUser.offlineuser= offlineusers;
            
            return Ok(resultGetOnlineUser.ToJson());
        }

        [HttpPost("Signup")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> signup(User paras , IFormFile myimg)        
        {

			string hashedPassword;
            string targetFolderPath = @"/upload";
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
                user.interest =paras.interest;
                user.longitude = "123";
                user.latitude = "123";

                if (myimg != null)
                {
                    string fileName = myimg.FileName;
                    string filePath = Path.Combine(targetFolderPath, fileName);
                    user.picture = filePath;
                    using (var stream = new FileStream(filePath, FileMode.Create))
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

            int sender = 0;
            string decodedString = "";
            ActivityList user = new ActivityList();
            user.date = paras.date;
            user.time = paras.time;
            user.location = paras.location;
            user.activityname = paras.activityname;
            foreach (var person in paras.creater)
            {

                if(person.name.IndexOf('#')>0)
                {                    
                  decodedString = System.Net.WebUtility.HtmlDecode(person.name);
                  user.creater = decodedString;
                }
                else
                {
                    user.creater = person.name;
                }

                
                sender=person.id;
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
                            await _notificationUserHubContext.Clients.Client(connectionId).SendAsync("activityinvitation", person.name + "-" + paras.activityname, paras.location + "," + paras.date + " " + paras.time, getactivityid[0], sender);

                            Notification notification = new Notification();
                            notification.date = Convert.ToDateTime(paras.date);
                            notification.eventid = getactivityid[0];
                            notification.title = user.creater + "-"+paras.activityname;
                            notification.message = paras.location;
                            notification.userid= person.id;
                            notification.senderid = sender;
                            notification.time=paras.time;
                            notification.alreadyread = "N";

                            _context.Notification.Add(notification);
                            await _context.SaveChangesAsync();


                            

                        }
                    }
                }
                catch (Exception ex) 
                {

                    Notification notification = new Notification();
                    notification.date = Convert.ToDateTime(paras.date);
                    notification.eventid = getactivityid[0];
                    notification.title = user.creater + "-" + paras.activityname;
                    notification.message = paras.location;
                    notification.userid = person.id;
                    notification.senderid = sender;
                    notification.time = paras.time;
                    notification.alreadyread = "N";

                    _context.Notification.Add(notification);
                    await _context.SaveChangesAsync();
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

            try
            {

                var notificationUpdate = await _context.Notification
               .Where(f => f.eventid == Convert.ToInt32(paras.activityid) && f.userid == paras.id)
               .ToListAsync();

                foreach (var notification in notificationUpdate)
                {
                    notification.alreadyread = "Y";
                    _context.Update(notification);
                }

                await _context.SaveChangesAsync();

            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }
          
            var friendIds = await _context.Friendlist
           .Where(f => f.userid == paras.id)
           .Select(f => f.friendid)
           .ToListAsync();

            if(friendIds.Contains(paras.sender))
            {

            }
            else
            {
                Friendlist friendlist = new Friendlist();
                friendlist.userid = paras.id;
                friendlist.friendid = paras.sender;

                _context.Friendlist.Add(friendlist);

                Friendlist myfriendlist = new Friendlist();
                myfriendlist.userid = paras.sender;
                myfriendlist.friendid = paras.id;

                _context.Friendlist.Add(myfriendlist);

                await _context.SaveChangesAsync();

            }

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
            string id = paras.chater;

            if ( Convert.ToUInt16(chater1) > Convert.ToUInt16(chater2))
            {
                roomid = chater1 + "8507" + chater2;
            }
            else
            {
                roomid = chater2 + "8507" + chater1;
            }

            ResultCreateActivity result = new ResultCreateActivity();
            result.message = roomid;

            //var connections = _userConnectionManager.GetUserConnections(paras.chater);
            //if (connections != null && connections.Count > 0)
            //{
            //    foreach (var connectionId in connections)
            //    {                    
            //        _notificationUserHubContext.Groups.AddToGroupAsync(connectionId, roomid);
            //        await _notificationUserHubContext.Clients.Group(roomid).SendAsync("user-connected", id, roomid);
            //    }
            //}

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
                userfriend.provider = users.provider;
              



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

        [HttpPost("GetSuggestionFriend")]
        public async Task<ActionResult<IEnumerable<ResultSuggestionFriend>>> GetSuggestionFriend(ParasUserFriendList paras)
        {
           
            var UserIds = await _context.User
            .Where(f => f.id != paras.userid)
            .Select(f => f.id)
            .ToListAsync();


            var friendIds = await _context.Friendlist
           .Where(f => f.userid == paras.userid)
           .Select(f => f.friendid)
           .ToListAsync();


            var rejfriendIds = await _context.Rejectlist
           .Where(f => f.userid == paras.userid)
           .Select(f => f.rejectid)
           .ToListAsync();

            var nonFriendUserIds = UserIds.Except(friendIds).Except(rejfriendIds).ToList();
            List<ResultSuggestionFriend> userlist = new List<ResultSuggestionFriend>();

            if (nonFriendUserIds.Count > 0)
            {
                

                var me = await _context.User.FindAsync(paras.userid);


                foreach (var userid in nonFriendUserIds)
                {

                    ResultSuggestionFriend userfriend = new ResultSuggestionFriend();
                    var users = await _context.User.FindAsync(userid);


                    if (me.gender != users.gender)
                    {
                        if (Math.Abs(me.age - users.age) <= 3)
                        {
                            if (me.interest == users.interest)
                            {
                                userfriend.message = "絕配";
                            }
                        }
                    }
                    else
                    {

                        if (me.interest == users.interest)
                        {
                            userfriend.message = "擁有共同興趣";

                        }
                        else
                        {

                        }
                    }

                    userfriend.id = userid;
                    userfriend.name = users.name;
                    userfriend.picture = users.picture;
                    userfriend.gender = users.gender;
                    userfriend.age = users.age;
                    userfriend.interest = users.interest;
                    userlist.Add(userfriend);
                    return userlist;

                }
            }
            else
            {              
                
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

        [HttpPost("GetUserChatContext")]
        public async Task<ActionResult<IEnumerable<MessageContext>>> GetUserChatContext(ParasChatContext paras)
        {

            var name = HttpContext.Session.GetString("name");
            var id = HttpContext.Session.GetString("id");


            string notifyid = "";
            string[] users = paras.roomId.ToString().Split("8507");

            

            for (int i = 0; i < users.Count(); i++)
            {
                var username = await _context.User
                .Where(f => f.id == Convert.ToInt16(users[i]))
                .Select(f => f.name)
                .FirstAsync();
                users[i] = username;
            }

            string useraim = users[0];
            string usermain = users[1];



            string user1 = useraim;
            string user2 = usermain;
            string chatcontext = "";

            StringBuilder chatHtml = new StringBuilder();

            var message = await _context.MessageContext
            .Where(f => f.roomId == paras.roomId)           
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
        [HttpPost("GetLimitedTimeEvent")]
        public async Task<ActionResult<IEnumerable<MessageContext>>> GetLimitedTimeEvent(ParasLimitedTimeEvent paras)
        {
            ToolRequest toolRequest = new ToolRequest();
            toolRequest.ToolName = paras.toolname;
            toolRequest.ReceiverUserId = paras.userid;

            _queueService.Enqueue(toolRequest);
            int num = _queueService.GetRequestCount();

            //while (true)
            //{
                
            //    ToolRequest Request = _queueService.Dequeue();

            //    if (Request != null)
            //    {
            //        // 處理 ToolRequest，例如執行工具操作
            //        Console.WriteLine($"處理 ToolRequest - 工具名稱：{toolRequest.ToolName}，接收者用戶ID：{toolRequest.ReceiverUserId}");
            //    }

            //    // 可以添加一些休眠時間以避免過度使用 CPU
            //    Thread.Sleep(1000); // 休眠 1 秒
            //    return Ok(num);
            //}
            return Ok(num);
        }

        [HttpPost("CreateRejectFriend")]
        public async Task<ActionResult<IEnumerable<Rejectlist>>> CreateRejectFriend(ParasUserRejectList paras)
        {

            Rejectlist rejectlist = new Rejectlist();
            rejectlist.userid=paras.userid;
            rejectlist.rejectid=paras.rejectid;

            _context.Rejectlist.Add(rejectlist);
            await _context.SaveChangesAsync();


            ResultCreateActivity result = new ResultCreateActivity();
            result.message = "已拒絕";

            return Ok(result);
        }

        [HttpPost("CreateAgreeFriend")]
        public async Task<ActionResult<IEnumerable<Friendlist>>> CreateAgreeFriend(ParasUserAgreeList paras)
        {

            Friendlist friendlist = new Friendlist();
            friendlist.userid=paras.userid;
            friendlist.friendid = paras.agreeid;

            _context.Friendlist.Add(friendlist);
            await _context.SaveChangesAsync();


            ResultCreateActivity result = new ResultCreateActivity();
            result.message = "已成為朋友";

            return Ok(result);
        }

        [HttpPost("CreateToollist")]
        public async Task<ActionResult<IEnumerable<UserToollist>>> CreateToollist(ParasCreateToollist paras)
        {

            UserToollist userToollist = new UserToollist();
            userToollist.userid=paras.userid;
            userToollist.toolname=paras.toolname;
            userToollist.number=paras.number;

            _context.UserToollist.Add(userToollist);
            await _context.SaveChangesAsync();


            ResultCreateActivity result = new ResultCreateActivity();
            result.message = "已成為朋友";

            return Ok(result);
        }

        [HttpPost("GetToollist")]
        public async Task<ActionResult<IEnumerable<ResultGetUserToolList>>> GetToollist(ParasUserFriendList paras)
        {
            List<UserToollist> toollists = new List<UserToollist>();
            var usertoollist = await _context.UserToollist.Where(f=>f.userid==paras.userid).ToListAsync();

            foreach (var tools in usertoollist)
            {
                UserToollist tool = new UserToollist();

                tool.userid = tools.userid;
                tool.toolname = tools.toolname;
                tool.number = tools.number;
                tool.id = tools.id;

                toollists.Add(tool);
            }

            return Ok(toollists);
        }


        [HttpPost("UseTool")]
        public async Task<ActionResult<IEnumerable<Friendlist>>> UseTool(ParasUseTool paras)
        {
            ResultCreateActivity result = new ResultCreateActivity();

            if (paras.toolname.Trim()== "解鎖五張用戶卡")
            {
                var UserIds = await _context.User
                .Where(f => f.id != paras.userid)
                .Select(f => f.id)
                .ToListAsync();


                var friendIds = await _context.Friendlist
               .Where(f => f.userid == paras.userid)
               .Select(f => f.friendid)
               .ToListAsync();

                var nonFriendUserIds = UserIds.Except(friendIds).Take(5).ToList();

                foreach (var ids in nonFriendUserIds)
                {

                    Friendlist friendlist = new Friendlist();
                    friendlist.userid = paras.userid;
                    friendlist.friendid= ids;

                    _context.Friendlist.Add(friendlist);
                    await _context.SaveChangesAsync();
                }

                var toolcard = await _context.UserToollist
                .Where(f => f.userid == paras.userid && f.toolname.Trim() == "解鎖五張用戶卡")
                .FirstOrDefaultAsync();

                if (toolcard != null)
                {
                    _context.UserToollist.Remove(toolcard);
                    await _context.SaveChangesAsync();
                }


                result.message = "ok";

            }


            return Ok(result);
        }

        [HttpPost("GetVideoUser")]
        public async Task<ActionResult> GetVideoUser(ParasGetVideoUser paras)
        {

            //string retrievedValue;

            //if (Userlist.TryGetValue(paras.connectionId, out retrievedValue))
            //{                
            //    Console.WriteLine($"对应的值是：{retrievedValue}");
            //}
            //else
            //{
               
            //}


            ResultCreateActivity result = new ResultCreateActivity();
            result.message = "已成為朋友";

            return Ok(result);
        }


        [HttpPost("GetUserMessage")]
        public async Task<ActionResult<IEnumerable<ResultGetUserToolList>>> GetUserMessage(ParasUserFriendList paras)
        {
            List<UserMessage> messagelists = new List<UserMessage>();
            var usermessagelist = await _context.UserMessage.Where(f => f.receiverid == paras.userid).ToListAsync();

            foreach (var message in usermessagelist)
            {
                UserMessage msg = new UserMessage();

                msg.name = message.name;
                msg.datetime = message.datetime;
                msg.message = message.message;
                msg.title = message.title;


                messagelists.Add(msg);
            }

            return Ok(messagelists);
        }

        [HttpPost("GetNotificationMessage")]
        public async Task<ActionResult<IEnumerable<ResultGetUserToolList>>> GetNotificationMessage(ParasUserFriendList paras)
        {
            List<Notification> notification = new List<Notification>();
            var notificationlist = await _context.Notification.Where(f => f.userid == paras.userid && f.alreadyread=="N").ToListAsync();

            foreach (var message in notificationlist)
            {
                Notification msg = new Notification();

                msg.userid = message.userid;
                msg.date = message.date;
                msg.time = message.time;
                msg.title = message.title;
                msg.eventid = message.eventid;
                msg.message = message.message;
                msg.alreadyread = message.alreadyread;
                msg.senderid = message.senderid;
                msg.Id = message.Id;
                notification.Add(msg);

                var connections = _userConnectionManager.GetUserConnections(message.userid.ToString());

                if (connections != null && connections.Count > 0)
                {
                    foreach (var connectionId in connections)
                    {
                        await _notificationUserHubContext.Clients.Client(connectionId).SendAsync("activityinvitation", message.title, message.message + "," + message.date.ToString("yyyy/MM/dd") + " " + message.time, message.eventid, message.senderid);
                    }
                }
            }

            return Ok(notification);
        }

    }
}
