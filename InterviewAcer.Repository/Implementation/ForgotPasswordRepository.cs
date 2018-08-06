using InterviewAcer.Common.DTO;
using InterviewAcer.Data;

using InterviewAcer.Repository.Contract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewAcer.Repository.Implementation
{
    public class ForgotPasswordRepository
    {

        private InterviewAcerDbContext _dbContext;
        public ForgotPasswordRepository(InterviewAcerDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public string SaveOTP(ForgotPasswordDTO forgotPassword)
        {
            var forgotPasswordItem = _dbContext.ForgotPasswords.FirstOrDefault(x => x.UserId == forgotPassword.UserId);
            if (forgotPasswordItem == null)
            {
                var newforgotPasswordItem = new ForgotPassword();
                newforgotPasswordItem.CreatedDate = forgotPassword.TokenCreationDate;
                newforgotPasswordItem.OTP = forgotPassword.OTP;
                newforgotPasswordItem.UserId = forgotPassword.UserId;
                _dbContext.ForgotPasswords.Add(newforgotPasswordItem);
                return forgotPassword.OTP;
            }
            else
            {
                if (forgotPasswordItem.CreatedDate.AddMinutes(30) > DateTime.Now)
                {
                    return forgotPasswordItem.OTP;
                }
                else
                {
                    forgotPasswordItem.OTP = forgotPassword.OTP;
                    forgotPasswordItem.CreatedDate = forgotPassword.TokenCreationDate;
                    return forgotPassword.OTP;
                }
            }
        }

        public async Task<bool> VerifyOTP(string OTP, string userId)
        {
            var forgotPasswordItem = await _dbContext.ForgotPasswords.FirstOrDefaultAsync(x => x.UserId == userId && x.OTP == OTP);
            if(forgotPasswordItem != null)
            {
                if(forgotPasswordItem.CreatedDate.AddMinutes(30) > DateTime.Now)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
