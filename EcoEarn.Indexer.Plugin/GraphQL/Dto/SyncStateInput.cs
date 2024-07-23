using AElfIndexer;

namespace EcoEarn.Indexer.Plugin.GraphQL.Dto;

public class SyncStateInput
{
    public string ChainId { get; set; }
    public BlockFilterType FilterType { get; set; }
}