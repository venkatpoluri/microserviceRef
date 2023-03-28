namespace SupplizeIdentityAuthorization
{
    using Amazon;
    using Amazon.SecretsManager;
    using Amazon.SimpleSystemsManagement;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Adds SupplizeAuthorization service to apply permissions based on the current user's roles.
    /// </summary>
    public static class SupplizeAuthorizationServiceRegistration
    {
        /// <summary>
        /// Adds SupplizeAuthorization service to enable permissions based on the current user's roles.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns> <see cref="SupplizeAuthorizationInitializer"/> </returns>
        public static SupplizeAuthorizationInitializer AddSupplizeAuthorization(this IServiceCollection services)
        {
             return new SupplizeAuthorizationInitializer(services, new AmazonSimpleSystemsManagementClient(RegionEndpoint.USEast1), new AmazonSecretsManagerClient(RegionEndpoint.USEast1));
        }
    }


}

