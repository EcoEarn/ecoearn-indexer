using EcoEarn.Indexer.Plugin.Entities;

namespace EcoEarn.Indexer.Plugin.GraphQL.Dto;

public class StakedInfoDto
{
    public string Id { get; set; }
    public string StakeId { get; set; }
    public string PoolId { get; set; }
    public string Account { get; set; }
    public string StakingToken { get; set; }
    public long UnlockTime { get; set; }
    public long LastOperationTime { get; set; }
    public long StakingPeriod { get; set; }
    public List<SubStakeInfoDto> SubStakeInfos { get; set; }
    public long CreateTime { get; set; }
    public long UpdateTime { get; set; }
    public PoolType PoolType { get; set; }
    public LockState LockState { get; set; }
}


public class SubStakeInfoDto
{
    public string SubStakeId { get; set; }
    public long StakedAmount { get; set; }
    public long StakedBlockNumber { get; set; }
    public long StakedTime { get; set; }
    public long Period { get; set; }
    public long BoostedAmount { get; set; }
    public long RewardDebt { get; set; }
    public long RewardAmount { get; set; }
    public long EarlyStakedAmount { get; set; }
}
public class StakedInfoDtoList
{
    public long TotalCount { get; set; }

    public List<StakedInfoDto> Data { get; set; }
}