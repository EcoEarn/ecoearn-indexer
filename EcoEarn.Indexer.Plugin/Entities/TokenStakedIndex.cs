using AElf.Indexing.Elasticsearch;
using AElfIndexer.Client;
using Nest;

namespace EcoEarn.Indexer.Plugin.Entities;

public class TokenStakedIndex : AElfIndexerClientEntity<string>, IIndexBuild
{
    [Keyword] public string StakeId { get; set; }
    [Keyword] public string PoolId { get; set; }
    [Keyword] public string Account { get; set; }
    [Keyword] public string StakingToken { get; set; }
    public long UnlockTime { get; set; }
    public long LastOperationTime { get; set; }
    public long StakingPeriod { get; set; }
    public List<SubStakeInfo> SubStakeInfos { get; set; }
    
    public long CreateTime { get; set; }
    public long UpdateTime { get; set; }
    public PoolType PoolType { get; set; }
    public LockState LockState { get; set; }

}

public class SubStakeInfo
{
    [Keyword] public string SubStakeId { get; set; }
    public long StakedAmount { get; set; }
    public long StakedBlockNumber { get; set; }
    public long EarlyStakedAmount { get; set; }
    public long StakedTime { get; set; }
    public long Period { get; set; }
    public long BoostedAmount { get; set; }
    public long RewardDebt { get; set; }
    public long RewardAmount { get; set; }
}

public enum LockState
{
    Locking,
    Unlock
}