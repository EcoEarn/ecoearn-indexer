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

public class RewardsEarlyStakedLogEventProcessor : AElfLogEventProcessorBase<EarlyStaked, LogEventInfo>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly ILogger<RewardsEarlyStakedLogEventProcessor> _logger;
    private readonly IAElfIndexerClientEntityRepository<RewardsClaimIndex, LogEventInfo> _claimRepository;
    private readonly IAElfIndexerClientEntityRepository<TokenPoolIndex, LogEventInfo> _tokenPoolRepository;

    public RewardsEarlyStakedLogEventProcessor(ILogger<RewardsEarlyStakedLogEventProcessor> logger,
        IObjectMapper objectMapper, IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<RewardsClaimIndex, LogEventInfo> claimRepository,
        IAElfIndexerClientEntityRepository<TokenPoolIndex, LogEventInfo> tokenPoolRepository) :
        base(logger)
    {
        _logger = logger;
        _contractInfoOptions = contractInfoOptions.Value;
        _objectMapper = objectMapper;
        _claimRepository = claimRepository;
        _tokenPoolRepository = tokenPoolRepository;
    }

    public override string GetContractAddress(string chainId)
    {
        return _contractInfoOptions.ContractInfos.First(c => c.ChainId == chainId).EcoEarnRewardsContractAddress;
    }

    protected override async Task HandleEventAsync(EarlyStaked eventValue, LogEventContext context)
    {
        _logger.Debug("RewardsWithdrawn: {eventValue} context: {context}",
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
                    WithdrawTime = claimInfo.WithdrawnTime == null
                        ? 0
                        : claimInfo.WithdrawnTime.ToDateTime().ToUtcMilliSeconds(),
                    EarlyStakedAmount = claimInfo.EarlyStakedAmount.ToString(),
                    StakeId = claimInfo.StakeId == null ? "" : claimInfo.StakeId.ToHex(),
                    Seed = claimInfo.Seed == null ? "" : claimInfo.Seed.ToHex(),
                };

                var tokenPoolIndex =
                    await _tokenPoolRepository.GetFromBlockStateSetAsync(rewardsClaim.PoolId, context.ChainId);
                rewardsClaim.PoolType = tokenPoolIndex.PoolType;
                _objectMapper.Map(context, rewardsClaim);
                await _claimRepository.AddOrUpdateAsync(rewardsClaim);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "RewardsClaimed HandleEventAsync error.");
            }
        }
    }
}