using EcoEarn.Indexer.Plugin.Entities;

namespace EcoEarn.Indexer.Plugin.GraphQL.Dto;

public class TokenPoolDto
{
    public string Id { get; set; }
    public string DappId { get; set; }
    public string PoolId { get; set; }
    public string PoolAddress { get; set; }
    public string Amount { get; set; }
    public TokenPoolConfigDto TokenPoolConfig { get; set; }
    public long CreateTime { get; set; }
    public PoolType PoolType { get; set; }
}

public class TokenPoolConfigDto
{
    public string RewardToken { get; set; }
    public long StartBlockNumber { get; set; }
    public long EndBlockNumber { get; set; }
    public long RewardPerBlock { get; set; }
    public string StakingToken { get; set; }
    public long FixedBoostFactor { get; set; }
    public long MinimumAmount { get; set; }
    public long ReleasePeriod { get; set; }
    public long MaximumStakeDuration { get; set; }
    public string RewardTokenContract { get; set; }
    public string StakeTokenContract { get; set; }
    public long MinimumClaimAmount { get; set; }
    public long UnlockWindowDuration { get; set; }
}

public class TokenPoolDtoList
{
    public long TotalCount { get; set; }

    public List<TokenPoolDto> Data { get; set; }
}