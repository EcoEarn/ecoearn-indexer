using AElf.Indexing.Elasticsearch;
using AElfIndexer.Client;
using Nest;

namespace EcoEarn.Indexer.Plugin.Entities;

public class TokenPoolIndex : AElfIndexerClientEntity<string>, IIndexBuild
{
    [Keyword] public string DappId { get; set; }
    [Keyword] public string PoolId { get; set; }
    [Keyword] public string PoolAddress { get; set; }
    [Keyword] public string Amount { get; set; }
    public TokenPoolConfig TokenPoolConfig { get; set; }
    public long CreateTime { get; set; }
    public PoolType PoolType { get; set; }
}

public class TokenPoolConfig
{
    [Keyword] public string RewardToken { get; set; }
    public long StartBlockNumber { get; set; }
    public long EndBlockNumber { get; set; }
    public long RewardPerBlock { get; set; }
    [Keyword] public string UpdateAddress { get; set; }
    [Keyword] public string StakingToken { get; set; }
    public long FixedBoostFactor { get; set; }
    public long MinimumAmount { get; set; }
    public long ReleasePeriod { get; set; }
    public long MaximumStakeDuration { get; set; }
    [Keyword] public string RewardTokenContract { get; set; }
    [Keyword] public string StakeTokenContract { get; set; }
    public long MinimumClaimAmount { get; set; }
}