namespace SupplizeIdentityAuthorization
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.IO;
    using Amazon;
    using Amazon.SecretsManager;
    using Amazon.SecretsManager.Model;
    using Json.Net;
    using HeimGuard;
    using SupplizeDomainAuthorization.Services;
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.IdentityModel.Tokens;
    using Amazon.SimpleSystemsManagement;
    using Amazon.SimpleSystemsManagement.Model;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.Extensions.Logging;
    using System.Net;
    using Newtonsoft.Json;
    using Microsoft.AspNetCore.Http;
    using SupplizeDomainAuthorization.Databases;
    using SupplizeIdentityAuthorization.Services;
    using Microsoft.AspNetCore.Authentication.Cookies;

    public class SupplizeAuthorizationInitializer
    {
        public IServiceCollection Services { get; set; }
        public IAmazonSimpleSystemsManagement _client { get; set; }
        public IAmazonSecretsManager _manager { get; set; }

        public SupplizeAuthorizationInitializer(IServiceCollection services,
            IAmazonSimpleSystemsManagement client, IAmazonSecretsManager manager)
        {
            Services = services;
            _client = client;
            _manager = manager;
            config();
        }

        private void config()
        {
            try
            {
                var authorityUrl = GetParameterValueAsync("cognitoAuthorityUrl").Result;
                var audeienceId = GetParameterValueAsync("cognitoAppClientId").Result;
                var userPoolId = GetParameterValueAsync("cognitoUserPoolId").Result;
                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
                Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.Authority = $"{authorityUrl}/{userPoolId}"; //with user pool Id
                    options.Audience = audeienceId; //user pool client Id;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidIssuer = $"{authorityUrl}/{userPoolId}",
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidAudience = audeienceId,
                        // IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) =>
                        // {
                        //     // get JsonWebKeySet from AWS
                        //     var json = new HttpClient().GetStringAsync(parameters.ValidIssuer + "/.well-known/jwks.json").Result;
                        //     var keys = JsonConvert.DeserializeObject<JsonWebKeySet>(json).Keys;
                        //     // cast the result to be the type expected by IssuerSigningKeyResolver
                        //     return (IEnumerable<SecurityKey>)keys;
                        // }
                    };
                  //  options.MetadataAddress = $"{authorityUrl}/{userPoolId}/.well-known/openid-configuration";
                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine("AUTHORIZER - AUTH FAILED");
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json; charset=utf-8";
                            var message = "An error occurred processing your authentication.";
                            var result = JsonConvert.SerializeObject(new { message });
                            return context.Response.WriteAsync(result);
                        },
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Cookies["id_token"];
                            return Task.CompletedTask;
                        },
                        OnForbidden = context =>
                        {
                            Console.WriteLine("AUTHORIZER - FORBIDDEN");
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            Console.WriteLine("TOKEN VALIDATED");
                            return Task.CompletedTask;
                        }
                    };
                });

              //  Services.AddScoped<IClaimsTransformation, ClaimsTransformer>();
                var trnas = new ClaimsTransformer();
              
                Services.AddDbContext<SupplizeDomainAuthorizationDbContext>(options =>
                {
                    options.UseNpgsql($"Host=rsisupplize3-cluster.cluster-cjz2p6lz56l8.us-east-1.rds.amazonaws.com;Port=5432;Database=postgres;Username=postgres;Password=Whopper2022!;SearchPath=core", builder => builder.MigrationsAssembly(typeof(SupplizeDomainAuthorizationDbContext).Assembly.FullName)).UseSnakeCaseNamingConvention();
                });
                var poly = new UserPolicyHandler(null, null);
                Services.AddSingleton<ICurrentUserService, CurrentUserService>();
                Services.AddHeimGuard<UserPolicyHandler>()
                .MapAuthorizationPolicies()
                .AutomaticallyCheckPermissions();
            }
            catch (Exception e)
            {
                Console.WriteLine("BIG EXCEPTION ");
                Console.WriteLine(e);
            }
        }

        public string GetSecretConnection()// need to write test cases
        {
            string secretName = "postgres";

            string secret = "";

            MemoryStream memoryStream = new MemoryStream();

            GetSecretValueRequest request = new GetSecretValueRequest();
            request.SecretId = secretName;
            request.VersionStage = "AWSCURRENT"; // VersionStage defaults to AWSCURRENT if unspecified.

            GetSecretValueResponse response = null;

            // In this we only handle the specific exceptions for the 'GetSecretValue' API.
            // See https://docs.aws.amazon.com/secretsmanager/latest/apireference/API_GetSecretValue.html
            // We rethrow the exception by default.
            try
            {
                response = _manager.GetSecretValueAsync(request).Result;
            }
            catch (DecryptionFailureException e)
            {
                // Secrets Manager can't decrypt the protected secret text using the provided KMS key.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }
            catch (InternalServiceErrorException e)
            {
                // An error occurred on the server side.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }
            catch (InvalidParameterException e)
            {
                // You provided an invalid value for a parameter.
                // Deal with the exception here, and/or rethrow at your discretion
                throw;
            }
            catch (InvalidRequestException e)
            {
                // You provided a parameter value that is not valid for the current state of the resource.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }
            catch (ResourceNotFoundException e)
            {
                // We can't find the resource that you asked for.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }
            catch (System.AggregateException ae)
            {
                // More than one of the above exceptions were triggered.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }

            // Decrypts secret using the associated KMS key.
            // Depending on whether the secret is a string or binary, one of these fields will be populated.
            if (response.SecretString != null)
            {
                secret = response.SecretString;
            }
            else
            {
                memoryStream = response.SecretBinary;
                StreamReader reader = new StreamReader(memoryStream);
                secret = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
            }
            return secret;
        }

        public async Task<string> GetParameterValueAsync(string parameter)
        {

            var response = await _client.GetParameterAsync(new GetParameterRequest
            {
                Name = parameter,
                WithDecryption = true
            });

            return response.Parameter.Value;
        }

    }


}