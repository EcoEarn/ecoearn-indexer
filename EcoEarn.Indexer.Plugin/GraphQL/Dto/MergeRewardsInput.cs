using EcoEarn.Indexer.Plugin.Entities;
using Volo.Abp.Application.Dtos;

namespace EcoEarn.Indexer.Plugin.GraphQL.Dto;

public class MergeRewardsInput : PagedResultRequestDto
{
    public string Address { get; set; }
    public List<string> DappIds { get; set; }
    public PoolType PoolType { get; set; }
    public List<string> PoolIds { get; set; }
}