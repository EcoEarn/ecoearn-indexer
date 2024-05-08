using EcoEarn.Indexer.Plugin.Entities;
using Volo.Abp.Application.Dtos;

namespace EcoEarn.Indexer.Plugin.GraphQL.Dto;

public class GetClaimInfoInput : PagedResultRequestDto
{
    public PoolType PoolType { get; set; }
    public bool FilterUnlock { get; set; }
}