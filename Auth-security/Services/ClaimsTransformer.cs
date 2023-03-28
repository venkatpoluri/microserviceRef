namespace SupplizeIdentityAuthorization.Services
{
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Identity;
    using System.Threading.Tasks;

    public class ClaimsTransformer : IClaimsTransformation
    {
        public static List<Claim> jwtRoles;
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {

            var claims = new List<Claim>();
            var claimsPrincipal = new ClaimsPrincipal();
            var cognitoGroupClaims = principal.Claims.Where(t => t.Type == "cognito:groups").ToList();
            Console.WriteLine("Claims "+cognitoGroupClaims);
            foreach (var claim in cognitoGroupClaims)
            {
                var claim2 = new Claim("groups", claim.Value);
                claims.Add(claim2);
            }
            var cognitoRoleClaims = principal.Claims.Where(t => t.Type.Contains(":roles")).ToList();
            foreach (var claim in cognitoRoleClaims)
            {
                var claim2 = new Claim(claim.Type, claim.Value);
                claims.Add(claim2);
            }
            var cognitoRegionClaims = principal.Claims.Where(t => t.Type.Contains(":regions")).ToList();
            foreach (var claim in cognitoRegionClaims)
            {
                var claim2 = new Claim(claim.Type, claim.Value);
                claims.Add(claim2);
            }
            var userId = principal.Claims.Where(t => t.Type.Contains("cognito:username")).ToList();
            foreach (var user in userId)
            {
                var claim = new Claim(ClaimTypes.NameIdentifier, user.Value);
                claims.Add(claim);
            }
            jwtRoles = claims;
            if (claims.Count() > 0)
            {
                var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                return Task.FromResult(new ClaimsPrincipal(claimsIdentity));
            }

            return Task.FromResult(claimsPrincipal);
        }
    }
}
