using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using EcoEarn.Contracts.Rewards;
using EcoEarn.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Orleans.Runtime;
using Volo.Abp.ObjectMapping;

namespace EcoEarn.Indexer.Plugin.Processors;

public class RewardsWithdrawnLogEventProcessor : AElfLogEventProcessorBase<Withdrawn, LogEventInfo>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly ILogger<RewardsWithdrawnLogEventProcessor> _logger;
    private readonly IAElfIndexerClientEntityRepository<RewardsClaimIndex, LogEventInfo> _repository;

    public RewardsWithdrawnLogEventProcessor(ILogger<RewardsWithdrawnLogEventProcessor> logger,
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
        return _contractInfoOptions.ContractInfos.First(c => c.ChainId == chainId).EcoEarnRewardsContractAddress;
    }

    protected override async Task HandleEventAsync(Withdrawn eventValue, LogEventContext context)
    {
        _logger.Debug("RewardsWithdrawn: {eventValue} context: {context}",
            JsonConvert.SerializeObject(eventValue), JsonConvert.SerializeObject(context));
        foreach (var claimId in eventValue.ClaimIds.Data)
        {
            try
            {
                var id = IdGenerateHelper.GetId(claimId.ToHex());
                var rewardsClaim = await _repository.GetFromBlockStateSetAsync(id, context.ChainId);
                rewardsClaim.WithdrawTime = context.BlockTime.ToUtcMilliSeconds();
                rewardsClaim.WithdrawSeed = eventValue.Seed == null ? "" : eventValue.Seed.ToHex();
                _objectMapper.Map(context, rewardsClaim);
                await _repository.AddOrUpdateAsync(rewardsClaim);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "RewardsWithdrawn HandleEventAsync error.");
            }
        }
    }
}