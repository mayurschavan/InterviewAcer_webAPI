using InterviewAcer.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InterviewAcer.AuthProvider
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            ApplicationUser user;
            IdentityRole userRole;
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            AuthenticationProperties authProperties;

            using (AuthRepository.AuthRepository _repo = new AuthRepository.AuthRepository())
            {
                user = await _repo.FindUser(context.UserName, context.Password);
                if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }
                else
                {
                    bool isAdmin = false;
                    var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                    identity.AddClaim(new Claim("sub", context.UserName));
                    if (user != null)
                    {
                        if (user.Roles.Any())
                        {
                            userRole = await _repo.GetRole(user.Roles.Select(x => x.RoleId).First());
                            if(userRole.Name == "Administrator")
                            {
                                isAdmin = true;
                            }
                            identity.AddClaim(new Claim(ClaimTypes.Role, userRole.Name));
                        }
                    }


                    authProperties = new AuthenticationProperties(new Dictionary<string, string>()
                    {
                        {
                            "Full Name", user.Name
                        },
                        {
                            "User Id", user.Id
                        },
                        {
                            "IsAdmin", isAdmin?"Yes":"No"
                        },
                        {
                            "ProfilePicture",user.ProfilePicture == null ? "NA":user.ProfilePicture
                            //"ProfilePicture","Hello"
                        }
                    });

                    
                    var authTickect = new AuthenticationTicket(identity, authProperties);
                    context.Validated(authTickect);
                }
            }
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                if (property.Key == "Full Name" || property.Key == "User Id"|| property.Key == "ProfilePicture" || property.Key == "IsAdmin")
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }
    }
}