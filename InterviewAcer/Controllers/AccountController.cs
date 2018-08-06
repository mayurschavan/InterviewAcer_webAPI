using InterviewAcer.Models;
using InterviewAcer.Repository.Contract;
using InterviewAcer.Repository.Implementation;
using InterviewAcer.RequestClasses;
using InterviewAcer.ResponseClasses;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace InterviewAcer.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private AuthRepository.AuthRepository _repo = null;
        private IUnitOfWork _unitOfWork { get; set; }
        public AccountController()
        {
            _repo = new AuthRepository.AuthRepository();
            _unitOfWork = new UnitOfWork();
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<HttpResponseMessage> Register(UserModel userModel)
        {
            if (!ModelState.IsValid)
            {
                var error = new ErrorResponse();
                error.Error = "Registration Failed";
                error.ErrorDescription = ModelState.Values.First().Errors.First().ErrorMessage;
                return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, error);
            }

            IdentityResult result = await _repo.RegisterUser(userModel);

            HttpResponseMessage errorResult = GetErrorResult(result);

            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                return await LoginUser(userModel.Email, userModel.Password);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public async Task<HttpResponseMessage> LoginUser(string username, string password)
        {
            // Invoke the "token" OWIN service to perform the login: /api/token
            // Ugly hack: I use a server-side HTTP POST because I cannot directly invoke the service (it is deeply hidden in the OAuthAuthorizationServerHandler class)
            var request = HttpContext.Current.Request;
            var tokenServiceUrl = request.Url.GetLeftPart(UriPartial.Authority) + request.ApplicationPath + "/Token";
            using (var client = new HttpClient())
            {
                var requestParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            };
                var requestParamsFormUrlEncoded = new FormUrlEncodedContent(requestParams);
                var tokenServiceResponse = await client.PostAsync(tokenServiceUrl, requestParamsFormUrlEncoded);
                var responseString = await tokenServiceResponse.Content.ReadAsStringAsync();
                var responseCode = tokenServiceResponse.StatusCode;
                var responseMsg = new HttpResponseMessage(responseCode)
                {
                    Content = new StringContent(responseString, Encoding.UTF8, "application/json")
                };
                return responseMsg;
            }
        }

        /// <summary>
        /// if user exists, sends an OTP to registered Email address
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>If user is found, userid is returned. If user is not found NotFound status code is returned</returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("FindUserAndSendOTP")]
        public async Task<IHttpActionResult> FindUser(string userName)
        {
            try
            {
                ApplicationUser user = await _repo.FindUser(userName);
                if (user == null)
                {
                    return NotFound();
                }
                else
                {
                    var OTP = _unitOfWork.GetForgotPasswordRepository().SaveOTP(new Common.DTO.ForgotPasswordDTO()
                    {
                        OTP = GetOTP(),
                        UserId = user.Id,
                        TokenCreationDate = DateTime.Now
                    });
                    await _unitOfWork.Save();
                    if (!string.IsNullOrEmpty(user.Email))
                        SendEmail(OTP, user.Email);
                    var userIdResponse = new UserIdResponse() { UserId = user.Id };
                    return Ok(userIdResponse);
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

        }

        [AllowAnonymous]
        [HttpGet]
        [Route("SendOTP")]
        public async Task<IHttpActionResult> SendOTP(string email, String UserId)
        {
            try
            {

                var OTP = _unitOfWork.GetForgotPasswordRepository().SaveOTP(new Common.DTO.ForgotPasswordDTO()
                {
                    OTP = GetOTP(),
                    UserId = UserId,
                    TokenCreationDate = DateTime.Now
                });
                await _unitOfWork.Save();
                if (!string.IsNullOrEmpty(email))
                    SendEmail(OTP, email);
                var userIdResponse = new UserIdResponse() { UserId = UserId };
                return Ok(userIdResponse);

            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

        }

        /// <summary>
        /// verifies the OTP entered from user and save email
        /// </summary>
        /// <param name="verifyOtpAndSaveEmail"></param>
        /// <returns>If OTP is valid status code 200 is returned. If OTP is not valid status code 404 is returned.</returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("VerifyOTPAndSaveEmail")]
        public async Task<IHttpActionResult> VerifyOTPAndSaveUser(VerifyOtpAndSaveUserRequest verifyOtpAndSaveEmail)
        {
            try
            {
                var isOTPValid = await _unitOfWork.GetForgotPasswordRepository().VerifyOTP(verifyOtpAndSaveEmail.OTP, verifyOtpAndSaveEmail.UserId);
                if (isOTPValid)
                {

                    var isSavedEmail=await _repo.ChangeEmailAddress(verifyOtpAndSaveEmail.Email, verifyOtpAndSaveEmail.UserId);
                    if (isSavedEmail)
                        return Ok("Email change successfully");
                    else
                        return Content(HttpStatusCode.NotFound, "Email id is already exist");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

        }

        private void SendEmail(string OTP, string userEmailAddress)
        {
            string fromAddress = ConfigurationManager.AppSettings["ResetPasswordMailFromAddress"];
            MailMessage mailMessage = new MailMessage(fromAddress, userEmailAddress);
            mailMessage.Body = "Your OTP for reseting the password is " + OTP;
            mailMessage.Subject = "Reset Password OTP";
            SmtpClient client = new SmtpClient();
            client.Send(mailMessage);
        }

        /// <summary>
        /// verifies the OTP entered from user
        /// </summary>
        /// <param name="verifyOtp"></param>
        /// <returns>If OTP is valid status code 200 is returned. If OTP is not valid status code 404 is returned.</returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("VerifyOTP")]
        public async Task<IHttpActionResult> VerifyOTP(VerifyOtpRequest verifyOtp)
        {
            try
            {
                var isOTPValid = await _unitOfWork.GetForgotPasswordRepository().VerifyOTP(verifyOtp.OTP, verifyOtp.UserId);
                if (isOTPValid)
                {
                    return Ok("OTP is valid");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

        }
        [AllowAnonymous]
        [HttpPost]
        [Route("ResetPassword")]
        /// <summary>
        /// Resets the password of the user.
        /// </summary>
        /// <param name="resetPasswordDetails"></param>
        /// <returns>returns 200 status code, if password reset is success</returns>
        public async Task<IHttpActionResult> ResetPassword(ResetPasswordRequest resetPasswordDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var isOTPValid = await _unitOfWork.GetForgotPasswordRepository().VerifyOTP(resetPasswordDetails.OTP, resetPasswordDetails.UserId);
                if (isOTPValid)
                    await _repo.ResetPassword(resetPasswordDetails.NewPassword, resetPasswordDetails.UserId);
                return Ok("password reset successfully");
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("SaveUserPersonalInformation")]
        public async Task<IHttpActionResult> SavePersonalInfo(UserPersonalInfo personalInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                IdentityResult result = await _repo.SavePersonalInfo(personalInfo);
                if (!result.Succeeded)
                {
                    if (result.Errors != null)
                    {
                        return BadRequest(result.Errors.First());
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return Ok();
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("UpdateEmailAddress")]
        [HttpGet]
        public async Task<IHttpActionResult> UpdateEmailAddress(string emailAddress, string userId)
        {
            var isSuccess = await _repo.ChangeEmailAddress(emailAddress, userId);
            if(isSuccess)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
            //return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("GetUserPeronalInformation")]
        public async Task<IHttpActionResult> GetPersonalInfo(string userId)
        {
            if (userId == null)
            {
                return BadRequest();
            }
            try
            {
                UserPersonalInfo personalInfo = new UserPersonalInfo();
                ApplicationUser user = await _repo.FindUserById(userId);
                if(user == null)
                {
                    return NotFound();
                }
                else
                {
                    personalInfo.UserId = userId;
                    personalInfo.Name = user.Name;
                    personalInfo.UniversityName = user.UniversityName;
                    personalInfo.MobileNumber = user.PhoneNumber;
                    personalInfo.Specialization = user.Specialization;
                    personalInfo.AcadamicScore = user.AcadamicScore;
                    personalInfo.CountryCode = user.CountryCode;
                    personalInfo.ProfileImagePath = user.ProfilePicture;
                    personalInfo.TotalScore = _unitOfWork.GetStageRepository().GetUserTotal(userId);
                }
                return Ok(personalInfo);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repo.Dispose();
            }

            base.Dispose(disposing);
        }

        private HttpResponseMessage GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    ErrorResponse error = new ErrorResponse();
                    error.Error = "Registration Failed";
                    error.ErrorDescription = result.Errors.First();
                    return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, error);
                }


                // No ModelState errors are available to send, so just return an empty BadRequest.
                return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest);
            }

            return null;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("ChangePassword")]
        /// <summary>
        /// chnage the password of the user.
        /// </summary>
        /// <param name="resetPasswordDetails"></param>
        /// <returns>returns 200 status code, if password change is success</returns>
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordRequest changePasswordDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _repo.ResetPassword(changePasswordDetails.NewPassword, changePasswordDetails.UserId);
                return Ok("password change successfully");
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        private string GetOTP()
        {
            var token = new StringBuilder();

            //Prepare a 6-character random text
            using (RNGCryptoServiceProvider
                                rngCsp = new RNGCryptoServiceProvider())
            {
                var data = new byte[4];
                for (int i = 0; i < 6; i++)
                {
                    //filled with an array of random numbers
                    rngCsp.GetBytes(data);
                    //this is converted into a character from A to Z
                    var randomchar = Convert.ToChar(
                                              //produce a random number 
                                              //between 0 and 25
                                              BitConverter.ToUInt32(data, 0) % 26
                                              //Convert.ToInt32('A')==65
                                              + 65
                                     );
                    token.Append(randomchar);
                }
            }
            //This will be the password change identifier 
            //that the user will be sent out
            return token.ToString();
        }

        [Authorize]
        [HttpPost]
        [Route("SaveProfilePicture")]
        public async Task<HttpResponseMessage> SaveProfilePicture()
        {
            // Check if the request contains multipart/form-data.  
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                var provider = await Request.Content.ReadAsMultipartAsync(new InMemoryMultipartFormDataStreamProvider());
                NameValueCollection formData = provider.FormData;

                string userId = string.Empty;
                foreach(var key in formData.AllKeys)
                {
                    if(key.ToLower() == "userid")
                    {
                        userId = formData[key];
                    }
                }
                if(string.IsNullOrWhiteSpace(userId))
                {
                    var res = string.Format("Please provide an user Id");
                    dict.Add("error", res);
                    return Request.CreateResponse(HttpStatusCode.NotFound, dict);
                }
                IList<HttpContent> files = provider.Files;
                HttpContent file1 = files[0];
                if (file1 != null)
                {
                    var fileName = file1.Headers.ContentDisposition.FileName.Trim('\"');
                    IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                    var ext = fileName.Substring(fileName.LastIndexOf('.'));
                    var extension = ext.ToLower();
                    if (!AllowedFileExtensions.Contains(extension))
                    {
                        var message = string.Format("Please Upload image of type .jpg,.gif,.png.");
                        dict.Add("error", message);
                        return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                    }
                    else
                    {
                        byte[] profilePicByteArray = await file1.ReadAsByteArrayAsync();
                        switch (extension)
                        {
                            case ".png":
                                using (Image image = Image.FromStream(new MemoryStream(profilePicByteArray)))
                                {
                                    image.Save(HttpContext.Current.Server.MapPath("~/Content/ProfilePictures/" + fileName), ImageFormat.Png);  // Or Png
                                }
                                break;
                            case ".jpg":
                                using (Image image = Image.FromStream(new MemoryStream(profilePicByteArray)))
                                {
                                    image.Save(HttpContext.Current.Server.MapPath("~/Content/ProfilePictures/" + fileName), ImageFormat.Jpeg);  // Or Png
                                }
                                break;
                            case ".gif":
                                using (Image image = Image.FromStream(new MemoryStream(profilePicByteArray)))
                                {
                                    image.Save(HttpContext.Current.Server.MapPath("~/Content/ProfilePictures/" + fileName), ImageFormat.Gif);  // Or Png
                                }
                                break;
                        }
                        IdentityResult result =  await _repo.SaveProfilePicture(userId, fileName);
                        if(!result.Succeeded)
                        {
                            dict.Add("error", result.Errors.FirstOrDefault());
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else
                        {
                            var response = Request.CreateResponse(HttpStatusCode.OK, new { ProfileImagePath = fileName });
                            return response;
                        }
                    }
                }
                else
                {
                    var res = string.Format("Please Upload a image.");
                    dict.Add("error", res);
                    return Request.CreateResponse(HttpStatusCode.NotFound, dict);
                }
            }
            catch(Exception e)
            {
                dict.Add("error", e.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, dict);
            }
        }


        public static ImageFormat GetImageFormat(string extension)
        {
            ImageFormat result = null;
            PropertyInfo prop = typeof(ImageFormat).GetProperties().Where(p => p.Name.Equals(extension, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (prop != null)
            {
                result = prop.GetValue(prop) as ImageFormat;
            }
            return result;
        }
        private MemoryStream CopyFileToMemory(string path)
        {
            MemoryStream ms = new MemoryStream();
            FileStream fs = new FileStream(path, FileMode.Open);
            fs.Position = 0;
            fs.CopyTo(ms);
            fs.Close();
            fs.Dispose();
            return ms;
        }

        [Authorize]
        [HttpGet]
        [Route("GetProfilePicture")]
        public HttpResponseMessage GetProfilePicture(string imagePath)
        {
            MemoryStream ms = null;
            HttpContext context = HttpContext.Current;
            string filePath = context.Server.MapPath(string.Concat("~/Content/ProfilePictures/", imagePath));
            string extension = Path.GetExtension(imagePath);
            if (File.Exists(filePath))
            {
                if (!string.IsNullOrWhiteSpace(extension))
                {
                    extension = extension.Substring(extension.IndexOf(".") + 1);
                }

                //If requested file is an image than load file to memory
                if (GetImageFormat(extension) != null)
                {
                    ms = CopyFileToMemory(filePath);
                }
            }

            if (ms == null)
            {
                extension = "png";
                ms = CopyFileToMemory(context.Server.MapPath("~/Content/ProfilePictures/fallback.png"));
            }

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(ms.ToArray());
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(string.Format("image/{0}", extension));
            return result;
        }

    }
}
