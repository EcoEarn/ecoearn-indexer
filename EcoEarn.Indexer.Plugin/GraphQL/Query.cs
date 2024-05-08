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
    public static async Task<TokenPoolDtoList> GetTokenPoolList(
        [FromServices] IAElfIndexerClientEntityRepository<TokenPoolIndex, LogEventInfo> repository,
        [FromServices] IObjectMapper objectMapper,
        GetTokenPoolListInput input)
    {
        var mustQuery = new List<Func<QueryContainerDescriptor<TokenPoolIndex>, QueryContainer>>();

        mustQuery.Add(q => q.Term(i => i.Field(f => f.PoolType).Value(input.PoolType)));

        if (!string.IsNullOrEmpty(input.TokenName))
        {
            mustQuery.Add(q => q.Term(i => i.Field(f => f.TokenPoolConfig.RewardToken).Value(input.TokenName)));
        }

        QueryContainer Filter(QueryContainerDescriptor<TokenPoolIndex> f) =>
            f.Bool(b => b.Must(mustQuery));

        var recordList = await repository.GetListAsync(Filter, skip: input.SkipCount, limit: input.MaxResultCount);

        var dataList =
            objectMapper.Map<List<TokenPoolIndex>, List<TokenPoolDto>>(recordList.Item2);
        return new TokenPoolDtoList
        {
            Data = dataList,
            TotalCount = recordList.Item1
        };
    }

    [Name("getStakedInfoList")]
    public static async Task<StakedInfoDtoList> GetStakedInfoList(
        [FromServices] IAElfIndexerClientEntityRepository<TokenStakedIndex, LogEventInfo> repository,
        [FromServices] IObjectMapper objectMapper,
        GetStakedInfoListInput input)
    {
        var mustQuery = new List<Func<QueryContainerDescriptor<TokenStakedIndex>, QueryContainer>>();


        if (!string.IsNullOrEmpty(input.TokenName))
        {
            mustQuery.Add(q => q.Term(i => i.Field(f => f.StakingToken).Value(input.TokenName)));
        }

        if (!string.IsNullOrEmpty(input.Address))
        {
            mustQuery.Add(q => q.Term(i => i.Field(f => f.Account).Value(input.Address)));
        }

        if (!input.PoolIds.IsNullOrEmpty())
        {
            mustQuery.Add(q => q.Terms(i => i.Field(f => f.PoolId).Terms(input.PoolIds)));
        }


        QueryContainer Filter(QueryContainerDescriptor<TokenStakedIndex> f) =>
            f.Bool(b => b.Must(mustQuery));

        var recordList = await repository.GetListAsync(Filter, skip: input.SkipCount, limit: input.MaxResultCount);

        var dataList =
            objectMapper.Map<List<TokenStakedIndex>, List<StakedInfoDto>>(recordList.Item2);
        return new StakedInfoDtoList
        {
            Data = dataList,
            TotalCount = recordList.Item1
        };
    }

    [Name("getClaimInfoList")]
    public static async Task<ClaimInfoDtoList> GetClaimInfoList(
        [FromServices] IAElfIndexerClientEntityRepository<RewardsClaimIndex, LogEventInfo> repository,
        [FromServices] IObjectMapper objectMapper,
        GetClaimInfoInput input)
    {
        var mustQuery = new List<Func<QueryContainerDescriptor<RewardsClaimIndex>, QueryContainer>>();

        mustQuery.Add(q => q.Term(i => i.Field(f => f.PoolType).Value(input.PoolType)));

        if (input.FilterUnlock)
        {
            mustQuery.Add(q =>
                q.LongRange(i => i.Field(f => f.UnlockTime).LessThan(DateTime.UtcNow.ToUtcMilliSeconds())));
        }

        QueryContainer Filter(QueryContainerDescriptor<RewardsClaimIndex> f) =>
            f.Bool(b => b.Must(mustQuery));

        var recordList = await repository.GetListAsync(Filter, skip: input.SkipCount, limit: input.MaxResultCount);

        var dataList =
            objectMapper.Map<List<RewardsClaimIndex>, List<ClaimInfoDto>>(recordList.Item2);
        return new ClaimInfoDtoList
        {
            Data = dataList,
            TotalCount = recordList.Item1
        };
    }
}