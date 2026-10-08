using DbUp;
using eKYC.Api;
using eKYC.Api.Middleware;
using eKYC.Application;
using eKYC.DataAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.Data.SqlClient;
using Scalar.AspNetCore;
using Serilog;

// Optional .env (searched upwards from the working directory) keeps the remote DB credentials out of source.
// Values become environment variables, so they are also visible through builder.Configuration.
DotNetEnv.Env.TraversePath().NoClobber().Load();

var builder = WebApplication.CreateBuilder(args);

// If DB_USER and DB_PASSWORD are filled in, they win over the user-secrets / appsettings connection string.
var dbUser = builder.Configuration["DB_USER"];
var dbPassword = builder.Configuration["DB_PASSWORD"];
if (!string.IsNullOrWhiteSpace(dbUser) && !string.IsNullOrWhiteSpace(dbPassword))
{
    var connectionStringBuilder = new SqlConnectionStringBuilder
    {
        DataSource = builder.Configuration["DB_SERVER"] ?? throw new InvalidOperationException("DB_SERVER is not set in .env."),
        InitialCatalog = builder.Configuration["DB_NAME"] ?? "eKYC",
        UserID = dbUser,
        Password = dbPassword,
        TrustServerCertificate = true,
    };
    builder.Configuration["ConnectionStrings:eKYC"] = connectionStringBuilder.ConnectionString;
}

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

// `dotnet run --project src/eKYC.Api -- --migrate` applies pending DbUp scripts from database/migrations
// and exits, matching this workspace's usual migration workflow — it does not start the web host.
if (args.Contains("--migrate"))
{
    var connectionString = builder.Configuration.GetConnectionString("eKYC")
        ?? throw new InvalidOperationException("Connection string 'eKYC' is not configured.");
    var migrationsPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "database", "migrations");

    var upgrader = DeployChanges.To
        .SqlDatabase(connectionString)
        .WithScriptsFromFileSystem(Path.GetFullPath(migrationsPath))
        .LogToConsole()
        .Build();

    var result = upgrader.PerformUpgrade();
    Environment.Exit(result.Successful ? 0 : 1);
    return;
}

// JSON property names are emitted exactly as declared on the C# models (no camelCase), so the A_* / CL_* models
// keep their table-column names (e.g. Clnt_Id, HBOR_ID) and the React types in eKYC.Web.React match 1:1.
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.PropertyNamingPolicy = null);
builder.Services.AddOpenApi();

builder.Services.AddEkycDataAccess();
builder.Services.AddEkycApplication();

// Windows-integrated SSO (SPNEGO/Kerberos), replacing the legacy app's Spring Security Kerberos setup —
// see the eKYC migration analysis, §03/§08. Role checks (UNOS/ODOBR1/ODOBR2/ADMIN/...) come from AD group
// membership once the object-level role/permission model (workstream 1, still in progress) maps them.
// DEVELOPMENT ONLY: see eKYC.Web.Blazor's matching handler and CLAUDE.md decision #4 — Negotiate has an
// unresolved Kestrel+Blazor Server handshake bug that blocks local dev, so Development swaps in an
// auto-authenticated dev user here too. [Authorize] still runs; only how the user is established changes.
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddAuthentication(DevelopmentAuthenticationHandler.SchemeName)
        .AddScheme<AuthenticationSchemeOptions, DevelopmentAuthenticationHandler>(
            DevelopmentAuthenticationHandler.SchemeName, _ => { });
}
else
{
    builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
        .AddNegotiate();
}
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // UI at /scalar, reads the /openapi/v1.json document above
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
