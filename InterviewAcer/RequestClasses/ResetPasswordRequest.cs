using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InterviewAcer.RequestClasses
{
    public class ResetPasswordRequest
    {
        [Required]
        [StringLength(100, ErrorMessage =
            "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage =
           "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string OTP { get; set; }
    }

    public class ChangePasswordRequest
    {
        [Required]
        [StringLength(100, ErrorMessage =
            "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage =
           "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}