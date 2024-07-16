using AElfIndexer.Client;
using AElfIndexer.Client.Providers;
using AElfIndexer.Grains;
using AElfIndexer.Grains.Grain.Client;
using AElfIndexer.Grains.State.Client;
using EcoEarn.Indexer.Plugin.Entities;
using EcoEarn.Indexer.Plugin.GraphQL.Dto;
using GraphQL;
using Microsoft.Extensions.Options;
using Nest;
using Orleans;
using Volo.Abp.ObjectMapping;

namespace EcoEarn.Indexer.Plugin.GraphQL;

public partial class Query
{
    [Name("syncState")]
    public static async Task<SyncStateDto> GetConfirmedBlockHeight([FromServices] IClusterClient clusterClient,
        [FromServices] IAElfIndexerClientInfoProvider clientInfoProvider, SyncStateInput input)
    {
        var version = clientInfoProvider.GetVersion();
        var clientId = clientInfoProvider.GetClientId();
        var blockStateSetInfoGrain =
            clusterClient.GetGrain<IBlockStateSetInfoGrain>(
                GrainIdHelper.GenerateGrainId("BlockStateSetInfo", clientId, input.ChainId, version));
        return new SyncStateDto
        {
            ConfirmedBlockHeight = await blockStateSetInfoGrain.GetConfirmedBlockHeight(input.FilterType)
        };
    }

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

        if (!input.PoolIds.IsNullOrEmpty())
        {
            mustQuery.Add(q => q.Terms(i => i.Field(f => f.PoolId).Terms(input.PoolIds)));
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

        if (input.PoolType != PoolType.All)
        {
            mustQuery.Add(q => q.Term(i => i.Field(f => f.PoolType).Value(input.PoolType)));
        }

        if (!string.IsNullOrEmpty(input.TokenName))
        {
            mustQuery.Add(q => q.Term(i => i.Field(f => f.TokenPoolConfig.StakingToken).Value(input.TokenName)));
        }

        if (!input.PoolIds.IsNullOrEmpty())
        {
            mustQuery.Add(q => q.Terms(i => i.Field(f => f.PoolId).Terms(input.PoolIds)));
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

        mustQuery.Add(q => q.Term(i => i.Field(f => f.LockState).Value(input.LockState)));

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

        mustQuery.Add(q => q.Term(i => i.Field(f => f.Account).Value(input.Address)));

        if (input.PoolType != PoolType.All)
        {
            mustQuery.Add(q => q.Term(i => i.Field(f => f.PoolType).Value(input.PoolType)));
        }

        if (input.FilterUnlock)
        {
            mustQuery.Add(q =>
                q.LongRange(i => i.Field(f => f.ReleaseTime).LessThan(DateTime.UtcNow.ToUtcMilliSeconds())));
        }

        if (input.FilterWithdraw)
        {
            mustQuery.Add(q => q.Term(i => i.Field(f => f.WithdrawTime).Value(0)));
        }

        if (!input.LiquidityIds.IsNullOrEmpty())
        {
            mustQuery.Add(q => q.Terms(i => i.Field(f => f.LiquidityId).Terms(input.LiquidityIds)));

        }

        QueryContainer Filter(QueryContainerDescriptor<RewardsClaimIndex> f) =>
            f.Bool(b => b.Must(mustQuery));

        var recordList = await repository.GetListAsync(Filter, skip: input.SkipCount, limit: input.MaxResultCount,
            sortType: SortOrder.Descending, sortExp: o => o.ClaimedTime);

        var dataList =
            objectMapper.Map<List<RewardsClaimIndex>, List<ClaimInfoDto>>(recordList.Item2);
        return new ClaimInfoDtoList
        {
            Data = dataList,
            TotalCount = recordList.Item1
        };
    }
    
    [Name("getClaimInfoCount")]
    public static async Task<long> GetClaimInfoCount(
        [FromServices] IAElfIndexerClientEntityRepository<RewardsClaimIndex, LogEventInfo> repository,
        [FromServices] IObjectMapper objectMapper,
        GetClaimInfoInput input)
    {
        var mustQuery = new List<Func<QueryContainerDescriptor<RewardsClaimIndex>, QueryContainer>>();

        mustQuery.Add(q => q.Term(i => i.Field(f => f.Account).Value(input.Address)));

        if (input.PoolType != PoolType.All)
        {
            mustQuery.Add(q => q.Term(i => i.Field(f => f.PoolType).Value(input.PoolType)));
        }

        if (input.FilterUnlock)
        {
            mustQuery.Add(q =>
                q.LongRange(i => i.Field(f => f.ReleaseTime).LessThan(DateTime.UtcNow.ToUtcMilliSeconds())));
        }

        if (input.FilterWithdraw)
        {
            mustQuery.Add(q => q.Term(i => i.Field(f => f.WithdrawTime).Value(0)));
        }

        if (!input.LiquidityIds.IsNullOrEmpty())
        {
            mustQuery.Add(q => q.Terms(i => i.Field(f => f.LiquidityId).Terms(input.LiquidityIds)));

        }

        QueryContainer Filter(QueryContainerDescriptor<RewardsClaimIndex> f) =>
            f.Bool(b => b.Must(mustQuery));

        var countResponse = await repository.CountAsync(Filter);
        return countResponse.Count;
    }

    [Name("getUnLockedStakeIdsAsync")]
    public static async Task<List<string>> GetUnLockedStakeIdsAsync(
        [FromServices] IAElfIndexerClientEntityRepository<TokenStakedIndex, LogEventInfo> repository,
        [FromServices] IObjectMapper objectMapper,
        GetUnLockedStakeIdsInput input)
    {
        var mustQuery = new List<Func<QueryContainerDescriptor<TokenStakedIndex>, QueryContainer>>();

        if (!string.IsNullOrEmpty(input.Address))
        {
            mustQuery.Add(q => q.Term(i => i.Field(f => f.Account).Value(input.Address)));
        }

        if (!input.StakeIds.IsNullOrEmpty())
        {
            mustQuery.Add(q => q.Terms(i => i.Field(f => f.StakeId).Terms(input.StakeIds)));
        }


        QueryContainer Filter(QueryContainerDescriptor<TokenStakedIndex> f) =>
            f.Bool(b => b.Must(mustQuery));

        var recordList = await repository.GetListAsync(Filter, skip: input.SkipCount, limit: input.MaxResultCount);

        var dataList =
            objectMapper.Map<List<TokenStakedIndex>, List<StakedInfoDto>>(recordList.Item2);

        return dataList.Where(x => x.LockState == LockState.Unlock)
            .Select(x => x.StakeId)
            .ToList();
    }


    [Name("getAllStakedInfoList")]
    public static async Task<StakedInfoDtoList> GetAllStakedInfoList(
        [FromServices] IAElfIndexerClientEntityRepository<TokenStakedIndex, LogEventInfo> repository,
        [FromServices] IObjectMapper objectMapper,
        GetAllStakedInfoListInput input)
    {
        var mustQuery = new List<Func<QueryContainerDescriptor<TokenStakedIndex>, QueryContainer>>();

        mustQuery.Add(q => q.Term(i => i.Field(f => f.LockState).Value(input.LockState)));

        if (input.FromBlockHeight > 0)
        {
            mustQuery.Add(q => q.Range(i => i.Field(f => f.BlockHeight).GreaterThanOrEquals(input.FromBlockHeight)));
        }

        if (input.ToBlockHeight > 0)
        {
            mustQuery.Add(q => q.Range(i => i.Field(f => f.BlockHeight).LessThanOrEquals(input.ToBlockHeight)));
        }


        QueryContainer Filter(QueryContainerDescriptor<TokenStakedIndex> f) =>
            f.Bool(b => b.Must(mustQuery));

        var recordList = await repository.GetListAsync(Filter);

        var dataList =
            objectMapper.Map<List<TokenStakedIndex>, List<StakedInfoDto>>(recordList.Item2);
        return new StakedInfoDtoList
        {
            Data = dataList,
            TotalCount = recordList.Item1
        };
    }

    [Name("getTokenPoolStakeInfoList")]
    public static async Task<TokenPoolStakeInfoListDto> GetTokenPoolStakeInfoList(
        [FromServices] IAElfIndexerClientEntityRepository<TokenPoolStakeInfoIndex, LogEventInfo> repository,
        [FromServices] IObjectMapper objectMapper,
        GetTokenPoolStakeInfoInput input)
    {
        var mustQuery = new List<Func<QueryContainerDescriptor<TokenPoolStakeInfoIndex>, QueryContainer>>();

        if (!input.PoolIds.IsNullOrEmpty())
        {
            mustQuery.Add(q => q.Terms(i => i.Field(f => f.PoolId).Terms(input.PoolIds)));
        }

        QueryContainer Filter(QueryContainerDescriptor<TokenPoolStakeInfoIndex> f) =>
            f.Bool(b => b.Must(mustQuery));

        var recordList = await repository.GetListAsync(Filter);

        var dataList =
            objectMapper.Map<List<TokenPoolStakeInfoIndex>, List<TokenPoolStakeInfoDto>>(recordList.Item2);
        return new TokenPoolStakeInfoListDto
        {
            Data = dataList,
            TotalCount = recordList.Item1
        };
    }

    [Name("getRealClaimInfoList")]
    public static async Task<ClaimInfoDtoList> GetRealClaimInfoList(
        [FromServices] IAElfIndexerClientEntityRepository<RewardsClaimIndex, LogEventInfo> repository,
        [FromServices] IObjectMapper objectMapper,
        GetRealClaimInfoInput input)
    {
        var mustQuery = new List<Func<QueryContainerDescriptor<RewardsClaimIndex>, QueryContainer>>();

        mustQuery.Add(q => q.Term(i => i.Field(f => f.Account).Value(input.Address)));

        if (!input.Seeds.IsNullOrEmpty())
        {
            var shouldQuery = new List<Func<QueryContainerDescriptor<RewardsClaimIndex>, QueryContainer>>();
            shouldQuery.Add(q => q.Terms(i => i.Field(f => f.Seed).Terms(input.Seeds)));
            shouldQuery.Add(q => q.Terms(i => i.Field(f => f.WithdrawSeed).Terms(input.Seeds)));
            shouldQuery.Add(q => q.Terms(i => i.Field(f => f.EarlyStakeSeed).Terms(input.Seeds)));
            shouldQuery.Add(q => q.Terms(i => i.Field(f => f.LiquidityAddedSeed).Terms(input.Seeds)));
            mustQuery.Add(q => q.Bool(b => b.Should(shouldQuery)));
        }

        if (!string.IsNullOrEmpty(input.PoolId))
        {
            mustQuery.Add(q => q.Term(i => i.Field(f => f.PoolId).Value(input.PoolId)));
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


    [Name("getLiquidityInfoList")]
    public static async Task<LiquidityInfoListDto> GetLiquidityInfoList(
        [FromServices] IAElfIndexerClientEntityRepository<LiquidityInfoIndex, LogEventInfo> repository,
        [FromServices] IObjectMapper objectMapper,
        GetLiquidityInfoInput input)
    {
        var mustQuery = new List<Func<QueryContainerDescriptor<LiquidityInfoIndex>, QueryContainer>>();

        mustQuery.Add(q => q.Term(i => i.Field(f => f.LpStatus).Value(input.LpStatus)));

        if (!input.LiquidityIds.IsNullOrEmpty())
        {
            mustQuery.Add(q => q.Terms(i => i.Field(f => f.LiquidityId).Terms(input.LiquidityIds)));
        }

        if (!string.IsNullOrEmpty(input.Address))
        {
            mustQuery.Add(q => q.Term(i => i.Field(f => f.Address).Value(input.Address)));
        }


        QueryContainer Filter(QueryContainerDescriptor<LiquidityInfoIndex> f) =>
            f.Bool(b => b.Must(mustQuery));

        var recordList = await repository.GetListAsync(Filter, skip: input.SkipCount, limit: input.MaxResultCount);

        var dataList =
            objectMapper.Map<List<LiquidityInfoIndex>, List<LiquidityInfoDto>>(recordList.Item2);
        return new LiquidityInfoListDto
        {
            Data = dataList,
            TotalCount = recordList.Item1
        };
    }


    [Name("test")]
    public static async Task<PointsPoolDtoList> GetPointsPoolList(
        [FromServices] IAElfIndexerClientEntityRepository<PointsPoolIndex, LogEventInfo> repository,
        [FromServices] IObjectMapper objectMapper,
        [FromServices] IOptionsSnapshot<PoolBlackListOptions> options,
        GetPointsPoolListInput input)
    {
        var mustQuery = new List<Func<QueryContainerDescriptor<PointsPoolIndex>, QueryContainer>>();


        if (string.IsNullOrEmpty(input.Name))
        {
            mustQuery.Add(q => q.Term(i => i.Field(f => f.PointsName).Value(input.Name)));
        }

        if (!input.PoolIds.IsNullOrEmpty())
        {
            mustQuery.Add(q => q.Terms(i => i.Field(f => f.PoolId).Terms(input.PoolIds)));
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
}