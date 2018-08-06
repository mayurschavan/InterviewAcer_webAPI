using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewAcer.Common.DTO
{
    public class ForgotPasswordDTO
    {
        public string UserId { get; set; }
        public DateTime TokenCreationDate { get; set; }

        public string OTP { get; set; }
    }
}
