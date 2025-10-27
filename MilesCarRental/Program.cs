using FluentValidation;
using MilesCarRental.Api.Filters;
using MilesCarRental.Api.HealthChecks;
using MilesCarRental.Api.Middleware;
using MilesCarRental.Api.Swagger;
using MilesCarRental.Application.Services;
using MilesCarRental.Application.Validators;
using MilesCarRental.Infrastructure.Extensions;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Threading.RateLimiting;

// ? 1. Configure Serilog (FIRST - before building)
Log.Logger = new LoggerConfiguration()
 .WriteTo.Console()
  .WriteTo.File(
        path: "logs/milescarrental-.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "MilesCarRental")
    .CreateLogger();

try
{
    Log.Information("Starting MilesCarRental API");

    var builder = WebApplication.CreateBuilder(args);

    // ? 2. Use Serilog for logging
    builder.Host.UseSerilog();

    // ? 3. Add Rate Limiting
    builder.Services.AddRateLimiter(options =>
    {
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
   RateLimitPartition.GetFixedWindowLimiter(
       partitionKey: context.User.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
    factory: partition => new FixedWindowRateLimiterOptions
    {
        AutoReplenishment = true,
        PermitLimit = 100,    // 100 requests
        Window = TimeSpan.FromMinutes(1),  // per minute
        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        QueueLimit = 10       // Queue up to 10 requests
    }));

        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    });

    // ? 4. Add OpenTelemetry
    builder.Services.AddOpenTelemetry()
        .ConfigureResource(resource => resource
   .AddService(
    serviceName: "MilesCarRental.API",
             serviceVersion: Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0"))
        .WithTracing(tracing => tracing
.AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
                options.Filter = (httpContext) =>
                     {
                         // No trace health check endpoints
                         return !httpContext.Request.Path.StartsWithSegments("/health");
                     };
            })
            .AddSource("MilesCarRental")
   .AddConsoleExporter());

    // Add services to the container.
    builder.Services.AddControllers();

    // ? 5. Add FluentValidation
    builder.Services.AddValidatorsFromAssemblyContaining<VehicleSearchRequestValidator>();
    builder.Services.AddScoped<ValidationFilter>(); // ?? Register ValidationFilter

    // ? 6. Add Health Checks
    builder.Services.AddHealthChecks()
        .AddCheck<ApiHealthCheck>("api", tags: new[] { "ready" })
        .AddCheck<DatabaseHealthCheck>("database", tags: new[] { "ready" });

    // ? 7. Configure Swagger with Examples
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "Miles Car Rental API",
            Version = "v1",
            Description = "API para búsqueda y gestión de alquiler de vehículos en Colombia.",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "Miles Car Rental",
                Email = "api@milescarrental.com",
                Url = new Uri("https://www.milescarrental.com")
            },
            License = new Microsoft.OpenApi.Models.OpenApiLicense
            {
                Name = "MIT",
                Url = new Uri("https://opensource.org/licenses/MIT")
            }
        });

        // Enable XML comments
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }

        // ? Add example filters
        options.ExampleFilters();
    });

    builder.Services.AddSwaggerExamplesFromAssemblyOf<VehicleSearchRequestExample>();

    // Register Infrastructure InMemory repositories
    builder.Services.AddInMemoryData();

    // Register Application services
    builder.Services.AddScoped<ISearchService, SearchService>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        // ? 1. Swagger PRIMERO
        app.UseSwagger();
        app.UseSwaggerUI(c =>
 {
     c.SwaggerEndpoint("/swagger/v1/swagger.json", "Miles Car Rental API v1");
     c.RoutePrefix = string.Empty; // Swagger at root
     c.DocumentTitle = "Miles Car Rental API";
     c.DisplayRequestDuration();
 });
    }

    // ? 2. Serilog Request Logging
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
      {
          diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
          diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
          diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress);
      };
    });

    // ? 3. Global Exception Handler (SOLO para rutas /api/*)
    app.UseWhen(
context => context.Request.Path.StartsWithSegments("/api"),
        appBuilder => appBuilder.UseGlobalExceptionHandler());

    // ? 4. Rate Limiting (SOLO para rutas /api/*)
    app.UseWhen(
        context => context.Request.Path.StartsWithSegments("/api"),
        appBuilder => appBuilder.UseRateLimiter());

    app.UseHttpsRedirection();

    app.UseAuthorization();

    // ? 5. Map Health Check Endpoints
    app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                status = report.Status.ToString(),
                timestamp = DateTime.UtcNow,
                duration = report.TotalDuration,
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description,
                    duration = e.Value.Duration,
                    exception = e.Value.Exception?.Message,
                    data = e.Value.Data
                })
            });
            await context.Response.WriteAsync(result);
        }
    });

    app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready")
    });

    app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        Predicate = _ => false // Only basic check
    });

    app.MapControllers();

    Log.Information("MilesCarRental API started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
