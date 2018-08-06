using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InterviewAcer.Models
{
    public class UserPersonalInfo
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        [Required]
        public string UniversityName { get; set; }
        public string CountryCode { get; set; }
        public string Specialization { get; set; }
        public string AcadamicScore { get; set; }
        public string MobileNumber { get; set; }
        public int TotalScore { get; set; }
        public string ProfileImagePath { get; set; }
    }
}