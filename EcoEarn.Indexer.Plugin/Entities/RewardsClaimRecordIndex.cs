using AElf.Indexing.Elasticsearch;
using AElfIndexer.Client;
using Nest;

namespace EcoEarn.Indexer.Plugin.Entities;

public class RewardsClaimRecordIndex : AElfIndexerClientEntity<string>, IIndexBuild
{
    [Keyword] public string PoolId { get; set; }
    [Keyword] public string Account { get; set; }
    [Keyword] public string Amount { get; set; }
    [Keyword] public string Seed { get; set; }
    public PoolType PoolType { get; set; }
}