namespace AWSWEBAPP.Services
{
    public interface IPasswordHashService
    {
        string HashPassword(string password);
    }
}
