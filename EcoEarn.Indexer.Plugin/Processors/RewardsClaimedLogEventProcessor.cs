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

public class RewardsClaimedLogEventProcessor : AElfLogEventProcessorBase<Claimed, LogEventInfo>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly ILogger<RewardsClaimedLogEventProcessor> _logger;
    private readonly IAElfIndexerClientEntityRepository<RewardsClaimIndex, LogEventInfo> _repository;
    private readonly IAElfIndexerClientEntityRepository<TokenPoolIndex, LogEventInfo> _tokenPoolRepository;

    public RewardsClaimedLogEventProcessor(ILogger<RewardsClaimedLogEventProcessor> logger,
        IObjectMapper objectMapper, IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<RewardsClaimIndex, LogEventInfo> pointsRewardsClaimRepository,
        IAElfIndexerClientEntityRepository<TokenPoolIndex, LogEventInfo> tokenPoolRepository) :
        base(logger)
    {
        _logger = logger;
        _contractInfoOptions = contractInfoOptions.Value;
        _objectMapper = objectMapper;
        _repository = pointsRewardsClaimRepository;
        _tokenPoolRepository = tokenPoolRepository;
    }

    public override string GetContractAddress(string chainId)
    {
        return _contractInfoOptions.ContractInfos.First(c => c.ChainId == chainId).EcoEarnRewardsContractAddress;
    }

    protected override async Task HandleEventAsync(Claimed eventValue, LogEventContext context)
    {
        _logger.Debug("RewardsClaimed: {eventValue} context: {context}",
            JsonConvert.SerializeObject(eventValue), JsonConvert.SerializeObject(context));
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
                };

                var tokenPoolIndex =
                    await _tokenPoolRepository.GetFromBlockStateSetAsync(rewardsClaim.PoolId, context.ChainId);
                rewardsClaim.PoolType = tokenPoolIndex?.PoolType ?? PoolType.Points;

                _objectMapper.Map(context, rewardsClaim);
                await _repository.AddOrUpdateAsync(rewardsClaim);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "RewardsClaimed HandleEventAsync error.");
            }
        }
    }
}