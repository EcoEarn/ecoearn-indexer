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

public class TokenPoolStakedLogEventProcessor : AElfLogEventProcessorBase<Staked, LogEventInfo>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly ILogger<TokenPoolStakedLogEventProcessor> _logger;
    private readonly IAElfIndexerClientEntityRepository<TokenStakedIndex, LogEventInfo> _tokenStakeRepository;
    private readonly IAElfIndexerClientEntityRepository<TokenPoolIndex, LogEventInfo> _tokenPoolRepository;

    private readonly IAElfIndexerClientEntityRepository<TokenPoolStakeInfoIndex, LogEventInfo>
        _tokenPoolStakeRepository;

    public TokenPoolStakedLogEventProcessor(ILogger<TokenPoolStakedLogEventProcessor> logger,
        IObjectMapper objectMapper, IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<TokenStakedIndex, LogEventInfo> tokenStakeRepository,
        IAElfIndexerClientEntityRepository<TokenPoolIndex, LogEventInfo> tokenPoolRepository,
        IAElfIndexerClientEntityRepository<TokenPoolStakeInfoIndex, LogEventInfo> tokenPoolStakeRepository) :
        base(logger)
    {
        _logger = logger;
        _contractInfoOptions = contractInfoOptions.Value;
        _objectMapper = objectMapper;
        _tokenStakeRepository = tokenStakeRepository;
        _tokenPoolRepository = tokenPoolRepository;
        _tokenPoolStakeRepository = tokenPoolStakeRepository;
    }

    public override string GetContractAddress(string chainId)
    {
        return _contractInfoOptions.ContractInfos.First(c => c.ChainId == chainId).EcoEarnTokenContractAddress;
    }

    protected override async Task HandleEventAsync(Staked eventValue, LogEventContext context)
    {
        try
        {
            _logger.Debug("TokenStaked: {eventValue} context: {context}", JsonConvert.SerializeObject(eventValue),
                JsonConvert.SerializeObject(context));
            var id = IdGenerateHelper.GetId(eventValue.StakeInfo.PoolId.ToHex(), eventValue.StakeInfo.StakeId.ToHex());
            var poolId = IdGenerateHelper.GetId(eventValue.StakeInfo.PoolId.ToHex());

            var tokenStakedIndex = new TokenStakedIndex
            {
                Id = id,
                StakeId = eventValue.StakeInfo.StakeId == null ? "" : eventValue.StakeInfo.StakeId.ToHex(),
                PoolId = eventValue.StakeInfo.PoolId == null ? "" : eventValue.StakeInfo.PoolId.ToHex(),
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
                LockState = LockState.Locking
            };
            var tokenPoolIndex =
                await _tokenPoolRepository.GetFromBlockStateSetAsync(tokenStakedIndex.PoolId, context.ChainId);
            tokenStakedIndex.PoolType = tokenPoolIndex.PoolType;
            _objectMapper.Map(context, tokenStakedIndex);
            await _tokenStakeRepository.AddOrUpdateAsync(tokenStakedIndex);

            var tokenPoolStakeInfoIndex = new TokenPoolStakeInfoIndex()
            {
                Id = poolId,
                PoolId = eventValue.StakeInfo.PoolId == null ? "" : eventValue.StakeInfo.PoolId.ToHex(),
                AccTokenPerShare = eventValue.PoolData.AccTokenPerShare == null
                    ? "0"
                    : eventValue.PoolData.AccTokenPerShare.Value,
                TotalStakedAmount = eventValue.PoolData.TotalStakedAmount.ToString(),
                LastRewardTime = eventValue.PoolData.LastRewardSecond == null
                    ? 0
                    : eventValue.PoolData.LastRewardSecond.ToDateTime().ToUtcMilliSeconds(),
            };
            _objectMapper.Map(context, tokenPoolStakeInfoIndex);
            await _tokenPoolStakeRepository.AddOrUpdateAsync(tokenPoolStakeInfoIndex);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "TokenStaked HandleEventAsync error.");
        }
    }
}