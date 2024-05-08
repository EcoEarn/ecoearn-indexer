using Volo.Abp.Application.Dtos;

namespace EcoEarn.Indexer.Plugin.GraphQL.Dto;

public class GetStakedInfoListInput : PagedResultRequestDto
{
    public string TokenName { get; set; }
    public string Address { get; set; }
    public List<string> PoolIds { get; set; }
}