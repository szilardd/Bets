using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bets.Data.Models;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using DataAnnotationsExtensions;
using System.Web.Mvc;

namespace Bets.Data.Models
{
	[Serializable]
	public enum Role
	{
		Guest = 0,
		User = 1,
		Admin = 2
	}

	[Serializable]
	public class UserModel : Model, ISerializable
	{
		public Role Role { get; set; }

		public int ProfileID { get; set; }

		[DisplayName("Is Mobile User")]
		public bool IsMobileUser { get; set; }

		[DisplayName("Profile")]
		public string ProfileName { get; set; }

		[Required]
		public string Username { get; set; }

		[Required] [StringLength(50)] [DisplayName("First Name")]
		public string FirstName { get; set; }

		[Required] [StringLength(50)] [DisplayName("Last Name")]
		public string LastName { get; set; }

		[DisplayName("Repeat new password")] 
		[RequiredIfNot("Password", "", ErrorMessage = "Please repeat the new password")]
		[StringLength(50)]
		public string Password2 { get; set; }

		[DisplayName("New password")]
		[StringLength(50)]
		[System.ComponentModel.DataAnnotations.Compare("Password2", ErrorMessage = "New passwords must match")]
		[RequiredIfNot("OldPassword", "", ErrorMessage = "Please enter the new password")]
		public string Password { get; set; }

		[DisplayName("Old Password")] [RequiredIfNot("Password", "", ErrorMessage = "Please enter the old password too")]
		[StringLength(50)]
		public string OldPassword { get; set; }
		public string PasswordSalt { get; set; }

        [Email]
		public string Email { get; set; }
		public bool Active { get; set; }

		public bool IsLockedOut { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime LastLoginDate { get; set; }

		[DisplayName("Display Name")] [Required]
		public string DisplayName { get; set; }

		public int Points { get; set; }
		public int? RoundID { get; set; }
		public int MatchID { get; set; }

		public UserModel()
		{
		}

		public UserModel(SerializationInfo info, StreamingContext context)
		{
			ID = (int)info.GetValue("ID", typeof(int));
			ProfileID = (int)info.GetValue("ProfileID", typeof(int));
			Role = (Role)info.GetValue("Role", typeof(Role));
			Username = (string)info.GetValue("Username", typeof(string));
			Password = (string)info.GetValue("Password", typeof(string));
			PasswordSalt = (string)info.GetValue("PasswordSalt", typeof(string));
			DisplayName = (string)info.GetValue("DisplayName", typeof(string));
			Email = (string)info.GetValue("Email", typeof(string));
			Active = (bool)info.GetValue("Active", typeof(bool));
			IsLockedOut = (bool)info.GetValue("IsLockedOut", typeof(bool));
			CreatedDate = (DateTime)info.GetValue("CreatedDate", typeof(DateTime));
			LastLoginDate = (DateTime)info.GetValue("LastLoginDate", typeof(DateTime));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("ID", ID);
			info.AddValue("ProfileID", ProfileID);
			info.AddValue("Role", Role);
			info.AddValue("Username", Username);
			info.AddValue("Password", Password);
			info.AddValue("PasswordSalt", PasswordSalt);
			info.AddValue("DisplayName", DisplayName);
			info.AddValue("Email", Email);
			info.AddValue("Active", Active);
			info.AddValue("IsLockedOut", IsLockedOut);
			info.AddValue("CreatedDate", CreatedDate);
			info.AddValue("LastLoginDate", LastLoginDate);
		}
	}
}