using AElf.Indexing.Elasticsearch;
using AElfIndexer.Client;
using Nest;

namespace EcoEarn.Indexer.Plugin.Entities;

public class RewardsClaimIndex : AElfIndexerClientEntity<string>, IIndexBuild
{
    [Keyword] public string ClaimId { get; set; }
    [Keyword] public string StakeId { get; set; }
    [Keyword] public string Seed { get; set; }
    [Keyword] public string PoolId { get; set; }
    [Keyword] public string ClaimedAmount { get; set; }
    [Keyword] public string EarlyStakedAmount { get; set; }
    [Keyword] public string ClaimedSymbol { get; set; }
    public long ClaimedBlockNumber { get; set; }
    public long ClaimedTime { get; set; }
    public long ReleaseTime { get; set; }
    public long WithdrawTime { get; set; }
    public long EarlyStakeTime { get; set; }
    [Keyword] public string Account { get; set; }
    public PoolType PoolType { get; set; }
    [Keyword] public string WithdrawSeed { get; set; } = "";
    [Keyword] public string LiquidityId { get; set; } = "";
    [Keyword] public string ContractAddress { get; set; } = "";
    [Keyword] public string EarlyStakeSeed { get; set; } = "";
    [Keyword] public string LiquidityAddedSeed { get; set; } = "";
}

public enum PoolType
{
    Points = 0,
    Token = 1,
    Lp = 2,
    All = -1,
}