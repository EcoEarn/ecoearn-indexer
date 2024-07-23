using Volo.Abp.Application.Dtos;

namespace EcoEarn.Indexer.Plugin.GraphQL.Dto;

public class GetUnLockedStakeIdsInput : PagedResultRequestDto
{
    public string Address { get; set; }
    public List<string> StakeIds { get; set; }
}