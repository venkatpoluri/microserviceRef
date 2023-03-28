using Serilog;
using TradingPartnerManagement.Extensions.Application;
using TradingPartnerManagement.Extensions.Host;
using TradingPartnerManagement.Extensions.Services;
using Microsoft.AspNetCore.Authorization;
using SupplizeDomainAuthorization.Domain.Concepts;



var builder = WebApplication.CreateBuilder(args);
builder.Host.AddLoggingConfiguration(builder.Environment);
AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.ConfigureServices();
var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// For elevated security, it is recommended to remove this middleware and set your server to only listen on https.
// A slightly less secure option would be to redirect http to 400, 505, etc.
app.UseHttpsRedirection();

app.UseCors("TradingPartnerManagementCorsPolicy");

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseMultiConcept();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/api/health");
    if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment(TradingPartnerManagement.Resources.LocalConfig.FunctionalTestingEnvName))
        endpoints.MapControllers().WithMetadata(new AllowAnonymousAttribute());
    else
        endpoints.MapControllers();
});

app.UseSwaggerExtension();

try
{
    Log.Information("Starting application");
    await app.RunAsync();
}
catch (Exception e)
{
    Log.Error(e, "The application failed to start correctly");
    throw;
}
finally
{
    Log.Information("Shutting down application");
    Log.CloseAndFlush();
}

// Make the implicit Program class public so the functional test project can access it
public partial class Program { }