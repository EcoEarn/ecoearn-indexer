using AElfIndexer.Client.GraphQL;

namespace EcoEarn.Indexer.Plugin.GraphQL;

public class EcoEarnIndexerPluginSchema : AElfIndexerClientSchema<Query>
{
    public EcoEarnIndexerPluginSchema(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}