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

public class TokenPoolEarlyStakedLogEventProcessor : AElfLogEventProcessorBase<EarlyStaked, LogEventInfo>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly ILogger<TokenPoolEarlyStakedLogEventProcessor> _logger;
    private readonly IAElfIndexerClientEntityRepository<TokenStakedIndex, LogEventInfo> _tokenStakeRepository;
    private readonly IAElfIndexerClientEntityRepository<RewardsClaimIndex, LogEventInfo> _claimRepository;
    private readonly IAElfIndexerClientEntityRepository<TokenPoolIndex, LogEventInfo> _tokenPoolRepository;

    private readonly IAElfIndexerClientEntityRepository<TokenPoolStakeInfoIndex, LogEventInfo>
        _tokenPoolStakeRepository;

    public TokenPoolEarlyStakedLogEventProcessor(ILogger<TokenPoolEarlyStakedLogEventProcessor> logger,
        IObjectMapper objectMapper, IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<TokenStakedIndex, LogEventInfo> tokenStakeRepository,
        IAElfIndexerClientEntityRepository<RewardsClaimIndex, LogEventInfo> claimRepository,
        IAElfIndexerClientEntityRepository<TokenPoolIndex, LogEventInfo> tokenPoolRepository,
        IAElfIndexerClientEntityRepository<TokenPoolStakeInfoIndex, LogEventInfo> tokenPoolStakeRepository) :
        base(logger)
    {
        _logger = logger;
        _contractInfoOptions = contractInfoOptions.Value;
        _objectMapper = objectMapper;
        _tokenStakeRepository = tokenStakeRepository;
        _claimRepository = claimRepository;
        _tokenPoolRepository = tokenPoolRepository;
        _tokenPoolStakeRepository = tokenPoolStakeRepository;
    }

    public override string GetContractAddress(string chainId)
    {
        return _contractInfoOptions.ContractInfos.First(c => c.ChainId == chainId).EcoEarnTokenContractAddress;
    }

    protected override async Task HandleEventAsync(EarlyStaked eventValue, LogEventContext context)
    {
        try
        {
            _logger.Debug("TokenEarlyStaked: {eventValue} context: {context}", JsonConvert.SerializeObject(eventValue),
                JsonConvert.SerializeObject(context));
            var id = IdGenerateHelper.GetId(eventValue.StakeInfo.PoolId.ToHex(), eventValue.StakeInfo.StakeId.ToHex());
            if (await _tokenStakeRepository.GetAsync(id) != null)
            {
                _logger.LogWarning("Token Pool {id} of {Staked} exists", eventValue.StakeInfo.PoolId.ToHex(),
                    eventValue.StakeInfo.StakeId.ToHex());
                return;
            }

            var tokenStakedIndex = new TokenStakedIndex
            {
                Id = id,
                StakeId = eventValue.StakeInfo.StakeId.ToHex(),
                PoolId = eventValue.StakeInfo.PoolId.ToHex(),
                StakingToken = eventValue.StakeInfo.StakingToken,
                StakedAmount = eventValue.StakeInfo.StakedAmount,
                EarlyStakedAmount = eventValue.StakeInfo.EarlyStakedAmount,
                ClaimedAmount = eventValue.StakeInfo.ClaimedAmount,
                StakedBlockNumber = eventValue.StakeInfo.StakedBlockNumber,
                StakedTime = eventValue.StakeInfo.StakedTime == null
                    ? 0
                    : eventValue.StakeInfo.StakedTime.ToDateTime().ToUtcMilliSeconds(),
                Period = eventValue.StakeInfo.Period,
                Account = eventValue.StakeInfo.Account.ToBase58(),
                BoostedAmount = eventValue.StakeInfo.BoostedAmount,
                RewardDebt = eventValue.StakeInfo.RewardDebt,
                WithdrawTime = eventValue.StakeInfo.UnlockTime == null
                    ? 0
                    : eventValue.StakeInfo.UnlockTime.ToDateTime().ToUtcMilliSeconds(),
                RewardAmount = eventValue.StakeInfo.RewardAmount,
                LockedRewardAmount = eventValue.StakeInfo.LockedRewardAmount,
                LastOperationTime = eventValue.StakeInfo.LastOperationTime == null
                    ? 0
                    : eventValue.StakeInfo.LastOperationTime.ToDateTime().ToUtcMilliSeconds(),
                UpdateTime = context.BlockTime.ToUtcMilliSeconds(),
                LockState = LockState.Locking,
                StakingPeriod = eventValue.StakeInfo.StakingPeriod
            };

            var tokenPool =
                await _tokenPoolRepository.GetFromBlockStateSetAsync(tokenStakedIndex.PoolId, context.ChainId);
            tokenStakedIndex.PoolType = tokenPool.PoolType;
            _objectMapper.Map(context, tokenStakedIndex);
            await _tokenStakeRepository.AddOrUpdateAsync(tokenStakedIndex);

            foreach (var claimInfo in eventValue.ClaimInfos.Data)
            {
                try
                {
                    var claimId = IdGenerateHelper.GetId(claimInfo.ClaimId.ToHex(),
                        claimInfo.PoolId.ToHex());

                    var rewardsClaim = new RewardsClaimIndex
                    {
                        Id = claimId,
                        ClaimId = claimInfo.ClaimId == null ? "" : claimInfo.ClaimId.ToHex(),
                        StakeId = claimInfo.StakeId == null ? "" : claimInfo.StakeId.ToHex(),
                        PoolId = claimInfo.PoolId == null ? "" : claimInfo.PoolId.ToHex(),
                        ClaimedAmount = claimInfo.ClaimedAmount.ToString(),
                        ClaimedSymbol = claimInfo.ClaimedSymbol,
                        ClaimedBlockNumber = claimInfo.ClaimedBlockNumber,
                        ClaimedTime = claimInfo.ClaimedTime == null
                            ? 0
                            : claimInfo.ClaimedTime.ToDateTime().ToUtcMilliSeconds(),
                        UnlockTime = claimInfo.UnlockTime == null
                            ? 0
                            : claimInfo.UnlockTime.ToDateTime().ToUtcMilliSeconds(),
                        WithdrawTime = claimInfo.WithdrawTime == null
                            ? 0
                            : claimInfo.WithdrawTime.ToDateTime().ToUtcMilliSeconds(),
                        Account = claimInfo.Account.ToBase58(),
                        EarlyStakeTime = claimInfo.EarlyStakeTime == null
                            ? 0
                            : claimInfo.EarlyStakeTime.ToDateTime().ToUtcMilliSeconds(),
                    };

                    var tokenPoolIndex =
                        await _tokenPoolRepository.GetFromBlockStateSetAsync(rewardsClaim.PoolId, context.ChainId);
                    rewardsClaim.PoolType = tokenPoolIndex.PoolType;
                    _objectMapper.Map(context, rewardsClaim);
                    await _claimRepository.AddOrUpdateAsync(rewardsClaim);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "TokenEarlyStaked Claimed error.");
                }
            }

            var tokenPoolStakeInfoIndex = new TokenPoolStakeInfoIndex()
            {
                Id = eventValue.StakeInfo.PoolId.ToHex(),
                PoolId = eventValue.StakeInfo.PoolId == null ? "" : eventValue.StakeInfo.PoolId.ToHex(),
                AccTokenPerShare = eventValue.PoolData.AccTokenPerShare == null
                    ? "0"
                    : eventValue.PoolData.AccTokenPerShare.Value,
                TotalStakedAmount = eventValue.PoolData.TotalStakedAmount.ToString(),
                LastRewardTime = eventValue.PoolData.LastRewardTime == null
                    ? 0
                    : eventValue.PoolData.LastRewardTime.ToDateTime().ToUtcMilliSeconds(),
            };
            _objectMapper.Map(context, tokenPoolStakeInfoIndex);
            await _tokenPoolStakeRepository.AddOrUpdateAsync(tokenPoolStakeInfoIndex);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "TokenEarlyStaked HandleEventAsync error.");
        }
    }
}