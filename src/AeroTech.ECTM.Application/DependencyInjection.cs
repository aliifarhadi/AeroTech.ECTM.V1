using AeroTech.ECTM.Application._Shared.Behaviors;
using AeroTech.ECTM.Application._Shared.Concurrency;
using AeroTech.ECTM.Application._Shared.Events;
using AeroTech.ECTM.Application._Shared.Paging;
using AeroTech.ECTM.Application.AircraftAggregate.Commands.CreateAircraft;
using AeroTech.ECTM.Application.AircraftAggregate.Commands.UpdateAircraft;
using AeroTech.ECTM.Application.AircraftAggregate.Queries.GetAircraftById;
using AeroTech.ECTM.Application.AircraftAggregate.Queries.GetAircraftPaginated;
using AeroTech.ECTM.Application.AircraftAggregate.Services;
using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Commands.RecomputeEngine;
using AeroTech.ECTM.Application.BaselineAggregate.Commands.CloneBaseline;
using AeroTech.ECTM.Application.BaselineAggregate.Commands.CreateBaseline;
using AeroTech.ECTM.Application.BaselineAggregate.Commands.PrefillBaselineFromFile;
using AeroTech.ECTM.Application.BaselineAggregate.Commands.PublishBaseline;
using AeroTech.ECTM.Application.BaselineAggregate.Commands.ReconcileBaseline;
using AeroTech.ECTM.Application.BaselineAggregate.Commands.RetireBaseline;
using AeroTech.ECTM.Application.BaselineAggregate.Commands.SetPrimaryBaseline;
using AeroTech.ECTM.Application.BaselineAggregate.Commands.UpdateBaseline;
using AeroTech.ECTM.Application.BaselineAggregate.Queries.GetBaselineById;
using AeroTech.ECTM.Application.BaselineAggregate.Queries.GetEngineBaselines;
using AeroTech.ECTM.Application.BaselineAggregate.Queries.PreviewBaseline;
using AeroTech.ECTM.Application.BaselineAggregate.Services;
using AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Queries.GetEngineBaselineTrend;
using AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Queries.GetFlightBaselineComparisons;
using AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Services;
using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Commands.RecomputeFlight;
using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Queries.GetFlightRevisions;
using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Services;
using AeroTech.ECTM.Application.AuditEntryAggregate.Queries.GetAuditEntriesPaginated;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.CreateConfigurationDraft;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.PublishConfigurationDocument;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.RetireConfigurationDocument;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.UpdateConfigurationDraft;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.DryRunOnFlight;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.DryRunPhasesOnFlight;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.EvaluateTypedDefinition;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationDocumentById;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationDocumentDiff;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationDocumentsPaginated;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationKinds;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetReadiness;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.PreviewDataProfile;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.ValidateConfigurationDocument;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services;
using AeroTech.ECTM.Application.EngineAggregate.Commands.AdjustEngineLifeCounters;
using AeroTech.ECTM.Application.EngineAggregate.Commands.CreateEngine;
using AeroTech.ECTM.Application.EngineAggregate.Commands.UpdateEngine;
using AeroTech.ECTM.Application.EngineAggregate.Queries.GetEngineById;
using AeroTech.ECTM.Application.EngineAggregate.Queries.GetEnginesPaginated;
using AeroTech.ECTM.Application.EngineAggregate.Services;
using AeroTech.ECTM.Application.EngineInstallationAggregate.Commands.CloseEngineInstallation;
using AeroTech.ECTM.Application.EngineInstallationAggregate.Commands.CreateEngineInstallation;
using AeroTech.ECTM.Application.EngineInstallationAggregate.Queries.GetEngineInstallationsPaginated;
using AeroTech.ECTM.Application.EngineInstallationAggregate.Services;
using AeroTech.ECTM.Application.EngineModelAggregate.Commands.CreateEngineModel;
using AeroTech.ECTM.Application.EngineModelAggregate.Commands.UpdateEngineModel;
using AeroTech.ECTM.Application.EngineModelAggregate.Queries.GetEngineModelById;
using AeroTech.ECTM.Application.EngineModelAggregate.Queries.GetEngineModelsPaginated;
using AeroTech.ECTM.Application.EngineModelAggregate.Services;
using AeroTech.ECTM.Application.FlightAggregate.Commands.ConfirmFlightIdentity;
using AeroTech.ECTM.Application.FlightAggregate.Commands.ReviewFlight;
using AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightById;
using AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightSamples;
using AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightSeries;
using AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightsPaginated;
using AeroTech.ECTM.Application.FlightAggregate.Services;
using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Queries.ExplainFlightPhase;
using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Queries.GetAircraftEventRates;
using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Queries.GetFlightEvents;
using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Queries.GetFlightPhases;
using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Services;
using AeroTech.ECTM.Application.FlightQualityAggregate.Queries.GetAircraftQuality;
using AeroTech.ECTM.Application.FlightQualityAggregate.Queries.GetFlightQuality;
using AeroTech.ECTM.Application.FlightQualityAggregate.Services;
using AeroTech.ECTM.Application.FlightReviewAggregate.Commands.BulkReviewFlights;
using AeroTech.ECTM.Application.FlightReviewAggregate.Queries.GetReviewReport;
using AeroTech.ECTM.Application.FlightReviewAggregate.Services;
using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetEngineDataCoverage;
using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetEnginePeriodPerformance;
using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetFlightControlContext;
using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetFlightPhaseMetrics;
using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetFlightStabilityPoints;
using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetModelPeriodPerformance;
using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Services;
using AeroTech.ECTM.Application.ImportBatchAggregate.Commands.CommitImport;
using AeroTech.ECTM.Application.ImportBatchAggregate.Commands.PreviewImport;
using AeroTech.ECTM.Application.ImportBatchAggregate.Queries.GetImportBatchById;
using AeroTech.ECTM.Application.ImportBatchAggregate.Queries.GetImportBatchesPaginated;
using AeroTech.ECTM.Application.ImportBatchAggregate.Services;
using AeroTech.ECTM.Application.MaintenanceEventAggregate.Commands.CreateMaintenanceEvent;
using AeroTech.ECTM.Application.MaintenanceEventAggregate.Commands.UpdateMaintenanceEvent;
using AeroTech.ECTM.Application.MaintenanceEventAggregate.Queries.GetMaintenanceEventsPaginated;
using AeroTech.ECTM.Application.MaintenanceEventAggregate.Services;
using AeroTech.ECTM.Application.ProcessingJobAggregate.Queries.GetProcessingJobById;
using AeroTech.ECTM.Application.ProcessingJobAggregate.Services;
using AeroTech.Framework.Core.ServiceContracts;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AeroTech.ECTM.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = typeof(DependencyInjection).Assembly;

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
            services.AddValidatorsFromAssembly(assembly);
            services.AddTransient(typeof(IValidator<>), typeof(PageQueryValidator<>));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

            var resourceLock = configuration.GetSection(ResourceLockOptions.SectionName);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(
                resourceLock.Get<ResourceLockOptions>()?.ExpirySeconds ?? 0,
                $"{ResourceLockOptions.SectionName}:{nameof(ResourceLockOptions.ExpirySeconds)}");
            services.Configure<ResourceLockOptions>(resourceLock);

            services.AddScoped<IConfigurationContentValidator, ParameterCatalogContentValidator>();
            services.AddScoped<IConfigurationContentValidator, DataProfileContentValidator>();
            services.AddScoped<IConfigurationContentValidator, QualityRulesContentValidator>();
            services.AddScoped<IConfigurationContentValidator, PhaseRuleSetContentValidator>();
            services.AddScoped<IConfigurationContentValidator, SnapshotRulesContentValidator>();
            services.AddScoped<IConfigurationContentValidator, BaselineRulesContentValidator>();
            services.AddScoped<ConfigurationContentValidators>();
            services.AddScoped<ConfigurationDocumentLoader>();
            services.AddScoped<ActiveCatalogLoader>();
            services.AddScoped<IConfigurationDocumentLock, ConfigurationDocumentLock>();
            services.AddScoped<IConfigurationSeeder, ConfigurationSeeder>();
            services.AddScoped<IGetConfigurationKindsService, GetConfigurationKindsService>();
            services.AddScoped<IGetConfigurationDocumentsPaginatedService, GetConfigurationDocumentsPaginatedService>();
            services.AddScoped<IGetConfigurationDocumentByIdService, GetConfigurationDocumentByIdService>();
            services.AddScoped<ICreateConfigurationDraftService, CreateConfigurationDraftService>();
            services.AddScoped<IUpdateConfigurationDraftService, UpdateConfigurationDraftService>();
            services.AddScoped<IPublishConfigurationDocumentService, PublishConfigurationDocumentService>();
            services.AddScoped<IRetireConfigurationDocumentService, RetireConfigurationDocumentService>();
            services.AddScoped<IValidateConfigurationDocumentService, ValidateConfigurationDocumentService>();
            services.AddScoped<IGetConfigurationDocumentDiffService, GetConfigurationDocumentDiffService>();
            services.AddScoped<IPreviewDataProfileService, PreviewDataProfileService>();
            services.AddScoped<IDryRunOnFlightService, DryRunOnFlightService>();
            services.AddScoped<IEvaluateTypedDefinitionService, EvaluateTypedDefinitionService>();
            services.AddScoped<IGetReadinessService, GetReadinessService>();

            services.AddScoped<IGetAuditEntriesPaginatedService, GetAuditEntriesPaginatedService>();

            services.AddScoped<IAircraftLock, AircraftLock>();
            services.AddScoped<ICreateAircraftService, CreateAircraftService>();
            services.AddScoped<IUpdateAircraftService, UpdateAircraftService>();
            services.AddScoped<IGetAircraftByIdService, GetAircraftByIdService>();
            services.AddScoped<IGetAircraftPaginatedService, GetAircraftPaginatedService>();

            services.AddScoped<IEngineModelLock, EngineModelLock>();
            services.AddScoped<ICreateEngineModelService, CreateEngineModelService>();
            services.AddScoped<IUpdateEngineModelService, UpdateEngineModelService>();
            services.AddScoped<IGetEngineModelByIdService, GetEngineModelByIdService>();
            services.AddScoped<IGetEngineModelsPaginatedService, GetEngineModelsPaginatedService>();

            services.AddScoped<IEngineLock, EngineLock>();
            services.AddScoped<ICreateEngineService, CreateEngineService>();
            services.AddScoped<IUpdateEngineService, UpdateEngineService>();
            services.AddScoped<IAdjustEngineLifeCountersService, AdjustEngineLifeCountersService>();
            services.AddScoped<IGetEngineByIdService, GetEngineByIdService>();
            services.AddScoped<IGetEnginesPaginatedService, GetEnginesPaginatedService>();

            services.AddScoped<IEngineInstallationLock, EngineInstallationLock>();
            services.AddScoped<EngineInstallationDtoLoader>();
            services.AddScoped<ICreateEngineInstallationService, CreateEngineInstallationService>();
            services.AddScoped<ICloseEngineInstallationService, CloseEngineInstallationService>();
            services.AddScoped<IGetEngineInstallationsPaginatedService, GetEngineInstallationsPaginatedService>();

            services.AddScoped<MaintenanceEventDtoLoader>();
            services.AddScoped<ICreateMaintenanceEventService, CreateMaintenanceEventService>();
            services.AddScoped<IUpdateMaintenanceEventService, UpdateMaintenanceEventService>();
            services.AddScoped<IGetMaintenanceEventsPaginatedService, GetMaintenanceEventsPaginatedService>();

            services.Configure<ImportOptions>(configuration.GetSection(ImportOptions.SectionName));
            services.Configure<ProcessingJobOptions>(configuration.GetSection(ProcessingJobOptions.SectionName));
            services.AddSingleton<IProcessingJobRunner, ProcessingJobRunner>();
            services.AddScoped<IProcessingJobHandler, ImportCommitJobHandler>();
            services.AddScoped<IGetProcessingJobByIdService, GetProcessingJobByIdService>();

            services.AddScoped<UploadExpander>();
            services.AddScoped<RecordingSetLoader>();
            services.AddScoped<RecordingInspector>();
            services.AddScoped<FlightIdentityService>();
            services.AddScoped<FlightDtoLoader>();
            services.AddScoped<ImportBatchDtoLoader>();
            services.AddScoped<IPreviewImportService, PreviewImportService>();
            services.AddScoped<ICommitImportService, CommitImportService>();
            services.AddScoped<IGetImportBatchByIdService, GetImportBatchByIdService>();
            services.AddScoped<IGetImportBatchesPaginatedService, GetImportBatchesPaginatedService>();

            services.AddScoped<IGetFlightsPaginatedService, GetFlightsPaginatedService>();
            services.AddScoped<IGetFlightByIdService, GetFlightByIdService>();
            services.AddScoped<IConfirmFlightIdentityService, ConfirmFlightIdentityService>();
            services.AddScoped<IReviewFlightService, ReviewFlightService>();
            services.AddScoped<IGetFlightSeriesService, GetFlightSeriesService>();
            services.AddScoped<IGetFlightSamplesService, GetFlightSamplesService>();

            services.AddScoped<FlightQualityInputLoader>();
            services.AddScoped<IGetFlightQualityService, GetFlightQualityService>();
            services.AddScoped<IGetAircraftQualityService, GetAircraftQualityService>();

            services.AddScoped<AnalysisRulesLoader>();
            services.AddScoped<AnalysisRecorder>();
            services.AddScoped<RevisionResults>();
            services.AddScoped<RecomputeScheduler>();
            services.AddScoped<PhasePreviewer>();
            services.AddScoped<IProcessingJobHandler, RecomputeJobHandler>();
            services.AddScoped<IRecomputeFlightService, RecomputeFlightService>();
            services.AddScoped<IGetFlightRevisionsService, GetFlightRevisionsService>();
            services.AddScoped<IGetFlightPhasesService, GetFlightPhasesService>();
            services.AddScoped<IExplainFlightPhaseService, ExplainFlightPhaseService>();
            services.AddScoped<IGetFlightEventsService, GetFlightEventsService>();
            services.AddScoped<IGetAircraftEventRatesService, GetAircraftEventRatesService>();
            services.AddScoped<IDryRunPhasesOnFlightService, DryRunPhasesOnFlightService>();

            services.AddScoped<FlightReviewer>();
            services.AddScoped<EngineObservationLoader>();
            services.AddScoped<IBulkReviewFlightsService, BulkReviewFlightsService>();
            services.AddScoped<IRecomputeEngineService, RecomputeEngineService>();
            services.AddScoped<IGetFlightStabilityPointsService, GetFlightStabilityPointsService>();
            services.AddScoped<IGetFlightPhaseMetricsService, GetFlightPhaseMetricsService>();
            services.AddScoped<IGetFlightControlContextService, GetFlightControlContextService>();
            services.AddScoped<IGetEnginePeriodPerformanceService, GetEnginePeriodPerformanceService>();
            services.AddScoped<IGetModelPeriodPerformanceService, GetModelPeriodPerformanceService>();
            services.AddScoped<IGetEngineDataCoverageService, GetEngineDataCoverageService>();
            services.AddScoped<IGetReviewReportService, GetReviewReportService>();

            services.AddScoped<BaselineDefinitionsLoader>();
            services.AddScoped<ShopVisitCheck>();
            services.AddScoped<BaselineDtoLoader>();
            services.AddScoped<ICreateBaselineService, CreateBaselineService>();
            services.AddScoped<IUpdateBaselineService, UpdateBaselineService>();
            services.AddScoped<IPublishBaselineService, PublishBaselineService>();
            services.AddScoped<ICloneBaselineService, CloneBaselineService>();
            services.AddScoped<IRetireBaselineService, RetireBaselineService>();
            services.AddScoped<ISetPrimaryBaselineService, SetPrimaryBaselineService>();
            services.AddScoped<IPrefillBaselineFromFileService, PrefillBaselineFromFileService>();
            services.AddScoped<IReconcileBaselineService, ReconcileBaselineService>();
            services.AddScoped<IGetEngineBaselinesService, GetEngineBaselinesService>();
            services.AddScoped<IGetBaselineByIdService, GetBaselineByIdService>();
            services.AddScoped<IPreviewBaselineService, PreviewBaselineService>();

            services.AddScoped<BaselineEvaluationScheduler>();
            services.AddScoped<FlightEvidenceLoader>();
            services.AddScoped<ComparisonTolerancesLoader>();
            services.AddScoped<EngineBaselineReconciler>();
            services.AddScoped<BaselineEvaluationDtoLoader>();
            services.AddScoped<IProcessingJobHandler, BaselineEvaluationJobHandler>();
            services.AddScoped<IGetFlightBaselineComparisonsService, GetFlightBaselineComparisonsService>();
            services.AddScoped<IGetEngineBaselineTrendService, GetEngineBaselineTrendService>();

            return services;
        }
    }
}
