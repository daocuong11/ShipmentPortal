using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace IdentityServer.Services
{
    public class IdentityWithAdditionalClaimsProfileService : IProfileService
    {
        public IdentityWithAdditionalClaimsProfileService()
        {
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectClaims = context.Subject.Claims.ToList();
            
            var email = subjectClaims.Find(c => c.Type == "emails") ?? subjectClaims.Find(c => c.Type == JwtClaimTypes.PreferredUserName);
            var claims = new List<Claim>
            {
                subjectClaims.Find(c => c.Type == JwtClaimTypes.Name),
                new Claim(JwtClaimTypes.PreferredUserName, email.Value),
                new Claim(JwtClaimTypes.Email, email.Value)
            };

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var identity = context.Subject.Identity;
            context.IsActive = identity != null && identity.IsAuthenticated;
        }
    }
}