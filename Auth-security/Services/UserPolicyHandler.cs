using System.Linq;
namespace SupplizeIdentityAuthorization.Services
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using HeimGuard;
    using SupplizeDomainAuthorization.Services;
    using SupplizeDomainAuthorization.Databases;
    using System.Net;
    using Microsoft.IdentityModel.Logging;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using SupplizeDomainAuthorization.Domain.Permissions;

    public class UserPolicyHandler : IUserPolicyHandler
    {
        private readonly SupplizeDomainAuthorizationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public UserPolicyHandler(SupplizeDomainAuthorizationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<string>> GetUserPermissions()
        {

            Console.WriteLine("Check User Policy");
            var user = _currentUserService.User;
            // string accessToken = _currentUserService.HttpContextAccessor.HttpContext.Request.Cookies["access_token"];
            // string idToken = _currentUserService.HttpContextAccessor.HttpContext.Request.Cookies["id_token"];
            // Console.WriteLine("ACCESS TOKEN " + accessToken);
            // Console.WriteLine("ID TOKEN " + idToken);
            foreach (var entry in user?.Claims)
            {
                Console.WriteLine(entry.Type + " " + entry.Value);
            }
            // tenant is used to differentiate between custom attributes and roles
            var tenant = _currentUserService.CurrentTenant;
            Console.WriteLine("Check User Policy - Tenant " + tenant);
            if (user == null || user.Claims.Count() == 0 || String.IsNullOrEmpty(tenant))
            {
                Console.WriteLine("Check User Policy - UNAUTHORIZED user has " + user?.Claims.Count()+" claims");
                throw new HttpRequestException(null, null, HttpStatusCode.Unauthorized);
            }


            var tenantRoles = user.Claims.Where(c => c.Type.Contains(tenant))?.Single();
            Console.WriteLine("Check User Policy - Tenant Roles " + tenantRoles);
            if (tenantRoles == null || tenantRoles.Value.Count() == 0)
            {
                throw new HttpRequestException(null, null, HttpStatusCode.Unauthorized);
            }

            string[] roles = tenantRoles.Value.Split(",");
            Console.WriteLine("Check User Policy - Roles " + roles.Count());
            try
            {
                var roleIds = _dbContext.Roles.Where(r => roles.Contains(r.Name)).Select(r => r.Id).Distinct().ToArray();
                Console.WriteLine("ROLES IDS " + roleIds.Count());
                var permissionIds = _dbContext.RolePermissions.Where(rp => roleIds.Contains(rp.RoleId)).Select(rp => rp.PermissionId).Distinct().ToArray();
                Console.WriteLine("PERMS IDS " + permissionIds.Count());
                var permissionsList = _dbContext.Permissions.Where(n => permissionIds.Contains(n.Id)).ToList().Distinct().Select(n => n.Name).ToArray();
                Console.WriteLine("PERMS LIST " + permissionsList.Count());
                return await Task.FromResult(permissionsList);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }

    }
}
