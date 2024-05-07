using AElf.Indexing.Elasticsearch;
using AElfIndexer.Client;
using Nest;
using Timestamp = Google.Protobuf.WellKnownTypes.Timestamp;

namespace EcoEarn.Indexer.Plugin.Entities;

public class RewardsClaimIndex : AElfIndexerClientEntity<string>, IIndexBuild
{
    [Keyword] public string ClaimId { get; set; }
    [Keyword] public string PoolId { get; set; }
    [Keyword] public string ClaimedAmount { get; set; }
    [Keyword] public string ClaimedSymbol { get; set; }
    public long ClaimedBlockNumber { get; set; }
    public Timestamp ClaimedTime { get; set; }
    public Timestamp UnlockTime { get; set; }
    public Timestamp WithdrawTime { get; set; }
    public Timestamp EarlyStakeTime { get; set; }
    [Keyword] public string Account { get; set; }
    public PoolType PoolType { get; set; }
}

public enum PoolType
{
    Points,
    Token,
    Lp
}