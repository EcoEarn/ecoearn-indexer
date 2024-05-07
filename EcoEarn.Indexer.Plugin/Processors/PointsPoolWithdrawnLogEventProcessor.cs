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

public class PointsPoolWithdrawnLogEventProcessor : AElfLogEventProcessorBase<Withdrawn, LogEventInfo>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly ILogger<PointsPoolWithdrawnLogEventProcessor> _logger;
    private readonly IAElfIndexerClientEntityRepository<RewardsClaimIndex, LogEventInfo> _repository;

    public PointsPoolWithdrawnLogEventProcessor(ILogger<PointsPoolWithdrawnLogEventProcessor> logger,
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
        return _contractInfoOptions.ContractInfos.First(c => c.ChainId == chainId).EcoEarnContractAddress;
    }

    protected override async Task HandleEventAsync(Withdrawn eventValue, LogEventContext context)
    {
        _logger.Debug("PointsPoolWithdrawn: {eventValue} context: {context}",
            JsonConvert.SerializeObject(eventValue), JsonConvert.SerializeObject(context));
        foreach (var claimInfo in eventValue.ClaimInfos.Data)
        {
            try
            {
                var id = IdGenerateHelper.GetId(claimInfo.ClaimId.ToHex(),
                    claimInfo.PoolId.ToHex());

                var rewardsClaim = new RewardsClaimIndex
                {
                    Id = id,
                    ClaimId = claimInfo.ClaimId.ToHex(),
                    PoolId = claimInfo.PoolId.ToHex(),
                    ClaimedAmount = claimInfo.ClaimedAmount.ToString(),
                    ClaimedSymbol = claimInfo.ClaimedSymbol,
                    ClaimedBlockNumber = claimInfo.ClaimedBlockNumber,
                    ClaimedTime = claimInfo.ClaimedTime,
                    UnlockTime = claimInfo.UnlockTime,
                    WithdrawTime = claimInfo.WithdrawTime,
                    Account = claimInfo.Account.ToString(),
                    EarlyStakeTime = claimInfo.EarlyStakeTime,
                    PoolType = PoolType.Points
                };

                _objectMapper.Map(context, rewardsClaim);
                await _repository.AddOrUpdateAsync(rewardsClaim);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "PointsPoolWithdrawn HandleEventAsync error.");
            }
        }
    }
}