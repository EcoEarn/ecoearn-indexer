using Volo.Abp.Application.Dtos;

namespace EcoEarn.Indexer.Plugin.GraphQL.Dto;

public class GetPointsPoolListInput : PagedResultRequestDto
{
    public string Name { get; set; }
    public List<string> PoolIds { get; set; }
}