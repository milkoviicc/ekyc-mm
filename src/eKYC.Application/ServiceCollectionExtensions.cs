using eKYC.Application.Admin;
using eKYC.Application.Clients;
using eKYC.Application.Dashboard;
using eKYC.Application.ReferenceData;
using eKYC.Application.Reports;
using eKYC.Application.Revisions;
using eKYC.Application.Tenancy;
using Microsoft.Extensions.DependencyInjection;

namespace eKYC.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEkycApplication(this IServiceCollection services)
    {
        services.AddScoped<IReferenceDataService, ReferenceDataService>();
        services.AddScoped<ICL_ClntService, CL_ClntService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IClientAnalysisService, ClientAnalysisService>();
        services.AddScoped<IReportRiskService, ReportRiskService>();
        services.AddScoped<ICL_Doc_RevisionsService, CL_Doc_RevisionsService>();
        services.AddScoped<IA_Object_LocksService, A_Object_LocksService>();
        services.AddScoped<IA_UsrService, A_UsrService>();
        services.AddScoped<IA_ISRoleService, A_ISRoleService>();
        services.AddScoped<IA_Usr_ISRoleService, A_Usr_ISRoleService>();
        services.AddScoped<IA_ISRole_ObjctService, A_ISRole_ObjctService>();
        services.AddScoped<IA_ObjctService, A_ObjctService>();
        services.AddScoped<IA_Objct_TypsService, A_Objct_TypsService>();
        services.AddScoped<IA_Apl_PrmtrService, A_Apl_PrmtrService>();
        services.AddScoped<ICurrentTenantProvider, SingleTenantProvider>();

        return services;
    }
}
