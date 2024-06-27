using AElf.Indexing.Elasticsearch;
using AElfIndexer.Client;
using Nest;

namespace EcoEarn.Indexer.Plugin.Entities;

public class LiquidityInfoIndex : AElfIndexerClientEntity<string>, IIndexBuild
{
    [Keyword] public string LiquidityId { get; set; }
    [Keyword] public string StakeId { get; set; }
    [Keyword] public string Address { get; set; }
    [Keyword] public string Seed { get; set; }
    public long LpAmount { get; set; }
    [Keyword] public string LpSymbol { get; set; }
    [Keyword] public string RewardSymbol { get; set; }
    public long TokenAAmount { get; set; }
    [Keyword] public string TokenASymbol { get; set; }
    public long TokenBAmount { get; set; }
    [Keyword] public string TokenBSymbol { get; set; }
    public long AddedTime { get; set; }
    public long RemovedTime { get; set; }
    [Keyword] public string DappId { get; set; }
    [Keyword] public string SwapAddress { get; set; }
    [Keyword] public string TokenAddress { get; set; }
    [Keyword] public string TokenALossAmount { get; set; } = "0";
    [Keyword] public string TokenBLossAmount { get; set; } = "0";
    public LpStatus LpStatus { get; set; }
}

public enum LpStatus
{
    Added,
    Removed
}