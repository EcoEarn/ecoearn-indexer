namespace EcoEarn.Indexer.Plugin.GraphQL.Dto;

public class PointsPoolDto
{
    public string DappId { get; set; }
    public string PoolId { get; set; }
    public string PointsName { get; set; }
    public string PoolAddress { get; set; }
    public string Amount { get; set; }
    public PointsPoolConfigDto PointsPoolConfig { get; set; }
    public long CreateTime { get; set; }
}

public class PointsPoolConfigDto
{
    public string RewardToken { get; set; }
    public long StartBlockNumber { get; set; }
    public long EndBlockNumber { get; set; }
    public long RewardPerBlock { get; set; }
    public long ReleasePeriod { get; set; }
    public string UpdateAddress { get; set; }
}

public class PointsPoolDtoList
{
    public long TotalCount { get; set; }

    public List<PointsPoolDto> Data { get; set; }
}