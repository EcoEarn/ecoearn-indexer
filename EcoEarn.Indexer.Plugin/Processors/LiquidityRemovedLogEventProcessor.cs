using System.Globalization;
using System.Numerics;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using EcoEarn.Contracts.Rewards;
using EcoEarn.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using Newtonsoft.Json;
using Orleans.Runtime;
using Volo.Abp.ObjectMapping;

namespace EcoEarn.Indexer.Plugin.Processors;

public class LiquidityRemovedLogEventProcessor : AElfLogEventProcessorBase<LiquidityRemoved, LogEventInfo>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly ILogger<LiquidityRemovedLogEventProcessor> _logger;
    private readonly IAElfIndexerClientEntityRepository<LiquidityInfoIndex, LogEventInfo> _repository;
    private readonly IAElfIndexerClientEntityRepository<RewardsClaimIndex, LogEventInfo> _rewardsRepository;

    public LiquidityRemovedLogEventProcessor(ILogger<LiquidityRemovedLogEventProcessor> logger,
        IObjectMapper objectMapper, IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<LiquidityInfoIndex, LogEventInfo> repository,
        IAElfIndexerClientEntityRepository<RewardsClaimIndex, LogEventInfo> rewardsRepository) :
        base(logger)
    {
        _logger = logger;
        _contractInfoOptions = contractInfoOptions.Value;
        _objectMapper = objectMapper;
        _repository = repository;
        _rewardsRepository = rewardsRepository;
    }

    public override string GetContractAddress(string chainId)
    {
        return _contractInfoOptions.ContractInfos.First(c => c.ChainId == chainId).EcoEarnRewardsContractAddress;
    }

    protected override async Task HandleEventAsync(LiquidityRemoved eventValue, LogEventContext context)
    {
        try
        {
            var tokenAOldAmount = 0L;
            var tokenBOldAmount = 0L;
            foreach (var liquidityId in eventValue.LiquidityIds.Data)
            {
                var id = IdGenerateHelper.GetId(liquidityId.ToHex());
                var liquidityInfoIndex = await _repository.GetFromBlockStateSetAsync(id, context.ChainId);
                tokenAOldAmount += liquidityInfoIndex.TokenAAmount;
                tokenBOldAmount += liquidityInfoIndex.TokenBAmount;
            }

            _logger.Debug("LiquidityRemoved: {eventValue} context: {context}", JsonConvert.SerializeObject(eventValue),
                JsonConvert.SerializeObject(context));
            var tokenANewAmount = eventValue.TokenAAmount;
            var tokenBNewAmount = eventValue.TokenBAmount;
            var tokenALossAmountSum = tokenANewAmount - tokenAOldAmount;
            var tokenBLossAmountSum = tokenBNewAmount - tokenBOldAmount;
            var removedTime = context.BlockTime.ToUtcMilliSeconds();
            var liquidityIds = eventValue.LiquidityIds.Data.Select(x => x.ToHex()).ToList();
            var rewardsList = await GetRewardsList(liquidityIds);
            var liquidityIdDic = rewardsList
                .SelectMany(dto => dto.LiquidityAddedInfos,
                    (dto, liquidityInfo) => new { dto, liquidityInfo.LiquidityId })
                .GroupBy(x => x.LiquidityId)
                .ToDictionary(g => g.Key, g => g.ToList());
            foreach (var liquidityId in eventValue.LiquidityIds.Data)
            {
                var id = IdGenerateHelper.GetId(liquidityId.ToHex());
                var liquidityInfoIndex = await _repository.GetFromBlockStateSetAsync(id, context.ChainId);
                liquidityInfoIndex.LpStatus = LpStatus.Removed;

                var tokenALossAmount =
                    -((decimal)liquidityInfoIndex.TokenAAmount / tokenAOldAmount) * tokenALossAmountSum;
                var tokenBLossAmount =
                    -((decimal)liquidityInfoIndex.TokenBAmount / tokenBOldAmount) * tokenBLossAmountSum;

                liquidityInfoIndex.TokenALossAmount =
                    Math.Ceiling(tokenALossAmount).ToString(CultureInfo.InvariantCulture);
                liquidityInfoIndex.TokenBLossAmount =
                    Math.Ceiling(tokenBLossAmount).ToString(CultureInfo.InvariantCulture);
                liquidityInfoIndex.RemovedTime = removedTime;
                _objectMapper.Map(context, liquidityInfoIndex);
                await _repository.AddOrUpdateAsync(liquidityInfoIndex);

                if (!liquidityIdDic.TryGetValue(id, out var rewardsDto)) continue;
                var rewards = rewardsDto.Select(x => x.dto).ToList();

                foreach (var rewardsClaimIndex in rewards)
                {
                    var rewardsLossASum = rewardsClaimIndex.LiquidityAddedInfos
                        .Select(x => BigInteger.Parse(x.TokenALossAmount))
                        .Aggregate(BigInteger.Zero, (acc, num) => acc + num).ToString();
                    var rewardsLossBSum = rewardsClaimIndex.LiquidityAddedInfos
                        .Select(x => BigInteger.Parse(x.TokenBLossAmount))
                        .Aggregate(BigInteger.Zero, (acc, num) => acc + num).ToString();
                    var lastAddedA = BigInteger.Parse(rewardsClaimIndex.ClaimedAmount) - BigInteger.Parse(rewardsLossASum);
                    var lastAddedB = BigInteger.Parse(rewardsClaimIndex.ClaimedAmount) - BigInteger.Parse(rewardsLossBSum);
                    var lossARate = double.Parse(lastAddedA.ToString()) /
                                    double.Parse(liquidityInfoIndex.TokenAAmount.ToString());
                    var lossBRate = double.Parse(lastAddedB.ToString()) /
                                    double.Parse(liquidityInfoIndex.TokenBAmount.ToString());
                    var currentLossA = Math.Ceiling(lossARate * double.Parse(liquidityInfoIndex.TokenALossAmount))
                        .ToString(CultureInfo.InvariantCulture);
                    var currentLossB = Math.Ceiling(lossBRate * double.Parse(liquidityInfoIndex.TokenBLossAmount))
                        .ToString(CultureInfo.InvariantCulture);
                    foreach (var liquidityAddedInfo in rewardsClaimIndex.LiquidityAddedInfos.Where(
                                 liquidityAddedInfo => liquidityAddedInfo.LiquidityId == id))
                    {
                        liquidityAddedInfo.TokenALossAmount = currentLossA;
                        liquidityAddedInfo.TokenBLossAmount = currentLossB;
                    }

                    _objectMapper.Map(context, rewardsClaimIndex);
                    await _rewardsRepository.AddOrUpdateAsync(rewardsClaimIndex);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "LiquidityRemoved HandleEventAsync error.");
        }
    }


    private async Task<List<RewardsClaimIndex>> GetRewardsList(List<string> liquidityIds)
    {
        var res = new List<RewardsClaimIndex>();
        var skipCount = 0;
        var maxResultCount = 5000;
        List<RewardsClaimIndex> list;
        do
        {
            var mustQuery = new List<Func<QueryContainerDescriptor<RewardsClaimIndex>, QueryContainer>>();
            mustQuery.Add(n =>
                n.Nested(n => n.Path("LiquidityAddedInfos").Query(q =>
                    q.Terms(i => i.Field("LiquidityAddedInfos.liquidityId").Terms(liquidityIds)))));

            QueryContainer Filter(QueryContainerDescriptor<RewardsClaimIndex> f) => f.Bool(b => b.Must(mustQuery));

            var recordList = await _rewardsRepository.GetListAsync(Filter, skip: skipCount, limit: maxResultCount,
                sortType: SortOrder.Ascending, sortExp: o => o.ClaimedTime);
            list = recordList.Item2;
            var count = list.Count;
            res.AddRange(list);
            if (list.IsNullOrEmpty() || count < maxResultCount)
            {
                break;
            }

            skipCount += count;
        } while (!list.IsNullOrEmpty());

        return res;
    }
}