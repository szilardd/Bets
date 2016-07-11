using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bets.Data.Models;
using System.Web.Security;
using Bets.Helpers;
using Bets.Data;
using System.IO;
using System.Drawing;
using ThatAuthentication;
using Bets.Infrastructure;

namespace Bets.Controllers
{
    public class ProfileController : BaseController
    {
		public ProfileController() : base(Module.Profile) 
		{
			this.name = "Profile";
		}

		protected override void SetIndexData()
		{
			base.SetIndexData();

			var userRepo = new UserRepository((int)Membership.GetUser().ProviderUserKey);
			string username = Membership.GetUser().UserName;

			ViewData.Model = userRepo.GetUser(username);
		}

		[HttpPost]
		public ActionResult Detail(UserModel user, HttpPostedFileBase file)
		{
			var userRepo = new UserRepository((int)Membership.GetUser().ProviderUserKey);

            if (!String.IsNullOrEmpty(user.OldPassword))
                new AuthenticationManager().HashPasswords(userRepo, user);

			var result = userRepo.SaveItem(user, DBActionType.Update);
			var loggedinUser = Membership.GetUser() as ThatMembershipUser;

			if (file != null && file.ContentLength > 0)
			{
                var extension = Path.GetExtension(file.FileName).ToLower().Replace(".", "");

                var allowedExtensions = new string[] { "jpg", "jpeg", "png" };

                if (!allowedExtensions.Contains(extension))
                {
                    result.Success = false;
                    result.Message = String.Format("Only images of type {0} extensions are allowed", String.Join(", ", allowedExtensions));
                }
                else
                {
                    string username = loggedinUser.UserName;
                    HttpPostedFileBase pictureFile = Request.Files[0];

                    if (!this.SaveProfilePicture(file, Server.MapPath(Data.UIExtensions.ImageRoot + "profile"), username))
                    {
                        result.Success = false;
                        result.Message = "An error occurred. The image was not saved!";
                    }
                }
			}

			//refresh session data
			if (result.Success)
				loggedinUser.DisplayName = user.DisplayName;

			TempData[Config.ActionStatus] = result;

			return RedirectToAction("Index");
		}

		private bool SaveProfilePicture(HttpPostedFileBase pictureFile, string path, string username)
		{
			try
			{
				string destinationPath = Path.Combine(path, username + ".jpg");
                FileInfo fileInfo = new FileInfo(destinationPath);
                var fullImagePath = destinationPath.Replace(fileInfo.Extension, "") + "_full" + fileInfo.Extension;

                if (!Directory.Exists(fileInfo.DirectoryName))
					Directory.CreateDirectory(fileInfo.DirectoryName);

                if (System.IO.File.Exists(destinationPath))
                {
                    System.IO.File.Delete(destinationPath);
                }

                if (System.IO.File.Exists(fullImagePath))
                {
                    System.IO.File.Delete(fullImagePath);
                }

                // save original image
                pictureFile.SaveAs(fullImagePath);

				//resize image
				using (Image image = Image.FromStream(pictureFile.InputStream))
				{
                    // if size matches config, save without resize
                    if (image.Size.Width == DataConfig.ProfileImageWidth && image.Size.Height == DataConfig.ProfileImageHeight)
                    {
                        image.Save(destinationPath, image.RawFormat);
                    }
                    else
                    {
                        using (Image resizedImage = new Bitmap(image, DataConfig.ProfileImageWidth, DataConfig.ProfileImageHeight))
                        {
                            resizedImage.Save(destinationPath, image.RawFormat);
                        }
                    }
				}

				return true;
			}
			catch (Exception ex) {
                Logger.Log(ex);
				return false; 
			}
		}
    }
}
