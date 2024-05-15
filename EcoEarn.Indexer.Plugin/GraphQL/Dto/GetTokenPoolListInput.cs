using EcoEarn.Indexer.Plugin.Entities;
using Volo.Abp.Application.Dtos;

namespace EcoEarn.Indexer.Plugin.GraphQL.Dto;

public class GetTokenPoolListInput : PagedResultRequestDto
{
    public string TokenName { get; set; }
    public PoolType PoolType { get; set; }
    public List<string> PoolIds { get; set; }
}