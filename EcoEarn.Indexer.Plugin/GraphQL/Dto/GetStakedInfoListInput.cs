using EcoEarn.Indexer.Plugin.Entities;
using Volo.Abp.Application.Dtos;

namespace EcoEarn.Indexer.Plugin.GraphQL.Dto;

public class GetStakedInfoListInput : PagedResultRequestDto
{
    public string TokenName { get; set; }
    public string Address { get; set; }
    public List<string> PoolIds { get; set; }
    public LockState LockState { get; set; }

}

public class GetAllStakedInfoListInput
{
    public long FromBlockHeight { get; set; }
    public long ToBlockHeight { get; set; }
    public LockState LockState { get; set; }
}