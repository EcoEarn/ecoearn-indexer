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

public class
    TokensPoolUnlockWindowDurationSetLogEventProcessor : AElfLogEventProcessorBase<TokensPoolUnlockWindowDurationSet,
    LogEventInfo>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly ILogger<TokensPoolUnlockWindowDurationSetLogEventProcessor> _logger;
    private readonly IAElfIndexerClientEntityRepository<RewardsClaimIndex, LogEventInfo> _repository;
    private readonly IAElfIndexerClientEntityRepository<TokenPoolIndex, LogEventInfo> _tokenPoolRepository;

    public TokensPoolUnlockWindowDurationSetLogEventProcessor(
        ILogger<TokensPoolUnlockWindowDurationSetLogEventProcessor> logger,
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

    protected override async Task HandleEventAsync(TokensPoolUnlockWindowDurationSet eventValue,
        LogEventContext context)
    {
        try
        {
            _logger.Debug("TokensPoolUnlockWindowDurationSet: {eventValue} context: {context}",
                JsonConvert.SerializeObject(eventValue),
                JsonConvert.SerializeObject(context));
            var id = IdGenerateHelper.GetId(eventValue.PoolId.ToHex());
            var tokenPoolIndex = await _tokenPoolRepository.GetFromBlockStateSetAsync(id, context.ChainId);

            tokenPoolIndex.TokenPoolConfig.UnlockWindowDuration = eventValue.UnlockWindowDuration;
            _objectMapper.Map(context, tokenPoolIndex);
            await _tokenPoolRepository.AddOrUpdateAsync(tokenPoolIndex);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "TokensPoolUnlockWindowDurationSet HandleEventAsync error.");
        }
    }
}