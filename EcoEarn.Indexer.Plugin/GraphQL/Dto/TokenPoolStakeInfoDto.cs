namespace EcoEarn.Indexer.Plugin.GraphQL.Dto;

public class TokenPoolStakeInfoDto
{
    public string PoolId { get; set; }
    public string AccTokenPerShare { get; set; }
    public string TotalStakedAmount { get; set; }
    public long LastRewardTime { get; set; }
}

public class TokenPoolStakeInfoListDto
{
    public long TotalCount { get; set; }

    public List<TokenPoolStakeInfoDto> Data { get; set; }
}