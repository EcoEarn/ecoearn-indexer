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

public class RewardsClaimedLogEventProcessor : AElfLogEventProcessorBase<Claimed, LogEventInfo>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly RewardsMergeInfoOptions _rewardsMergeInfoOptions;
    private readonly ILogger<RewardsClaimedLogEventProcessor> _logger;
    private readonly IAElfIndexerClientEntityRepository<RewardsClaimIndex, LogEventInfo> _repository;
    private readonly IAElfIndexerClientEntityRepository<TokenPoolIndex, LogEventInfo> _tokenPoolRepository;
    private readonly IAElfIndexerClientEntityRepository<RewardsMergeIndex, LogEventInfo> _mergeRewardsRepository;
    private readonly IAElfIndexerClientEntityRepository<RewardsInfoIndex, LogEventInfo> _rewardsInfoRepository;
    private readonly IAElfIndexerClientEntityRepository<PointsPoolIndex, LogEventInfo> _pointsPoolRepository;

    public RewardsClaimedLogEventProcessor(ILogger<RewardsClaimedLogEventProcessor> logger,
        IObjectMapper objectMapper, IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<RewardsClaimIndex, LogEventInfo> pointsRewardsClaimRepository,
        IAElfIndexerClientEntityRepository<TokenPoolIndex, LogEventInfo> tokenPoolRepository,
        IAElfIndexerClientEntityRepository<RewardsMergeIndex, LogEventInfo> mergeRewardsRepository,
        IOptionsSnapshot<RewardsMergeInfoOptions> rewardsMergeInfoOptions,
        IAElfIndexerClientEntityRepository<RewardsInfoIndex, LogEventInfo> rewardsInfoRepository,
        IAElfIndexerClientEntityRepository<PointsPoolIndex, LogEventInfo> pointsPoolRepository) :
        base(logger)
    {
        _logger = logger;
        _contractInfoOptions = contractInfoOptions.Value;
        _objectMapper = objectMapper;
        _repository = pointsRewardsClaimRepository;
        _tokenPoolRepository = tokenPoolRepository;
        _mergeRewardsRepository = mergeRewardsRepository;
        _rewardsInfoRepository = rewardsInfoRepository;
        _pointsPoolRepository = pointsPoolRepository;
        _rewardsMergeInfoOptions = rewardsMergeInfoOptions.Value;
    }

    public override string GetContractAddress(string chainId)
    {
        return _contractInfoOptions.ContractInfos.First(c => c.ChainId == chainId).EcoEarnRewardsContractAddress;
    }

    protected override async Task HandleEventAsync(Claimed eventValue, LogEventContext context)
    {
        _logger.Debug("RewardsClaimed: {eventValue} context: {context}",
            JsonConvert.SerializeObject(eventValue), JsonConvert.SerializeObject(context));
        var now = DateTime.UtcNow.ToUtcMilliSeconds();

        var poolIdHash = eventValue.ClaimInfos.Data.First()?.PoolId;
        var poolId = poolIdHash == null ? "" : poolIdHash.ToHex();
        var address = eventValue.ClaimInfos.Data.First()?.Account;
        var account = address == null ? "" : address.ToBase58();

        var poolType = PoolType.Points;
        string dappId;
        var pointsPoolIndex =
            await _pointsPoolRepository.GetFromBlockStateSetAsync(poolId, context.ChainId);
        if (pointsPoolIndex != null)
        {
            dappId = pointsPoolIndex.DappId;
        }
        else
        {
            var tokenPoolIndex =
                await _tokenPoolRepository.GetFromBlockStateSetAsync(poolId, context.ChainId);
            poolType = tokenPoolIndex.PoolType;
            dappId = tokenPoolIndex.DappId;
        }

        var mergedRewardsList = await GetMergedRewardsList(account, poolId, poolType, dappId);
        var releasedClaimedIds = mergedRewardsList
            .Where(x => x.ReleaseTime <= now)
            .SelectMany(x => x.MergeClaimInfos.Select(m => m.ClaimId))
            .ToList();

        var reMergeRewardsList = mergedRewardsList.Where(x => x.ReleaseTime > now).ToList();
        foreach (var rewardsMergeIndex in reMergeRewardsList)
        {
            _objectMapper.Map(context, rewardsMergeIndex);
            await _mergeRewardsRepository.DeleteAsync(rewardsMergeIndex);
        }

        var unWithdrawRewardsList = await GetUnWithdrawRewardsList(account, poolId, poolType, dappId);
        var unReleasedRewardsList = unWithdrawRewardsList.Where(x => !releasedClaimedIds.Contains(x.ClaimId)).ToList();

        var newRewardsList = new List<RewardsClaimIndex>();
        foreach (var claimInfo in eventValue.ClaimInfos.Data)
        {
            try
            {
                var id = IdGenerateHelper.GetId(claimInfo.ClaimId.ToHex());

                var rewardsClaim = new RewardsClaimIndex
                {
                    Id = id,
                    ClaimId = claimInfo.ClaimId == null ? "" : claimInfo.ClaimId.ToHex(),
                    PoolId = claimInfo.PoolId == null ? "" : claimInfo.PoolId.ToHex(),
                    DappId = dappId,
                    ClaimedAmount = claimInfo.ClaimedAmount.ToString(),
                    ClaimedSymbol = claimInfo.ClaimedSymbol,
                    ClaimedBlockNumber = claimInfo.ClaimedBlockNumber,
                    Account = claimInfo.Account.ToBase58(),
                    ClaimedTime = claimInfo.ClaimedTime == null
                        ? 0
                        : claimInfo.ClaimedTime.ToDateTime().ToUtcMilliSeconds(),
                    ReleaseTime = claimInfo.ReleaseTime == null
                        ? 0
                        : claimInfo.ReleaseTime.ToDateTime().ToUtcMilliSeconds(),
                    Seed = claimInfo.Seed == null ? "" : claimInfo.Seed.ToHex(),
                    ContractAddress = claimInfo.ContractAddress == null ? "" : claimInfo.ContractAddress.ToBase58(),
                    PoolType = poolType
                };
                _objectMapper.Map(context, rewardsClaim);
                await _repository.AddOrUpdateAsync(rewardsClaim);
                newRewardsList.Add(rewardsClaim);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "RewardsClaimed HandleEventAsync error.");
            }
        }

        unReleasedRewardsList.AddRange(newRewardsList);
        var rewardsMergeList = MergeRewards(unReleasedRewardsList, _rewardsMergeInfoOptions.MergeDays, now);
        foreach (var rewardsMergeIndex in rewardsMergeList)
        {
            _objectMapper.Map(context, rewardsMergeIndex);
            await _mergeRewardsRepository.AddOrUpdateAsync(rewardsMergeIndex);
        }

        var rewardsInfoIndex = _objectMapper.Map<RewardsClaimIndex, RewardsInfoIndex>(newRewardsList.First());
        var amountSum = newRewardsList.Sum(x => long.Parse(x.ClaimedAmount));
        rewardsInfoIndex.ClaimedAmount = amountSum.ToString();
        _objectMapper.Map(context, rewardsInfoIndex);
        await _rewardsInfoRepository.AddOrUpdateAsync(rewardsInfoIndex);
    }

    private List<RewardsMergeIndex> MergeRewards(List<RewardsClaimIndex> rewards, double mergeDays, long now)
    {
        rewards = rewards.OrderBy(r => r.ReleaseTime).ToList();
        var mergedRewards = new List<RewardsMergeIndex>();

        RewardsMergeIndex currentMerge = null;

        foreach (var reward in rewards)
        {
            if (currentMerge == null)
            {
                currentMerge = new RewardsMergeIndex
                {
                    Amount = reward.ClaimedAmount,
                    ReleaseTime = reward.ReleaseTime,
                    MergeClaimInfos = new List<MergeClaimInfo>
                    {
                        new()
                        {
                            ClaimId = reward.ClaimId,
                            ClaimedAmount = reward.ClaimedAmount
                        }
                    }
                };
            }
            else
            {
                var currentReleaseDate = DateTimeOffset.FromUnixTimeMilliseconds(currentMerge.ReleaseTime);
                var rewardReleaseDate = DateTimeOffset.FromUnixTimeMilliseconds(reward.ReleaseTime);
                var daysDifference = (rewardReleaseDate - currentReleaseDate).TotalDays;

                if (daysDifference <= mergeDays)
                {
                    currentMerge.Amount =
                        (long.Parse(currentMerge.Amount) + long.Parse(reward.ClaimedAmount)).ToString();
                    currentMerge.ReleaseTime = reward.ReleaseTime;
                    currentMerge.MergeClaimInfos.Add(new MergeClaimInfo
                    {
                        ClaimId = reward.ClaimId,
                        ClaimedAmount = reward.ClaimedAmount
                    });
                }
                else
                {
                    mergedRewards.Add(currentMerge);
                    currentMerge = new RewardsMergeIndex
                    {
                        Amount = reward.ClaimedAmount,
                        ReleaseTime = reward.ReleaseTime,
                        MergeClaimInfos = new List<MergeClaimInfo>
                        {
                            new()
                            {
                                ClaimId = reward.ClaimId,
                                ClaimedAmount = reward.ClaimedAmount
                            }
                        }
                    };
                }
            }

            currentMerge.Account = reward.Account;
            currentMerge.PoolType = reward.PoolType;
            currentMerge.CreateTime = now;
            currentMerge.Id = Guid.NewGuid().ToString();
            currentMerge.DappId = reward.DappId;
            currentMerge.PoolId = reward.PoolId;
        }

        if (currentMerge != null)
        {
            mergedRewards.Add(currentMerge);
        }

        return mergedRewards;
    }

    private async Task<List<RewardsClaimIndex>> GetUnWithdrawRewardsList(string address, string poolId, PoolType poolType,
        string dappId)
    {
        var res = new List<RewardsClaimIndex>();
        var skipCount = 0;
        var maxResultCount = 5000;
        List<RewardsClaimIndex> list;
        do
        {
            var mustQuery = new List<Func<QueryContainerDescriptor<RewardsClaimIndex>, QueryContainer>>();

            mustQuery.Add(q => q.Term(i => i.Field(f => f.Account).Value(address)));
            if (poolType == PoolType.Points)
            {
                mustQuery.Add(q => q.Term(i => i.Field(f => f.DappId).Value(dappId)));
                mustQuery.Add(q => q.Term(i => i.Field(f => f.PoolType).Value(poolType)));
            }
            else
            {
                mustQuery.Add(q => q.Term(i => i.Field(f => f.PoolId).Value(poolId)));
            }
            mustQuery.Add(q => q.Term(i => i.Field(f => f.WithdrawTime).Value(0)));

            QueryContainer Filter(QueryContainerDescriptor<RewardsClaimIndex> f) => f.Bool(b => b.Must(mustQuery));

            var recordList = await _repository.GetListAsync(Filter, skip: skipCount, limit: maxResultCount,
                sortType: SortOrder.Descending, sortExp: o => o.ClaimedTime);
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

    private async Task<List<RewardsMergeIndex>> GetMergedRewardsList(string address, string poolId, PoolType poolType,
        string dappId)
    {
        var res = new List<RewardsMergeIndex>();
        var skipCount = 0;
        var maxResultCount = 5000;
        List<RewardsMergeIndex> list;
        do
        {
            var mustQuery = new List<Func<QueryContainerDescriptor<RewardsMergeIndex>, QueryContainer>>();

            mustQuery.Add(q => q.Term(i => i.Field(f => f.Account).Value(address)));
            if (poolType == PoolType.Points)
            {
                mustQuery.Add(q => q.Term(i => i.Field(f => f.DappId).Value(dappId)));
                mustQuery.Add(q => q.Term(i => i.Field(f => f.PoolType).Value(poolType)));
            }
            else
            {
                mustQuery.Add(q => q.Term(i => i.Field(f => f.PoolId).Value(poolId)));
            }

            QueryContainer Filter(QueryContainerDescriptor<RewardsMergeIndex> f) => f.Bool(b => b.Must(mustQuery));

            var recordList = await _mergeRewardsRepository.GetListAsync(Filter, skip: skipCount, limit: maxResultCount,
                sortType: SortOrder.Descending, sortExp: o => o.CreateTime);
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