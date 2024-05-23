using Volo.Abp.Application.Dtos;

namespace EcoEarn.Indexer.Plugin.GraphQL.Dto;

public class GetRealClaimInfoInput : PagedResultRequestDto
{
    public List<string> Seeds { get; set; }
    public string Address { get; set; }
    public string PoolId { get; set; }
}