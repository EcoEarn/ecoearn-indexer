using EcoEarn.Indexer.Plugin.Entities;

namespace EcoEarn.Indexer.Plugin.GraphQL.Dto;

public class RewardsInfoDto
{
    public string ClaimId { get; set; }
    public string StakeId { get; set; }
    public string Seed { get; set; }
    public string PoolId { get; set; }
    public string DappId { get; set; }
    public string ClaimedAmount { get; set; }
    public string ClaimedSymbol { get; set; }
    public long ClaimedTime { get; set; }
    public string Account { get; set; }
    public PoolType PoolType { get; set; }
}

public class RewardsInfoListDto
{
    public long TotalCount { get; set; } 

    public List<RewardsInfoDto> Data { get; set; }
}