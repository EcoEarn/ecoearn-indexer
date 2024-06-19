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

public class TokenPoolUnlockedLogEventProcessor : AElfLogEventProcessorBase<Unlocked, LogEventInfo>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly ILogger<TokenPoolUnlockedLogEventProcessor> _logger;
    private readonly IAElfIndexerClientEntityRepository<TokenStakedIndex, LogEventInfo> _repository;

    public TokenPoolUnlockedLogEventProcessor(ILogger<TokenPoolUnlockedLogEventProcessor> logger,
        IObjectMapper objectMapper, IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<TokenStakedIndex, LogEventInfo> repository) :
        base(logger)
    {
        _logger = logger;
        _contractInfoOptions = contractInfoOptions.Value;
        _objectMapper = objectMapper;
        _repository = repository;
    }

    public override string GetContractAddress(string chainId)
    {
        return _contractInfoOptions.ContractInfos.First(c => c.ChainId == chainId).EcoEarnTokenContractAddress;
    }

    protected override async Task HandleEventAsync(Unlocked eventValue, LogEventContext context)
    {
        try
        {
            _logger.Debug("TokenPoolUnlocked: {eventValue} context: {context}", JsonConvert.SerializeObject(eventValue),
                JsonConvert.SerializeObject(context));
            var id = IdGenerateHelper.GetId(eventValue.PoolData.PoolId.ToHex(), eventValue.StakeInfo.StakeId.ToHex());
            var stakedIndex = await _repository.GetFromBlockStateSetAsync(id, context.ChainId);
            _logger.LogDebug("TokenPoolUnlocked Get Staked Info:{info}", JsonConvert.SerializeObject(stakedIndex));
            stakedIndex.LockState = LockState.Unlock;
            _objectMapper.Map(context, stakedIndex);
            await _repository.AddOrUpdateAsync(stakedIndex);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "TokenPoolUnlocked HandleEventAsync error.");
        }
    }
}