using AElfIndexer.Client;
using AElfIndexer.Grains.State.Client;
using EcoEarn.Indexer.Plugin.Entities;
using EcoEarn.Indexer.Plugin.GraphQL.Dto;
using GraphQL;
using Nest;
using Volo.Abp.ObjectMapping;

namespace EcoEarn.Indexer.Plugin.GraphQL;

public partial class Query
{
    [Name("getPointsPoolList")]
    public static async Task<PointsPoolDtoList> GetPointsPoolList(
        [FromServices] IAElfIndexerClientEntityRepository<PointsPoolIndex, LogEventInfo> repository,
        [FromServices] IObjectMapper objectMapper,
        GetPointsPoolListInput input)
    {
        var mustQuery = new List<Func<QueryContainerDescriptor<PointsPoolIndex>, QueryContainer>>();

        if (string.IsNullOrEmpty(input.Name))
        {
            mustQuery.Add(q => q.Term(i => i.Field(f => f.PointsName).Value(input.Name)));
        }

        QueryContainer Filter(QueryContainerDescriptor<PointsPoolIndex> f) =>
            f.Bool(b => b.Must(mustQuery));

        var recordList = await repository.GetListAsync(Filter, skip: input.SkipCount, limit: input.MaxResultCount);

        var dataList =
            objectMapper.Map<List<PointsPoolIndex>, List<PointsPoolDto>>(recordList.Item2);
        return new PointsPoolDtoList
        {
            Data = dataList,
            TotalCount = recordList.Item1
        };
    }

    [Name("getTokenPoolList")]
    public static async Task<PointsPoolDtoList> GetTokenPoolList(
        [FromServices] IAElfIndexerClientEntityRepository<TokenPoolIndex, LogEventInfo> repository,
        [FromServices] IObjectMapper objectMapper,
        GetPointsPoolListInput input)
    {
        var mustQuery = new List<Func<QueryContainerDescriptor<TokenPoolIndex>, QueryContainer>>();

        QueryContainer Filter(QueryContainerDescriptor<TokenPoolIndex> f) =>
            f.Bool(b => b.Must(mustQuery));

        var recordList = await repository.GetListAsync(Filter, skip: input.SkipCount, limit: input.MaxResultCount);

        var dataList =
            objectMapper.Map<List<TokenPoolIndex>, List<PointsPoolDto>>(recordList.Item2);
        return new PointsPoolDtoList
        {
            Data = dataList,
            TotalCount = recordList.Item1
        };
    }
}