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

public class TokenPoolCreatedLogEventProcessor : AElfLogEventProcessorBase<TokensPoolCreated, LogEventInfo>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly ILogger<TokenPoolCreatedLogEventProcessor> _logger;
    private readonly IAElfIndexerClientEntityRepository<TokenPoolIndex, LogEventInfo> _tokenPoolRepository;

    public TokenPoolCreatedLogEventProcessor(ILogger<TokenPoolCreatedLogEventProcessor> logger,
        IObjectMapper objectMapper, IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<TokenPoolIndex, LogEventInfo> tokenPoolRepository) : base(logger)
    {
        _logger = logger;
        _contractInfoOptions = contractInfoOptions.Value;
        _objectMapper = objectMapper;
        _tokenPoolRepository = tokenPoolRepository;
    }

    public override string GetContractAddress(string chainId)
    {
        return _contractInfoOptions.ContractInfos.First(c => c.ChainId == chainId).EcoEarnTokenContractAddress;
    }

    protected override async Task HandleEventAsync(TokensPoolCreated eventValue, LogEventContext context)
    {
        var tokenContractAddress = _contractInfoOptions.ContractInfos.First(c => c.ChainId == context.ChainId)
            .AElfTokenContractAddress;
        try
        {
            _logger.Debug("TokenPoolCreated: {eventValue} context: {context}", JsonConvert.SerializeObject(eventValue),
                JsonConvert.SerializeObject(context));
            var id = IdGenerateHelper.GetId(eventValue.PoolId.ToHex());
            if (await _tokenPoolRepository.GetAsync(id) != null)
            {
                _logger.LogWarning("Token Pool {id} of {DApp} exists", eventValue.DappId.ToHex(),
                    eventValue.DappId.ToHex());
                return;
            }

            var tokenPoolIndex = new TokenPoolIndex
            {
                Id = id,
                DappId = eventValue.DappId.ToHex(),
                PoolId = eventValue.PoolId.ToHex(),
                Amount = eventValue.Amount.ToString(),
                TokenPoolConfig = new TokenPoolConfig()
                {
                    RewardToken = eventValue.Config.RewardToken,
                    StartBlockNumber = eventValue.Config.StartTime == null ? 0 : eventValue.Config.StartTime.ToDateTime().ToUtcMilliSeconds(),
                    EndBlockNumber = eventValue.Config.EndTime == null ? 0 : eventValue.Config.EndTime.ToDateTime().ToUtcMilliSeconds(),
                    RewardPerBlock = eventValue.Config.RewardPerSecond,
                    StakingToken = eventValue.Config.StakingToken,
                    FixedBoostFactor = eventValue.Config.FixedBoostFactor,
                    MinimumAmount = eventValue.Config.MinimumAmount,
                    ReleasePeriod = eventValue.Config.ReleasePeriod,
                    MaximumStakeDuration = eventValue.Config.MaximumStakeDuration,
                    RewardTokenContract = eventValue.Config.RewardTokenContract.ToBase58(),
                    StakeTokenContract = eventValue.Config.StakeTokenContract.ToBase58(),
                    MinimumClaimAmount = eventValue.Config.MinimumClaimAmount,
                    UnlockWindowDuration = eventValue.Config.UnlockWindowDuration,
                },
                CreateTime = context.BlockTime.ToUtcMilliSeconds()
            };
            tokenPoolIndex.PoolType = tokenPoolIndex.TokenPoolConfig.StakeTokenContract == tokenContractAddress
                ? PoolType.Token
                : PoolType.Lp;

            _objectMapper.Map(context, tokenPoolIndex);
            await _tokenPoolRepository.AddOrUpdateAsync(tokenPoolIndex);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "TokenPoolCreated HandleEventAsync error.");
        }
    }
}