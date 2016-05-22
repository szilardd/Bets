using System.Web.Security;
using Bets.Data;
using Bets.Data.Models;

namespace Bets
{
    public class AuthenticationManager
    {
        private const string HashKey = "!0lBPk||gnT^uPx^VF%K69bpqi!";

        public UserModel ValidateLogin(IUserRepository userRepo, string username, string password, MembershipPasswordFormat passwordFormat)
        {
            var modelUser = userRepo.GetUser(username);

            if (modelUser == null)
                return null;

            var valid = (string.Compare(GetFinalPassword(passwordFormat, password, modelUser.PasswordSalt), modelUser.Password, false) == 0);

            if (valid)
            {
                return modelUser;
            }
            else
            {
                return null;
            }
        }

        public void HashPasswords(UserRepository userRepo, UserModel model)
        {
            var salt = userRepo.GetUserPasswordSalt();

            model.Password = CreatePasswordHash(model.Password, salt);
            model.OldPassword = CreatePasswordHash(model.OldPassword, salt);
        }

        public string GetFinalPassword(MembershipPasswordFormat passwordFormat, string password, string salt = "")
        {
            switch (passwordFormat)
            {
                case MembershipPasswordFormat.Hashed:

                    return CreatePasswordHash(password, salt);

                case MembershipPasswordFormat.Clear:

                    return password;

                default:

                    return null;
            }
        }

        private static string CreatePasswordHash(string password, string salt)
        {
            string saltAndPwd = string.Concat(password, HashKey, salt);
            return FormsAuthentication.HashPasswordForStoringInConfigFile(saltAndPwd, "sha1");
        }
    }

}