namespace TradingPartnerManagement.Extensions.Services;

using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;
using System.Diagnostics;

public static class OpenTelemetryServiceExtension
{
    public static void OpenTelemetryRegistration(this IServiceCollection services, string serviceName,IWebHostEnvironment webHostEnvironment)
    {
        services.AddOpenTelemetryTracing(builder =>
        {
            builder.SetResourceBuilder(GetResourceBuilder(webHostEnvironment,serviceName))
                .AddSource("MassTransit")
                .AddSource("Npgsql")
                // The following subscribes to activities from Activity Source
                // named "MyCompany.MyProduct.MyLibrary" only.
                // .AddSource("MyCompany.MyProduct.MyLibrary")
                .AddSqlClientInstrumentation(opt => opt.SetDbStatementForText = true)
                .AddAspNetCoreInstrumentation(
                    options =>
                    {
                        options.Enrich = Enrich;
                        options.RecordException = true;
                    })
                .AddJaegerExporter(o =>
                {
                    o.AgentHost = Environment.GetEnvironmentVariable("JAEGER_HOST");
                    o.AgentPort = 6831;
                    o.MaxPayloadSizeInBytes = 4096;
                    o.ExportProcessorType = ExportProcessorType.Batch;
                    o.BatchExportProcessorOptions = new BatchExportProcessorOptions<System.Diagnostics.Activity>
                    {
                        MaxQueueSize = 2048,
                        ScheduledDelayMilliseconds = 5000,
                        ExporterTimeoutMilliseconds = 30000,
                        MaxExportBatchSize = 512,
                    };
                });
        });
    }

    private static ResourceBuilder GetResourceBuilder(IWebHostEnvironment webHostEnvironment,string serviceName)
    {
        var version = Assembly
            .GetExecutingAssembly()
            .GetCustomAttribute<AssemblyFileVersionAttribute>()!
            .Version;
        return ResourceBuilder
            .CreateEmpty()
            .AddService(String.IsNullOrEmpty(serviceName) ? webHostEnvironment.ApplicationName : serviceName, serviceVersion: version)
            .AddAttributes(
                new KeyValuePair<string, object>[]
                {
                    new("deployment.environment", webHostEnvironment.EnvironmentName),
                    new("host.name", Environment.MachineName),
                })
            .AddTelemetrySdk()
            .AddEnvironmentVariableDetector();
    }

    private static void Enrich(Activity activity, string eventName, object obj)
    {
        if (obj is HttpRequest request)
        {
            var context = request.HttpContext;
            activity.AddTag("http.flavor", GetHttpFlavour(request.Protocol));
            activity.AddTag("http.scheme", request.Scheme);
            activity.AddTag("http.client_ip", context.Connection.RemoteIpAddress);
            activity.AddTag("http.request_content_length", request.ContentLength);
            activity.AddTag("http.request_content_type", request.ContentType);
        }
        else if (obj is HttpResponse response)
        {
            activity.AddTag("http.response_content_length", response.ContentLength);
            activity.AddTag("http.response_content_type", response.ContentType);
        }
    }

    public static string GetHttpFlavour(string protocol)
    {
        if (HttpProtocol.IsHttp10(protocol))
        {
            return "1.0";
        }
        else if (HttpProtocol.IsHttp11(protocol))
        {
            return "1.1";
        }
        else if (HttpProtocol.IsHttp2(protocol))
        {
            return "2.0";
        }
        else if (HttpProtocol.IsHttp3(protocol))
        {
            return "3.0";
        }

        throw new InvalidOperationException($@"Protocol {protocol} not recognized.");
    }
}