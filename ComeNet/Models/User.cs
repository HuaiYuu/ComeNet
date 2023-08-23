using AWSWEBAPP.Services;
using ComeNet.Data;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ComeNet.Models
{
    public class User
    {
        public int id { get; set; }
        public string provider { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string picture { get; set; }
        public string password { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }

	}
    public class Friendlist
    {
        public int id { get; set; }
        public int userid { get; set; }
        public int friendid { get; set; }

    }
    public class Jwt
    {
        public string access_token { get; set; }
        public int access_expired { get; set; }
        public UserView user { get; set; }
    }
    public class UserView
    {
        public int id { get; set; }
        public string provider { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string picture { get; set; }
    }
    public class UserProfile
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public PictureDataForm Picture { get; set; }
    }
    public class PictureDataForm
    {
        public PictureData data { get; set; }
    }
    public class PictureData
    {
        public int Height { get; set; }
        public bool IsSilhouette { get; set; }
        public string Url { get; set; }
        public int Width { get; set; }
    }
    public class UserService : IUserService
    {

        private readonly ComeNetContext _context;

        private IPasswordHashService _passwordHashService;

        private readonly IConfiguration _configuration;

        private readonly object _lock = new object();

        public UserService(IConfiguration configuration, ComeNetContext context, IPasswordHashService passwordHashService)
        {
            _configuration = configuration;
            _context = context;
            _passwordHashService = passwordHashService;
        }
        string secretKey;
        public void ReadJwtSettings()
        {
            secretKey = _configuration["Jwt:SecretKey"];
        }

        public Jwt Authenticate(string username, string password)
        {
            ReadJwtSettings();
            var useraccount = _context.User.Where(x => x.email == username && x.password == password).FirstOrDefault();
            if (useraccount == null) return null;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var jwt = new JwtSecurityToken(
                  claims: new List<Claim> { new Claim(ClaimTypes.Name, username) },
                  notBefore: DateTime.UtcNow,
                  expires: DateTime.UtcNow.AddHours(1),
                  signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
            Jwt jwtmodel = new Jwt();
            jwtmodel.access_token = tokenHandler.WriteToken(jwt);
            jwtmodel.access_expired = 3600;

            UserView userview = new UserView();
            userview.id = useraccount.id;
            userview.name = useraccount.name;
            userview.email = useraccount.email;
            userview.provider = useraccount.provider;
            userview.picture = useraccount.picture;

            jwtmodel.user = userview;
            return jwtmodel;
        }


        public class PasswordHashService : IPasswordHashService
        {
            public string HashPassword(string password)
            {

                byte[] salt = new byte[128 / 8]
                {
                 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A,0x05, 0x06, 0x07, 0x08, 0x09, 0x0A
                };
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));

                return hashed;
            }
        }
    }
	public class GeoCoordinate
	{
		public double Latitude { get; set; }
		public double Longitude { get; set; }
	}
	public static class GeoCalculator
	{
		private const double EarthRadiusKm = 6371.0;

		public static double CalculateDistance(GeoCoordinate point1, GeoCoordinate point2)
		{
			double lat1Rad = DegreeToRadian(point1.Latitude);
			double lon1Rad = DegreeToRadian(point1.Longitude);
			double lat2Rad = DegreeToRadian(point2.Latitude);
			double lon2Rad = DegreeToRadian(point2.Longitude);

			double deltaLat = lat2Rad - lat1Rad;
			double deltaLon = lon2Rad - lon1Rad;

			double a = Math.Pow(Math.Sin(deltaLat / 2), 2) +
					   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
					   Math.Pow(Math.Sin(deltaLon / 2), 2);

			double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

			return EarthRadiusKm * c;
		}

		private static double DegreeToRadian(double degree)
		{
			return degree * Math.PI / 180.0;
		}
	}
    public class Article
    {
        public string articleHeading { get; set; }
        public string articleContent { get; set; }
        public string userId { get; set; }
    }

	public class ChatContext
	{
		public string name { get; set; }
		public string message { get; set; }
		public string userId { get; set; }
	}
}
