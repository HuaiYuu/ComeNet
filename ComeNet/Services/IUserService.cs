using AWSWEBAPP.Models;
using ComeNet.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AWSWEBAPP.Services
{
    public interface IUserService
    {
        Jwt Authenticate(string username, string password);
    }   
}
