using EcoEarn.Indexer.Plugin.Entities;
using Volo.Abp.Application.Dtos;

namespace EcoEarn.Indexer.Plugin.GraphQL.Dto;

public class GetLiquidityInfoInput  : PagedResultRequestDto
{
    public List<string> LiquidityIds { get; set; }
    public string Address { get; set; }
    public LpStatus LpStatus { get; set; }
}