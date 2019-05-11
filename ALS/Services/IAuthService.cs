using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALS.EntityСontext;
using CryptoHelper;

namespace ALS.Services
{
    interface IAuthService
    {
        string GetAuthData(string email, User user);
        string GetHashedPassword(string pass);
        bool ValidateUserPassword(string hashed, string pass);
    }
}
