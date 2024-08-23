using AElf.Indexing.Elasticsearch;
using AElfIndexer.Client;
using Nest;

namespace EcoEarn.Indexer.Plugin.Entities;

public class RewardsMergeIndex : AElfIndexerClientEntity<string>, IIndexBuild
{
    [Keyword] public string Amount { get; set; }
    [Keyword] public string Account { get; set; }
    [Keyword] public string PoolId { get; set; } = "";
    [Keyword] public string DappId { get; set; } = "";
    public PoolType PoolType { get; set; }
    public List<MergeClaimInfo> MergeClaimInfos { get; set; }
    public long ReleaseTime { get; set; }
    public long CreateTime { get; set; }
}

public class MergeClaimInfo
{
    [Keyword] public string ClaimId { get; set; }
    [Keyword] public string ClaimedAmount { get; set; }
}