using AElf.Indexing.Elasticsearch;
using AElfIndexer.Client;
using Nest;

namespace EcoEarn.Indexer.Plugin.Entities;

public class RewardsClaimIndex : AElfIndexerClientEntity<string>, IIndexBuild
{
    [Keyword] public string ClaimId { get; set; }
    [Keyword] public string Seed { get; set; }
    [Keyword] public string PoolId { get; set; }
    [Keyword] public string ClaimedAmount { get; set; }
    [Keyword] public string EarlyStakedAmount { get; set; } = "0";
    [Keyword] public string ClaimedSymbol { get; set; }
    public long ClaimedBlockNumber { get; set; }
    public long ClaimedTime { get; set; }
    public long ReleaseTime { get; set; }
    public long WithdrawTime { get; set; }
    public long EarlyStakeTime { get; set; }
    [Keyword] public string Account { get; set; }
    public PoolType PoolType { get; set; }
    [Keyword] public string WithdrawSeed { get; set; } = "";
    [Keyword] public string ContractAddress { get; set; } = "";
    
    [Nested(Name = "LiquidityAddedInfos", Enabled = true, IncludeInParent = true, IncludeInRoot = true)]
    public List<LiquidityAddedInfo> LiquidityAddedInfos { get; set; } = new();
    
    [Nested(Name = "EarlyStakeInfos", Enabled = true, IncludeInParent = true, IncludeInRoot = true)]
    public List<EarlyStakeInfo> EarlyStakeInfos { get; set; } = new();
}


public class LiquidityAddedInfo
{
    [Keyword] public string LiquidityAddedSeed { get; set; } = "";
    [Keyword] public string LiquidityId { get; set; } = "";
    [Keyword] public string TokenALossAmount { get; set; } = "0";
    [Keyword] public string TokenBLossAmount { get; set; } = "0";
    public long AddedTime { get; set; }
}

public class EarlyStakeInfo
{
    [Keyword] public string EarlyStakeSeed { get; set; } = "";
    [Keyword] public string StakeId { get; set; } = "";
    public long StakeTime { get; set; }
}

public enum PoolType
{
    Points = 0,
    Token = 1,
    Lp = 2,
    All = -1,
}