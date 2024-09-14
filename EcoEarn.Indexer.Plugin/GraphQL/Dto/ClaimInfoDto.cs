using EcoEarn.Indexer.Plugin.Entities;

namespace EcoEarn.Indexer.Plugin.GraphQL.Dto;

public class ClaimInfoDto
{
    public string Id { get; set; }
    public string ClaimId { get; set; }
    public string PoolId { get; set; }
    public string DappId { get; set; }
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
    public string WithdrawSeed { get; set; }
    public string ContractAddress { get; set; }
    public List<LiquidityAddedInfoDto> LiquidityAddedInfos { get; set; }
    public List<EarlyStakeInfoDto> EarlyStakeInfos { get; set; }
}

public class ClaimInfoDtoList
{
    public long TotalCount { get; set; }

    public List<ClaimInfoDto> Data { get; set; }
}

public class LiquidityAddedInfoDto
{
    public string LiquidityAddedSeed { get; set; }
    public string LiquidityId { get; set; }
    public string TokenALossAmount { get; set; }
    public string TokenBLossAmount { get; set; }
    public long AddedTime { get; set; }
}

public class EarlyStakeInfoDto
{
    public string EarlyStakeSeed { get; set; }
    public string StakeId { get; set; }
    public long StakeTime { get; set; }
}