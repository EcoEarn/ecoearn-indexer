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

public class TokensPoolStakeConfigSetLogEventProcessor : AElfLogEventProcessorBase<TokensPoolStakeConfigSet,
    LogEventInfo>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly ILogger<TokensPoolStakeConfigSetLogEventProcessor> _logger;
    private readonly IAElfIndexerClientEntityRepository<RewardsClaimIndex, LogEventInfo> _repository;
    private readonly IAElfIndexerClientEntityRepository<TokenPoolIndex, LogEventInfo> _tokenPoolRepository;

    public TokensPoolStakeConfigSetLogEventProcessor(
        ILogger<TokensPoolStakeConfigSetLogEventProcessor> logger,
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

    protected override async Task HandleEventAsync(TokensPoolStakeConfigSet eventValue,
        LogEventContext context)
    {
        try
        {
            _logger.Debug("TokensPoolStakeConfigSet: {eventValue} context: {context}",
                JsonConvert.SerializeObject(eventValue),
                JsonConvert.SerializeObject(context));
            var id = IdGenerateHelper.GetId(eventValue.PoolId.ToHex());
            var tokenPoolIndex = await _tokenPoolRepository.GetFromBlockStateSetAsync(id, context.ChainId);

            tokenPoolIndex.TokenPoolConfig.MinimumAmount = eventValue.MinimumAmount;
            tokenPoolIndex.TokenPoolConfig.MinimumClaimAmount = eventValue.MinimumClaimAmount;
            tokenPoolIndex.TokenPoolConfig.MinimumStakeDuration = eventValue.MinimumStakeDuration;
            tokenPoolIndex.TokenPoolConfig.MaximumStakeDuration = eventValue.MaximumStakeDuration;
            tokenPoolIndex.TokenPoolConfig.MinimumAddLiquidityAmount = eventValue.MinimumAddLiquidityAmount;
            _objectMapper.Map(context, tokenPoolIndex);
            await _tokenPoolRepository.AddOrUpdateAsync(tokenPoolIndex);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "TokensPoolStakeConfigSet HandleEventAsync error.");
        }
    }
}