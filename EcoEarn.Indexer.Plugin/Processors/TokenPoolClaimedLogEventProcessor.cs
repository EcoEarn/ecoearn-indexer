using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using EcoEarn.Contracts.Tokens;
using EcoEarn.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Orleans.Runtime;
using Volo.Abp.ObjectMapping;

namespace EcoEarn.Indexer.Plugin.Processors;

public class TokenPoolClaimedLogEventProcessor : AElfLogEventProcessorBase<Claimed, LogEventInfo>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly ILogger<TokenPoolClaimedLogEventProcessor> _logger;
    private readonly IAElfIndexerClientEntityRepository<RewardsClaimIndex, LogEventInfo> _repository;
    private readonly IAElfIndexerClientEntityRepository<TokenPoolIndex, LogEventInfo> _tokenPoolRepository;


    public TokenPoolClaimedLogEventProcessor(ILogger<TokenPoolClaimedLogEventProcessor> logger,
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
        return _contractInfoOptions.ContractInfos.First(c => c.ChainId == chainId).EcoEarnTokenContractAddress;
    }

    protected override async Task HandleEventAsync(Claimed eventValue, LogEventContext context)
    {
        try
        {
            _logger.Debug("TokenPoolClaimed: {eventValue} context: {context}", JsonConvert.SerializeObject(eventValue),
                JsonConvert.SerializeObject(context));
            var id = IdGenerateHelper.GetId(eventValue.ClaimInfo.ClaimId.ToHex(), eventValue.ClaimInfo.PoolId.ToHex());
            if (await _repository.GetAsync(id) != null)
            {
                _logger.LogWarning("Claimed {id} of Pool {DApp} exists", eventValue.ClaimInfo.ClaimId.ToHex(),
                    eventValue.ClaimInfo.PoolId.ToHex());
                return;
            }

            var rewardsClaimIndex = new RewardsClaimIndex
            {
                Id = id,
                ClaimId = eventValue.ClaimInfo.ClaimId.ToHex(),
                StakeId = eventValue.ClaimInfo.StakeId == null ? "" : eventValue.ClaimInfo.StakeId.ToHex(),
                PoolId = eventValue.ClaimInfo.PoolId.ToHex(),
                ClaimedAmount = eventValue.ClaimInfo.ClaimedAmount.ToString(),
                ClaimedSymbol = eventValue.ClaimInfo.ClaimedSymbol,
                ClaimedBlockNumber = eventValue.ClaimInfo.ClaimedBlockNumber,
                ClaimedTime = eventValue.ClaimInfo.ClaimedTime == null ? 0 : eventValue.ClaimInfo.ClaimedTime.ToDateTime().ToUtcMilliSeconds(),
                UnlockTime = eventValue.ClaimInfo.UnlockTime == null ? 0 : eventValue.ClaimInfo.UnlockTime.ToDateTime().ToUtcMilliSeconds(),
                WithdrawTime = eventValue.ClaimInfo.WithdrawTime == null ? 0 : eventValue.ClaimInfo.WithdrawTime.ToDateTime().ToUtcMilliSeconds(),
                Account = eventValue.ClaimInfo.Account.ToBase58(),
                EarlyStakeTime = eventValue.ClaimInfo.EarlyStakeTime == null ? 0 : eventValue.ClaimInfo.EarlyStakeTime.ToDateTime().ToUtcMilliSeconds(),
            };

            var tokenPoolIndex =
                await _tokenPoolRepository.GetFromBlockStateSetAsync(rewardsClaimIndex.PoolId, context.ChainId);
            rewardsClaimIndex.PoolType = tokenPoolIndex.PoolType;
            _objectMapper.Map(context, rewardsClaimIndex);
            await _repository.AddOrUpdateAsync(rewardsClaimIndex);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "TokenPoolClaimed HandleEventAsync error.");
        }
    }
}