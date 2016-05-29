using System;
using System.Web;
using System.Web.Security;
using System.Collections.Specialized;
using System.Security.Cryptography;
using Bets.Data;
using Bets.Data.Models;
using Bets.Helpers;
using System.Web.Mvc;
using Bets;

namespace ThatAuthentication
{
	public class ThatMembershipProvider : MembershipProvider
	{
		private const string ProviderName = "CustomMembershipProvider";

		// Properties from web.config, default all to False
		private string applicationName;
		private bool enablePasswordReset;
		private bool enablePasswordRetrieval = false;
		private bool requiresQuestionAndAnswer = false;
		private bool requiresUniqueEmail = true;
		private int maxInvalidPasswordAttempts;
		private int passwordAttemptWindow;
		private int minRequiredPasswordLength;
		private int minRequiredNonalphanumericCharacters;
		private string passwordStrengthRegularExpression;
		private MembershipPasswordFormat passwordFormat;

		private IUserRepository userRepo;

		private IUserRepository UserRepo
		{
			get
            {
                if (userRepo == null)
                {
                    userRepo = DependencyResolver.Current.GetService<IUserRepository>();
                }

                return userRepo;
            }
			set { userRepo = value; }
		}

		public ThatMembershipProvider()
        {
        }

		private string GetConfigValue(string configValue, string defaultValue)
		{
			if (string.IsNullOrEmpty(configValue))
				return defaultValue;

			return configValue;
		}

		public static string CreateSalt()
		{
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			byte[] buff = new byte[32];
			rng.GetBytes(buff);

			return Convert.ToBase64String(buff);
		}

		private MembershipUser GetMembershipUser(string username)
		{
			UserModel user;

            //if user data is saved into the session, retrieve it from there
			if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionKey.User.ToString()] != null)
				user = (UserModel)HttpContext.Current.Session[SessionKey.User.ToString()];
            //otherwise retrieve from database
			else
				user = UserRepo.GetUser(username);

			if (user == null)
				return null;

			var member = new ThatMembershipUser
			(
				ProviderName,
				user.Username,
				user.ID,
				user.Email,
				"",
				"",
				user.Active,
				user.IsLockedOut,
				user.CreatedDate,
				user.LastLoginDate,
				DateTime.Now,
				DateTime.Now,
				DateTime.Now,
				user.Role,
				user.DisplayName
			);

			return member;
		}

		public override string ApplicationName
		{
			get { return applicationName; }
			set { applicationName = value; }
		}

		public override void Initialize(string name, NameValueCollection config)
		{
			if (config == null)
				throw new ArgumentNullException(nameof(config));

			if (name == null || name.Length == 0)
				name = "CustomMembershipProvider";

			if (String.IsNullOrEmpty(config["description"]))
			{
				config.Remove("description");
				config.Add("description", "Custom Membership Provider");
			}

			base.Initialize(name, config);

			this.applicationName = GetConfigValue(config["applicationName"], System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
			this.maxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
			this.passwordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
			this.minRequiredNonalphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredNonalphanumericCharacters"], "1"));
			this.minRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "6"));
			this.enablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
			this.enablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "false"));
			this.passwordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));
			this.requiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));
			this.passwordFormat = (MembershipPasswordFormat)Enum.Parse(typeof(MembershipPasswordFormat), GetConfigValue(config["passwordFormat"], MembershipPasswordFormat.Clear.ToString()));
		}

		public override bool ChangePassword(string username, string oldPassword, string newPassword)
		{
			throw new NotImplementedException();
		}

		public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
		{
			return false;
		}

		public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
		{
			ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, password, true);

			OnValidatingPassword(args);

			if (args.Cancel)
			{
				status = MembershipCreateStatus.InvalidPassword;
				return null;
			}

			if (RequiresUniqueEmail && !String.IsNullOrEmpty(GetUserNameByEmail(email)))
			{
				status = MembershipCreateStatus.DuplicateEmail;
				return null;
			}

			MembershipUser u = GetUser(username, false);

			if (u == null)
			{
				var userModel = new UserModel { Username = username, Email = email };

				userModel.PasswordSalt = (passwordFormat == MembershipPasswordFormat.Hashed) ? CreateSalt() : "";
				userModel.Password = new AuthenticationManager().GetFinalPassword(passwordFormat, password, userModel.PasswordSalt);

				bool userCreated = UserRepo.CreateUser(userModel);

				if (userCreated)
				{
					status = MembershipCreateStatus.Success;
					return GetUser(username, false);
				}
				else
					status = MembershipCreateStatus.ProviderError;
			}
			else
				status = MembershipCreateStatus.DuplicateUserName;

			return null;
		}

		public override bool DeleteUser(string username, bool deleteAllRelatedData)
		{
			throw new NotImplementedException();
		}

		public override bool EnablePasswordReset
		{
			get { return enablePasswordReset; }
		}

		public override bool EnablePasswordRetrieval
		{
			get { return enablePasswordRetrieval; }
		}

		public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			throw new NotImplementedException();
		}

		public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			throw new NotImplementedException();
		}

		public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
		{
			throw new NotImplementedException();
		}

		public override int GetNumberOfUsersOnline()
		{
			throw new NotImplementedException();
		}

		public override string GetPassword(string username, string answer)
		{
			throw new NotImplementedException();
		}

		public override MembershipUser GetUser(string username, bool userIsOnline)
		{
			return this.GetMembershipUser(username);
		}

		public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
		{
			throw new NotImplementedException();
		}

		public override string GetUserNameByEmail(string email)
		{
			return UserRepo.GetUserNameByEmail(email);
		}

		public override int MaxInvalidPasswordAttempts
		{
			get { return maxInvalidPasswordAttempts; }
		}

		public override int MinRequiredNonAlphanumericCharacters
		{
			get { return minRequiredNonalphanumericCharacters; }
		}

		public override int MinRequiredPasswordLength
		{
			get { return minRequiredPasswordLength; }
		}

		public override int PasswordAttemptWindow
		{
			get { return passwordAttemptWindow; }
		}

		public override MembershipPasswordFormat PasswordFormat
		{
			get { return passwordFormat; }
		}

		public override string PasswordStrengthRegularExpression
		{
			get { return passwordStrengthRegularExpression; }
		}

		public override bool RequiresQuestionAndAnswer
		{
			get { return requiresQuestionAndAnswer; }
		}

		public override bool RequiresUniqueEmail
		{
			get { return requiresUniqueEmail; }
		}

		public override string ResetPassword(string username, string answer)
		{
			throw new NotImplementedException();
		}

		public override bool UnlockUser(string userName)
		{
			throw new NotImplementedException();
		}

		public override void UpdateUser(MembershipUser user)
		{
			throw new NotImplementedException();
		}

		public override bool ValidateUser(string username, string password)
		{
            var modelUser = new AuthenticationManager().ValidateLogin(UserRepo, username, password, passwordFormat);
            var valid = (modelUser != null);

            //if user is online, store the user data into session
            if (valid && HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session[SessionKey.User.ToString()] = modelUser;
                HttpContext.Current.Session[SessionKey.MaxBonusPerMatch.ToString()] = new SettingsRepository().GetItem(0).MaxBonusPerMatch;
            }

			return valid;
		}
    }
}