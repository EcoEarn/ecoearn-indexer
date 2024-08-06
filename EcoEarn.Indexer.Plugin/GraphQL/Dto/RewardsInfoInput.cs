using EcoEarn.Indexer.Plugin.Entities;
using Volo.Abp.Application.Dtos;

namespace EcoEarn.Indexer.Plugin.GraphQL.Dto;

public class RewardsInfoInput : PagedResultRequestDto
{
    public string Address { get; set; }
    public PoolType PoolType { get; set; }
}