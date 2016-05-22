using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bets.Data.Models;
using System.IO;

namespace Bets.Data
{
	public class UserRepository : Repository<User, UserModel>, IUserRepository
	{
		public UserRepository() : base() { }
		public UserRepository(int userID) : base(userID) { }
		public UserRepository(BetsDataContext context, int userID) : base(context, userID) { }

		public IQueryable<User> GetActiveUsers()
		{
			return this.GetAll().Where(e => e.Username != "admin");
		}

		public bool CreateUser(UserModel userModel)
		{
			User u = new User();
			u.Username = userModel.Username;
			u.Password = userModel.Password;
			u.Email = userModel.Email;

			this.Add(u);
			return (this.Save() == StoredProcResult.Success);
		}

		public string GetUserNameByEmail(string email)
		{
			return
			(
				from user in this.Context.Users
				where user.Email == email
				select user.Username
			).SingleOrDefault();
		}

		public UserModel GetUser(string username)
		{
			var userModel =
			(
				from user in this.GetAll()
				where user.Username == username
				select new UserModel
						{
							ID = user.UserID,
							ProfileID = user.ProfileID,
							Username = user.Username,
							Password = user.Password,
                            PasswordSalt = user.PasswordSalt,
							Email = user.Email,
							DisplayName = user.DisplayName,
							Active = true,
							IsLockedOut = false,
							LastLoginDate = DateTime.Now
						}
			).SingleOrDefault();

			if (userModel != null)
				userModel.Role = Convert.ToInt32(userModel.ProfileID).ToEnum<Role>();

			return userModel;
		}

		public override ActionStatus SaveItem(UserModel model, DBActionType action)
		{
			return this.CallStoredProcedure
			(
				action,
				() =>
				{
					var result = this.Context.UpdateUser(this.UserID, model.DisplayName, model.Email, model.OldPassword, model.Password);
					return new SPResult { Result = result };
				}
			);
		}

        public string GetUserPasswordSalt()
        {
            return this.Context.Users.Where(e => e.UserID == this.UserID).Select(e => e.PasswordSalt).First();
        }
    }
}