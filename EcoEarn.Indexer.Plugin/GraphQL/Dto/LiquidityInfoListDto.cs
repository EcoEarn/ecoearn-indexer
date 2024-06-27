using EcoEarn.Indexer.Plugin.Entities;

namespace EcoEarn.Indexer.Plugin.GraphQL.Dto;

public class LiquidityInfoDto
{
    public string LiquidityId { get; set; }
    public string StakeId { get; set; }
    public string Seed { get; set; }
    public string Address { get; set; }
    public long LpAmount { get; set; }
    public string LpSymbol { get; set; }
    public string RewardSymbol { get; set; }
    public long TokenAAmount { get; set; }
    public string TokenASymbol { get; set; }
    public long TokenBAmount { get; set; }
    public string TokenBSymbol { get; set; }
    public long AddedTime { get; set; }
    public long RemovedTime { get; set; }
    public string DappId { get; set; }
    public string SwapAddress { get; set; }
    public string TokenAddress { get; set; }
    public string TokenALossAmount { get; set; }
    public string TokenBLossAmount { get; set; }
    public LpStatus LpStatus { get; set; }
}

public class LiquidityInfoListDto
{
    public long TotalCount { get; set; }

    public List<LiquidityInfoDto> Data { get; set; }
}