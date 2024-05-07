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
        serviceCollection.AddSingleton<IBlockChainDataHandler, EcoEarnTransactionHandler>();
        
    }
    
    protected override string ClientId => "AElfIndexer_Points";
    protected override string Version => "bcc8994e376e47309e6b287d97244c98";
}







