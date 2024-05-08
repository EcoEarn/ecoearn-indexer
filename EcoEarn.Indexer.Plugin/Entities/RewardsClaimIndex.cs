using AElf.Indexing.Elasticsearch;
using AElfIndexer.Client;
using Nest;

namespace EcoEarn.Indexer.Plugin.Entities;

public class RewardsClaimIndex : AElfIndexerClientEntity<string>, IIndexBuild
{
    [Keyword] public string ClaimId { get; set; }
    [Keyword] public string StakeId { get; set; }
    [Keyword] public string PoolId { get; set; }
    [Keyword] public string ClaimedAmount { get; set; }
    [Keyword] public string ClaimedSymbol { get; set; }
    public long ClaimedBlockNumber { get; set; }
    public long ClaimedTime { get; set; }
    public long UnlockTime { get; set; }
    public long WithdrawTime { get; set; }
    public long EarlyStakeTime { get; set; }
    [Keyword] public string Account { get; set; }
    public PoolType PoolType { get; set; }
}

public enum PoolType
{
    Points,
    Token,
    Lp
}