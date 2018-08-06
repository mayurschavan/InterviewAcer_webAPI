using InterviewAcer.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace InterviewAcer.AuthRepository
{
    public class AuthRepository : IDisposable
    {
        private AuthContext.AuthContext _ctx;

        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public AuthRepository()
        {
            _ctx = new AuthContext.AuthContext();
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_ctx));
            _userManager.UserValidator = new UserValidator<ApplicationUser>(_userManager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_ctx));
        }

        public async Task<IdentityResult> RegisterUser(UserModel userModel)
        {
            ApplicationUser user = new ApplicationUser
            {
                Name = userModel.Name,
                UserName = userModel.Email,
                Email = userModel.Email,
                EmailConfirmed = true,
                LicenseKey = userModel.LicenseKey
            };

            var result = await _userManager.CreateAsync(user, userModel.Password);

            return result;
        }



        public async Task<IdentityResult> SavePersonalInfo(UserPersonalInfo personalInfo)
        {
            var user = _userManager.FindById(personalInfo.UserId);
            user.PhoneNumber = personalInfo.MobileNumber;
            user.UniversityName = personalInfo.UniversityName;
            user.Specialization = personalInfo.Specialization;
            user.AcadamicScore = personalInfo.AcadamicScore;
            user.CountryCode = personalInfo.CountryCode;
            user.Name = personalInfo.Name;
             var identityUser = await _userManager.UpdateAsync(user);
            _ctx.SaveChanges();
            return identityUser;
        }

        //public async Task<IdentityResult> ChangeEmailAddress(string email, string userId)
        //{
        //    try
        //    {
        //        var user = _userManager.FindById(userId);
        //        user.Email = email;
        //        user.UserName = email;
        //        var identityUser = await _userManager.UpdateAsync(user);
        //        _ctx.SaveChanges();
        //        return identityUser;
        //    }
        //    catch(Exception e)
        //    {
        //        throw e;
        //    }

        //}

        public async Task<bool> ChangeEmailAddress(string email, string userId)
        {
            try
            {
                var user = _userManager.FindById(userId);
                user.Email = email;
                user.UserName = email;
                var identityUser = await _userManager.UpdateAsync(user);
                _ctx.SaveChanges();
                if (identityUser.Succeeded)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public async Task<IdentityResult> SaveProfilePicture(string userId, string profilePicturePath)
        {
            var user = _userManager.FindById(userId);
            user.ProfilePicture = profilePicturePath;
            var identityUser = await _userManager.UpdateAsync(user);
            _ctx.SaveChanges();
            return identityUser;
        }

        public async Task<IdentityRole> GetRole(string Id)
        {
            return await _roleManager.FindByIdAsync(Id);
        }


        public async Task<ApplicationUser> FindUser(string userName, string password)
        {
            ApplicationUser user = await _userManager.FindAsync(userName, password);
            return user;
        }

        public async Task<ApplicationUser> FindUser(string userName)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(userName);
            return user;
        }

        public async Task<ApplicationUser> FindUserById(string userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            return user;
        }

        public async Task<IdentityResult> ResetPassword(string password, string userId)
        {
            _userManager.RemovePassword(userId);
            return await _userManager.AddPasswordAsync(userId, password);
        }



        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();
        }
    }
}