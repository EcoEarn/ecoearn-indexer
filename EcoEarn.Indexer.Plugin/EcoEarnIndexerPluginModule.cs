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
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, PointsPoolEndTimeSetLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, PointsPoolRewardPerSecondSetLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, PointsPoolRewardReleasePeriodSetLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, PointsPoolUpdateAddressSetLogEventProcessor>();
        
        
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokenPoolClaimedLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokenPoolCreatedLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokensPoolEndTimeSetLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokensPoolFixedBoostFactorSetLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokensPoolRewardPerSecondSetLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokensPoolRewardReleasePeriodSetLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokensPoolStakeConfigSetLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokenPoolEarlyStakedLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokenPoolStakedLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokenPoolWithdrawnLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, PointsPoolRestartedLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokenPoolUnlockedLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokensPoolUnlockWindowDurationSetLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, RenewedLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokensPoolRewardReleasePeriodSetLogEventProcessor>();
        serviceCollection.AddSingleton<IBlockChainDataHandler, EcoEarnTransactionHandler>();
    }

    protected override string ClientId => "AElfIndexer_EcoEarn";
    protected override string Version => "918fcad21b7d464186557002ad3a80ea";
}