using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bets.Data.Models;

namespace Bets.Data
{
	public interface IUserRepository
	{
		bool CreateUser(UserModel userModel);
		string GetUserNameByEmail(string email);
		UserModel GetUser(string username);
	}
}