using AElf.Indexing.Elasticsearch;
using AElfIndexer.Client;
using Nest;

namespace EcoEarn.Indexer.Plugin.Entities;

public class RewardsInfoIndex : AElfIndexerClientEntity<string>, IIndexBuild
{
    [Keyword] public string ClaimId { get; set; }
    [Keyword] public string StakeId { get; set; } = "";
    [Keyword] public string Seed { get; set; }
    [Keyword] public string PoolId { get; set; }
    [Keyword] public string DappId { get; set; }
    [Keyword] public string ClaimedAmount { get; set; }
    [Keyword] public string ClaimedSymbol { get; set; }
    public long ClaimedTime { get; set; }
    [Keyword] public string Account { get; set; }
    public PoolType PoolType { get; set; }
}