﻿using System;
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

    public class ParasUserFriendList
    {
        public int userid { get; set; }       
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

	







	[Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ComeNetContext _context;
        private readonly HttpClient _httpClient;
        private IPasswordHashService _passwordHashService;
        private IUserService _userService;

        public UsersController(ComeNetContext context, HttpClient httpClient, IPasswordHashService passwordHashService, IUserService userService)
        {
            _context = context;
            _httpClient = httpClient;
            _passwordHashService = passwordHashService;
            _userService = userService;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
          if (_context.User == null)
          {
              return NotFound();
          }
            return await _context.User.ToListAsync();
        }

       
       

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
          if (_context.User == null)
          {
              return Problem("Entity set 'ComeNetContext.User'  is null.");
          }
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.id }, user);
        }


		[HttpPost("signup")]
		[EnableCors("AllowDomainA")]
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

		[HttpPost("signin")]
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
                    HttpContext.Session.SetString("email", email);


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



        [HttpPost("friendlist")]
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




        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (_context.User == null)
            {
                return NotFound();
            }
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return (_context.User?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}