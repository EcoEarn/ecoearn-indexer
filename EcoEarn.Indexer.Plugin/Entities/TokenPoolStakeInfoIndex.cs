using AElf.Indexing.Elasticsearch;
using AElfIndexer.Client;
using Nest;

namespace EcoEarn.Indexer.Plugin.Entities;

public class TokenPoolStakeInfoIndex : AElfIndexerClientEntity<string>, IIndexBuild
{
    [Keyword] public string PoolId { get; set; }
    [Keyword] public string AccTokenPerShare { get; set; }
    [Keyword] public string TotalStakedAmount { get; set; }
    public long LastRewardTime { get; set; }
}