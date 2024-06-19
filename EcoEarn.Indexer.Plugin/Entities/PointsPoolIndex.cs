using AElf.Indexing.Elasticsearch;
using AElfIndexer.Client;
using Nest;

namespace EcoEarn.Indexer.Plugin.Entities;

public class PointsPoolIndex : AElfIndexerClientEntity<string>, IIndexBuild
{
    [Keyword] public string DappId { get; set; }
    [Keyword] public string PoolId { get; set; }
    [Keyword] public string PointsName { get; set; }
    [Keyword] public string PoolAddress { get; set; }
    [Keyword] public string Amount { get; set; }
    public PointsPoolConfig PointsPoolConfig { get; set; }
    public long CreateTime { get; set; }
}

public class PointsPoolConfig
{
    [Keyword] public string RewardToken { get; set; }
    public long StartBlockNumber { get; set; }
    public long EndBlockNumber { get; set; }
    public long RewardPerBlock { get; set; }
    public long ReleasePeriod { get; set; }
    public List<long> ReleasePeriods { get; set; }
    public long ClaimInterval { get; set; }
    [Keyword] public string UpdateAddress { get; set; }
}