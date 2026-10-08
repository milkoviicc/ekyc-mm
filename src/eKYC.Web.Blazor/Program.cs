using eKYC.Web.Blazor;
using eKYC.Web.Blazor.Components;
using eKYC.Web.Blazor.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Windows Authentication (Negotiate/NTLM) requires the whole multi-leg handshake to happen on one
// HTTP/1.1 connection — Kestrel's default HTTP/2 negotiation over TLS breaks it. Force 1.1 on every
// endpoint rather than just the https one, since the plain-http endpoint hit the same failure in testing.
builder.WebHost.ConfigureKestrel(options =>
    options.ConfigureEndpointDefaults(listenOptions => listenOptions.Protocols = HttpProtocols.Http1));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

// Same Windows-integrated SSO as eKYC.Api — see eKYC migration analysis §03/§08 and CLAUDE.md.
// DEVELOPMENT ONLY: Negotiate has an unresolved Kestrel+Blazor Server handshake bug (CLAUDE.md,
// decision #4) that blocks local dev entirely, so Development swaps in an auto-authenticated dev user
// instead. [Authorize]/.RequireAuthorization() still run either way — only *how* the user is
// established changes. Every other environment uses real Negotiate, unchanged.
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
builder.Services.AddCascadingAuthenticationState();

var apiBaseUrl = builder.Configuration["Api:BaseUrl"]
    ?? throw new InvalidOperationException("Configuration value 'Api:BaseUrl' is not set.");

// UseDefaultCredentials flows this process's Windows identity through to eKYC.Api's Negotiate auth.
// That's correct for same-machine dev; a real multi-server deployment needs Kerberos constrained
// delegation configured in AD so the *browser user's* identity (not the host process's) makes the hop —
// an infrastructure/AD-admin task, not something fixable from this code alone.
// A new handler instance is returned per call (not a shared one) — IHttpClientFactory owns and disposes
// handler instances on its own rotation schedule, so reusing one instance across clients would break both.
static HttpClientHandler CreateApiHandler() => new() { UseDefaultCredentials = true };

builder.Services.AddHttpClient<IReferenceDataApiClient, ReferenceDataApiClient>(client =>
    client.BaseAddress = new Uri(apiBaseUrl)).ConfigurePrimaryHttpMessageHandler(CreateApiHandler);

builder.Services.AddHttpClient<ICL_ClntApiClient, CL_ClntApiClient>(client =>
    client.BaseAddress = new Uri(apiBaseUrl)).ConfigurePrimaryHttpMessageHandler(CreateApiHandler);

builder.Services.AddHttpClient<IDashboardApiClient, DashboardApiClient>(client =>
    client.BaseAddress = new Uri(apiBaseUrl)).ConfigurePrimaryHttpMessageHandler(CreateApiHandler);

builder.Services.AddHttpClient<IClientAnalysisApiClient, ClientAnalysisApiClient>(client =>
    client.BaseAddress = new Uri(apiBaseUrl)).ConfigurePrimaryHttpMessageHandler(CreateApiHandler);

builder.Services.AddHttpClient<IReportsApiClient, ReportsApiClient>(client =>
    client.BaseAddress = new Uri(apiBaseUrl)).ConfigurePrimaryHttpMessageHandler(CreateApiHandler);

builder.Services.AddHttpClient<ICL_Doc_RevisionsApiClient, CL_Doc_RevisionsApiClient>(client =>
    client.BaseAddress = new Uri(apiBaseUrl)).ConfigurePrimaryHttpMessageHandler(CreateApiHandler);

builder.Services.AddHttpClient<IA_Object_LocksApiClient, A_Object_LocksApiClient>(client =>
    client.BaseAddress = new Uri(apiBaseUrl)).ConfigurePrimaryHttpMessageHandler(CreateApiHandler);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    // Forces the Negotiate challenge to happen in ASP.NET Core's own auth middleware, before Blazor's
    // router/AuthorizeRouteView ever starts rendering. Without this, an unauthenticated request hits
    // AuthorizeRouteView mid-render, which tries to challenge after the response has already started
    // streaming — that invalid state surfaces to the client as a bare 400, not a clean 401 challenge.
    .RequireAuthorization();

app.Run();
