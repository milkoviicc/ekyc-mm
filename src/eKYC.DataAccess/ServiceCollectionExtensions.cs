using eKYC.DataAccess.Admin;
using eKYC.DataAccess.Clients;
using eKYC.DataAccess.Dashboard;
using eKYC.DataAccess.ReferenceData;
using eKYC.DataAccess.Reports;
using eKYC.DataAccess.Revisions;
using Microsoft.Extensions.DependencyInjection;

namespace eKYC.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEkycDataAccess(this IServiceCollection services)
    {
        services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();

        services.AddScoped<IClStateRepository, ClStateRepository>();
        services.AddScoped<IClientTypeRepository, ClientTypeRepository>();
        services.AddScoped<IRiskClassRepository, RiskClassRepository>();
        services.AddScoped<IRiskEstimateRepository, RiskEstimateRepository>();
        services.AddScoped<IOwnershipTypeRepository, OwnershipTypeRepository>();
        services.AddScoped<IDocumentTypeRepository, DocumentTypeRepository>();
        services.AddScoped<IClientProcessingStatusRepository, ClientProcessingStatusRepository>();
        services.AddScoped<ICL_ClntRepository, CL_ClntRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();
        services.AddScoped<IClientAnalysisRepository, ClientAnalysisRepository>();
        services.AddScoped<IReportRiskRepository, ReportRiskRepository>();
        services.AddScoped<ICL_Doc_RevisionsRepository, CL_Doc_RevisionsRepository>();
        services.AddScoped<IRevisionTypeRepository, RevisionTypeRepository>();
        services.AddScoped<IA_Object_LocksRepository, A_Object_LocksRepository>();
        services.AddScoped<IA_UsrRepository, A_UsrRepository>();
        services.AddScoped<IA_ISRoleRepository, A_ISRoleRepository>();
        services.AddScoped<IA_Usr_ISRoleRepository, A_Usr_ISRoleRepository>();
        services.AddScoped<IA_ISRole_ObjctRepository, A_ISRole_ObjctRepository>();
        services.AddScoped<IA_ObjctRepository, A_ObjctRepository>();
        services.AddScoped<IA_Objct_TypsRepository, A_Objct_TypsRepository>();
        services.AddScoped<IA_Apl_PrmtrRepository, A_Apl_PrmtrRepository>();
        services.AddScoped<IOperation_LogRepository, Operation_LogRepository>();

        return services;
    }
}
