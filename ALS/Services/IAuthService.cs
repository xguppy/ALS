using ALS.EntityСontext;

namespace ALS.Services
{
    public interface IAuthService
    {
        string GetAuthData(string email, User user);
        string GetHashedPassword(string pass);
        bool ValidateUserPassword(string hashed, string pass);
    }
}
