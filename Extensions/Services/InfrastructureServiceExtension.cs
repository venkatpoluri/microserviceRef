namespace TradingPartnerManagement.Extensions.Services;

using TradingPartnerManagement.Databases;
using TradingPartnerManagement.Resources;
using Microsoft.EntityFrameworkCore;
using SupplizeDomainAuthorization.Databases;
using SupplizeDomainAuthorization.Domain.Concepts;


public static class ServiceRegistration
{
    public static void AddInfrastructure(this IServiceCollection services, IWebHostEnvironment env)
    {
        // DbContext -- Do Not Delete
        if (env.IsEnvironment(LocalConfig.FunctionalTestingEnvName))
        {
            services.AddDbContext<ConceptDbContext>(options =>
            {
                options.UseInMemoryDatabase($"suppliZe");
                options.EnableSensitiveDataLogging();

            });
                services.AddDbContext<TradingPartnerDbContext>(options => {
                options.UseInMemoryDatabase($"suppliZe");
                options.EnableSensitiveDataLogging();
            });
        }
        else
        {
            var initConnectionString = "";
            if(env.IsEnvironment(LocalConfig.IntegrationTestingEnvName))
            {
                var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
                initConnectionString = connectionString;
            }
            else
            {

                var region = Amazon.RegionEndpoint.GetBySystemName("us-east-1");
                var client = new Amazon.SimpleSystemsManagement.AmazonSimpleSystemsManagementClient(region);
                var connectionString = GetParameterValueAsync("dbConnectionString", client).Result;
                if (string.IsNullOrEmpty(connectionString))
                {
                    // this makes local migrations easier to manage. feel free to refactor if desired.
                    connectionString = env.IsDevelopment()
                        ? "Host=localhost;Port=63306;Database=dev_tradingpartnermanagement;Username=postgres;Password=postgres"
                        : throw new Exception("DB_CONNECTION_STRING environment variable is not set.");
                }
                initConnectionString = "Host=rsisupplize3-cluster.cluster-cjz2p6lz56l8.us-east-1.rds.amazonaws.com;Port=5432;Database=dev;Username=postgres;Password=Whopper2022!;SearchPath=core";
            }
            services.AddDbContext<ConceptDbContext>(options => options.UseNpgsql(initConnectionString))
.AddDbContext<TradingPartnerDbContext>(options =>
                options.AddInterceptors(new PostgresConceptDbConnectionInterceptor()).UseNpgsql(initConnectionString,
                    builder => builder.MigrationsAssembly(typeof(TradingPartnerDbContext).Assembly.FullName))
                            .UseSnakeCaseNamingConvention());

            
        }

        // Auth -- Do Not Delete
    }

    public static async Task<string> GetParameterValueAsync(string parameter, Amazon.SimpleSystemsManagement.IAmazonSimpleSystemsManagement client)
    {
        var response = await client.GetParameterAsync(new Amazon.SimpleSystemsManagement.Model.GetParameterRequest
        {
            Name = parameter,
            WithDecryption = true
        });

        return response.Parameter.Value;
    }
}
