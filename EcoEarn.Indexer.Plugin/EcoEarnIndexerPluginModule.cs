using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using EcoEarn.Indexer.Plugin.GraphQL;
using EcoEarn.Indexer.Plugin.Handlers;
using EcoEarn.Indexer.Plugin.Processors;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace EcoEarn.Indexer.Plugin;

[DependsOn(typeof(AElfIndexerClientModule), typeof(AbpAutoMapperModule))]
public class EcoEarnIndexerPluginModule : AElfIndexerClientPluginBaseModule<EcoEarnIndexerPluginModule,
    EcoEarnIndexerPluginSchema, Query>
{
    protected override void ConfigureServices(IServiceCollection serviceCollection)
    {
        var configuration = serviceCollection.GetConfiguration();
        Configure<ContractInfoOptions>(configuration.GetSection("ContractInfo"));

        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, PointsPoolClaimedLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, PointsPoolCreatedLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, PointsPoolEarlyStakedLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, PointsPoolWithdrawnLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokenPoolClaimedLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokenPoolCreatedLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokenPoolEarlyStakedLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokenPoolStakedLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokenPoolWithdrawnLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, PointsPoolRestartedLogEventProcessor>();
        serviceCollection.AddSingleton<IBlockChainDataHandler, EcoEarnTransactionHandler>();
    }

    protected override string ClientId => "AElfIndexer_EcoEarn";
    protected override string Version => "e09fe829c3c0478482aefd3d2af7ba8f";
}