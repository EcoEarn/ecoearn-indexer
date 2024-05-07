using AElf.Indexing.Elasticsearch;
using AElfIndexer.Client;
using Nest;
using Timestamp = Google.Protobuf.WellKnownTypes.Timestamp;


namespace EcoEarn.Indexer.Plugin.Entities;

public class TokenStakedIndex : AElfIndexerClientEntity<string>, IIndexBuild
{
    [Keyword] public string StakeId { get; set; }
    [Keyword] public string PoolId { get; set; }
    [Keyword] public string StakingToken { get; set; }
    public long StakedAmount { get; set; }
    public long StakedBlockNumber { get; set; }
    public Timestamp StakedTime { get; set; }
    public long Period { get; set; }
    [Keyword] public string Account { get; set; }
    public long BoostedAmount { get; set; }
    public long RewardDebt { get; set; }
    public Timestamp WithdrawTime { get; set; }
    public long RewardAmount { get; set; }
    public long LockedRewardAmount { get; set; }
    public Timestamp LastOperationTime { get; set; }
    public long CreateTime { get; set; }
    public long UpdateTime { get; set; }
    public PoolType PoolType { get; set; }
}