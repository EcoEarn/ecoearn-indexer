using EcoEarn.Indexer.Plugin.Entities;

namespace EcoEarn.Indexer.Plugin.GraphQL.Dto;

public class StakedInfoDto
{
    public string Id { get; set; }
    public string StakeId { get; set; }
    public string PoolId { get; set; }
    public string StakingToken { get; set; }
    public long StakedAmount { get; set; }
    public long EarlyStakedAmount { get; set; }
    public long ClaimedAmount { get; set; }
    public long StakedBlockNumber { get; set; }
    public long StakedTime { get; set; }
    public long Period { get; set; }
    public string Account { get; set; }
    public long BoostedAmount { get; set; }
    public long RewardDebt { get; set; }
    public long WithdrawTime { get; set; }
    public long RewardAmount { get; set; }
    public long LockedRewardAmount { get; set; }
    public long LastOperationTime { get; set; }
    public long CreateTime { get; set; }
    public long UpdateTime { get; set; }
    public PoolType PoolType { get; set; }
}

public class StakedInfoDtoList
{
    public long TotalCount { get; set; }

    public List<StakedInfoDto> Data { get; set; }
}