using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using EcoEarn.Contracts.Points;
using EcoEarn.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Orleans.Runtime;
using Volo.Abp.ObjectMapping;

namespace EcoEarn.Indexer.Plugin.Processors;

public class PointsPoolClaimedLogEventProcessor : AElfLogEventProcessorBase<Claimed, LogEventInfo>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly ILogger<PointsPoolClaimedLogEventProcessor> _logger;
    private readonly IAElfIndexerClientEntityRepository<RewardsClaimIndex, LogEventInfo> _repository;

    public PointsPoolClaimedLogEventProcessor(ILogger<PointsPoolClaimedLogEventProcessor> logger,
        IObjectMapper objectMapper, IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<RewardsClaimIndex, LogEventInfo> pointsRewardsClaimRepository) :
        base(logger)
    {
        _logger = logger;
        _contractInfoOptions = contractInfoOptions.Value;
        _objectMapper = objectMapper;
        _repository = pointsRewardsClaimRepository;
    }

    public override string GetContractAddress(string chainId)
    {
        return _contractInfoOptions.ContractInfos.First(c => c.ChainId == chainId).EcoEarnPointsContractAddress;
    }

    protected override async Task HandleEventAsync(Claimed eventValue, LogEventContext context)
    {
        try
        {
            _logger.Debug("PointsPoolClaimed: {eventValue} context: {context}", JsonConvert.SerializeObject(eventValue),
                JsonConvert.SerializeObject(context));
            var id = IdGenerateHelper.GetId(eventValue.ClaimInfo.ClaimId.ToHex(), eventValue.ClaimInfo.PoolId.ToHex());

            var rewardsClaimIndex = new RewardsClaimIndex
            {
                Id = id,
                ClaimId = eventValue.ClaimInfo.ClaimId.ToHex(),
                PoolId = eventValue.ClaimInfo.PoolId.ToHex(),
                ClaimedAmount = eventValue.ClaimInfo.ClaimedAmount.ToString(),
                ClaimedSymbol = eventValue.ClaimInfo.ClaimedSymbol,
                ClaimedBlockNumber = eventValue.ClaimInfo.ClaimedBlockNumber,
                ClaimedTime = eventValue.ClaimInfo.ClaimedTime.ToDateTime().ToUtcMilliSeconds(),
                UnlockTime = eventValue.ClaimInfo.UnlockTime.ToDateTime().ToUtcMilliSeconds(),
                WithdrawTime = eventValue.ClaimInfo.WithdrawTime.ToDateTime().ToUtcMilliSeconds(),
                Account = eventValue.ClaimInfo.Account.ToString(),
                EarlyStakeTime = eventValue.ClaimInfo.EarlyStakeTime.ToDateTime().ToUtcMilliSeconds(),
                PoolType = PoolType.Points
            };

            _objectMapper.Map(context, rewardsClaimIndex);
            await _repository.AddOrUpdateAsync(rewardsClaimIndex);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "PointsPoolClaimed HandleEventAsync error.");
        }
    }
}