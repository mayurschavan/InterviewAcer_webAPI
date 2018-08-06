using InterviewAcer.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace InterviewAcer.AuthContext
{
    public class AuthContext : IdentityDbContext<ApplicationUser>
    {
        public AuthContext()
            : base("AuthContext")
        {

        }
    }
}