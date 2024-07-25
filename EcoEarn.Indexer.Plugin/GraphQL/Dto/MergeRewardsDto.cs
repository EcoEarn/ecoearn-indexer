using EcoEarn.Indexer.Plugin.Entities;

namespace EcoEarn.Indexer.Plugin.GraphQL.Dto;

public class MergeRewardsDto
{
    public string Amount { get; set; }
    public string Account { get; set; }
    public PoolType PoolType { get; set; }
    public List<MergeClaimInfoDto> MergeClaimInfos { get; set; }
    public long ReleaseTime { get; set; }
    public long CreateTime { get; set; }
}

public class MergeClaimInfoDto
{
    public string ClaimId { get; set; }
    public string ClaimedAmount { get; set; }
}

public class MergeRewardsListDto
{
    public long TotalCount { get; set; }

    public List<MergeRewardsDto> Data { get; set; }
}