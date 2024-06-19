using EcoEarn.Indexer.Plugin.Entities;

namespace EcoEarn.Indexer.Plugin.GraphQL.Dto;

public class ClaimInfoDto
{
    public string Id { get; set; }
    public string ClaimId { get; set; }
    public string StakeId { get; set; }
    public string PoolId { get; set; }
    public string ClaimedAmount { get; set; }
    public string Seed { get; set; }
    public string ClaimedSymbol { get; set; }
    public long ClaimedBlockNumber { get; set; }
    public string EarlyStakedAmount { get; set; }
    public long ClaimedTime { get; set; }
    public long ReleaseTime { get; set; }
    public long WithdrawTime { get; set; }
    public long EarlyStakeTime { get; set; }
    public string Account { get; set; }
    public PoolType PoolType { get; set; }
    public LockState LockState { get; set; }
}

public class ClaimInfoDtoList
{
    public long TotalCount { get; set; }

    public List<ClaimInfoDto> Data { get; set; }
}