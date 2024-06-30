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
        Configure<PoolBlackListOptions>(configuration.GetSection("PoolBlackList"));

        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, PointsPoolClaimedLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, PointsPoolCreatedLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, PointsPoolEndTimeSetLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, PointsPoolRewardPerSecondSetLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, PointsPoolRewardConfigSetLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, PointsPoolUpdateAddressSetLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, PointsPoolRestartedLogEventProcessor>();
        
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokenPoolClaimedLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokenPoolCreatedLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokensPoolEndTimeSetLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokensPoolFixedBoostFactorSetLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokensPoolRewardPerSecondSetLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokensPoolRewardConfigSetLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokensPoolStakeConfigSetLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokenPoolStakedLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokenPoolUnlockedLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokensPoolUnlockWindowDurationSetLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokenPoolRenewedLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, TokensPoolMergeIntervalSetLogEventProcessor>();
        
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, RewardsClaimedLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, RewardsEarlyStakedLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, RewardsWithdrawnLogEventProcessor>();

        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, LiquidityAddedLogEvenProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, LiquidityRemovedLogEventProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, LiquidityStakedLogEventProcessor>();
        
        serviceCollection.AddSingleton<IBlockChainDataHandler, EcoEarnTransactionHandler>();
    }

    protected override string ClientId => "AElfIndexer_EcoEarn";
    protected override string Version => "879e18b2f2d44230950c7dd000df51b2";
}