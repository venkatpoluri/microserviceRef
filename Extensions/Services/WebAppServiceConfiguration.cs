namespace TradingPartnerManagement.Extensions.Services;

using TradingPartnerManagement.Middleware;
using TradingPartnerManagement.Services;
using System.Text.Json.Serialization;
using Serilog;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Sieve.Services;
using System.Reflection;
using Serilog.Formatting.Compact;
using Serilog;
using AWS.Logger.Core;
using AWS.Logger.AspNetCore;
using AWS.Logger.SeriLog;
using Amazon.CloudWatchLogs;
using SupplizeIdentityAuthorization;

public static class WebAppServiceConfiguration
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        // Do not move: Sieve must be added to scope first
        builder.Services.AddSieve();
        builder.Services.AddSingleton(Log.Logger);
        // TODO update CORS for your env
        builder.Services.AddCorsService("TradingPartnerManagementCorsPolicy", builder.Environment);
        builder.Services.OpenTelemetryRegistration("TradingPartnerManagement", builder.Environment);
        builder.Services.AddInfrastructure(builder.Environment);
        builder.Host.UseSerilog((ctx, lc) =>
        {
            lc.ReadFrom.Configuration(ctx.Configuration)
                .WriteTo.AWSSeriLog(configuration: ctx.Configuration, textFormatter: new RenderedCompactJsonFormatter());
        });
   
        builder.Services
            .AddControllers()            
            .AddJsonOptions((o) => {
                o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                o.JsonSerializerOptions.Converters.Add(new SystemTextJsonPatch.Converters.JsonPatchDocumentConverterFactory());
                o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumMemberConverter());    
                                o.JsonSerializerOptions.Converters.Add(new Ardalis.SmartEnum.SystemTextJson.SmartEnumNameConverter<TradingPartnerManagement.Domain.SupplyAgreements.ClassificationTypeEnum,int>());;
                o.JsonSerializerOptions.Converters.Add(new Ardalis.SmartEnum.SystemTextJson.SmartEnumNameConverter<TradingPartnerManagement.Domain.SupplyAgreements.DiscountTypeEnum,int>());;
                o.JsonSerializerOptions.Converters.Add(new Ardalis.SmartEnum.SystemTextJson.SmartEnumNameConverter<TradingPartnerManagement.Domain.SupplyAgreements.OrderLeadTimeUomEnum,int>());;
                o.JsonSerializerOptions.Converters.Add(new Ardalis.SmartEnum.SystemTextJson.SmartEnumNameConverter<TradingPartnerManagement.Domain.SupplyAgreements.OrderCancelTimeUomEnum,int>());;
                o.JsonSerializerOptions.Converters.Add(new Ardalis.SmartEnum.SystemTextJson.SmartEnumNameConverter<TradingPartnerManagement.Domain.SupplyAgreements.TermTypeEnum,int>()); 
                o.JsonSerializerOptions.Converters.Add(new TinyHelpers.Json.Serialization.TimeOnlyConverter());    
                o.JsonSerializerOptions.Converters.Add(new TinyHelpers.Json.Serialization.DateOnlyConverter());  
                o.JsonSerializerOptions.Converters.Add(new TinyHelpers.Json.Serialization.TimeSpanTicksConverter()); 
                o.JsonSerializerOptions.Converters.Add(Open.Serialization.Json.System.Converters.JsonNullableDecimalConverter.Instance);
                o.JsonSerializerOptions.Converters.Add(Open.Serialization.Json.System.Converters.JsonNullableDoubleConverter.Instance);
                o.JsonSerializerOptions.Converters.Add(new TradingPartnerManagement.Domain.JsonGuidConverter());
            });
        builder.Services.AddApiVersioningExtension();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
        // registers all services that inherit from your base service interface - ITradingPartnerManagementService
        builder.Services.AddBoundaryServices(Assembly.GetExecutingAssembly());

        builder.Services.AddMvc(options => options.Filters.Add<ErrorHandlerFilterAttribute>())
            .AddFluentValidation(cfg => { cfg.AutomaticValidationEnabled = false; });
        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

        builder.Services.AddHealthChecks();
        builder.Services.AddSwaggerExtension();
        builder.Services.AddSupplizeAuthorization();
    }

    /// <summary>
    /// Registers all services in the assembly of the given interface.
    /// </summary>
    private static void AddBoundaryServices(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (!assemblies.Any())
            throw new ArgumentException("No assemblies found to scan. Supply at least one assembly to scan for handlers.");

        foreach (var assembly in assemblies)
        {
            var rules = assembly.GetTypes()
                .Where(x => !x.IsAbstract && x.IsClass && x.GetInterface(nameof(ITradingPartnerManagementService)) == typeof(ITradingPartnerManagementService));

            foreach (var rule in rules)
            {
                foreach (var @interface in rule.GetInterfaces())
                {
                    services.Add(new ServiceDescriptor(@interface, rule, ServiceLifetime.Scoped));
                }
            }
        }
    }
}