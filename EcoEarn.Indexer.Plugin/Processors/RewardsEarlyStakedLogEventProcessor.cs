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

public class RewardsEarlyStakedLogEventProcessor : AElfLogEventProcessorBase<RewardsStaked, LogEventInfo>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly ILogger<RewardsEarlyStakedLogEventProcessor> _logger;
    private readonly IAElfIndexerClientEntityRepository<RewardsClaimIndex, LogEventInfo> _claimRepository;

    public RewardsEarlyStakedLogEventProcessor(ILogger<RewardsEarlyStakedLogEventProcessor> logger,
        IObjectMapper objectMapper, IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<RewardsClaimIndex, LogEventInfo> claimRepository) :
        base(logger)
    {
        _logger = logger;
        _contractInfoOptions = contractInfoOptions.Value;
        _objectMapper = objectMapper;
        _claimRepository = claimRepository;
    }

    public override string GetContractAddress(string chainId)
    {
        return _contractInfoOptions.ContractInfos.First(c => c.ChainId == chainId).EcoEarnRewardsContractAddress;
    }

    protected override async Task HandleEventAsync(RewardsStaked eventValue, LogEventContext context)
    {
        _logger.Debug("EarlyStaked: {eventValue} context: {context}",
            JsonConvert.SerializeObject(eventValue), JsonConvert.SerializeObject(context));
        foreach (var claimId in eventValue.ClaimIds.Data)
        {
            try
            {
                var id = IdGenerateHelper.GetId(claimId.ToHex());
                var rewardsClaim = await _claimRepository.GetFromBlockStateSetAsync(id, context.ChainId);
                var rewardsClaimEarlyStakeInfos = rewardsClaim.EarlyStakeInfos;
                rewardsClaimEarlyStakeInfos.Add(new EarlyStakeInfo()
                {
                    EarlyStakeSeed = eventValue.Seed == null ? "" : eventValue.Seed.ToHex(),
                    StakeId = eventValue.StakeId == null ? "" : eventValue.StakeId.ToHex(),
                    StakeTime = context.BlockTime.ToUtcMilliSeconds()
                });

                _objectMapper.Map(context, rewardsClaim);
                await _claimRepository.AddOrUpdateAsync(rewardsClaim);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "EarlyStaked HandleEventAsync error.");
            }
        }
    }
}