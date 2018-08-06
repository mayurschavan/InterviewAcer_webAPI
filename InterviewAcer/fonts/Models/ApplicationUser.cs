using Microsoft.AspNet.Identity.EntityFramework;

namespace InterviewAcer.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string LicenseKey { get; set; }
        public string UniversityName { get; set; }
        public string CountryCode { get; set; }
        public string Specialization { get; set; }
        public string AcadamicScore { get; set; }
        public string ProfilePicture { get; set; }
    }
}