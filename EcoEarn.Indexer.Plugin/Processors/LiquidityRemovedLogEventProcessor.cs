using System.Globalization;
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

public class LiquidityRemovedLogEventProcessor : AElfLogEventProcessorBase<LiquidityRemoved, LogEventInfo>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly ILogger<LiquidityRemovedLogEventProcessor> _logger;
    private readonly IAElfIndexerClientEntityRepository<LiquidityInfoIndex, LogEventInfo> _repository;

    public LiquidityRemovedLogEventProcessor(ILogger<LiquidityRemovedLogEventProcessor> logger,
        IObjectMapper objectMapper, IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<LiquidityInfoIndex, LogEventInfo> repository) :
        base(logger)
    {
        _logger = logger;
        _contractInfoOptions = contractInfoOptions.Value;
        _objectMapper = objectMapper;
        _repository = repository;
    }

    public override string GetContractAddress(string chainId)
    {
        return _contractInfoOptions.ContractInfos.First(c => c.ChainId == chainId).EcoEarnRewardsContractAddress;
    }

    protected override async Task HandleEventAsync(LiquidityRemoved eventValue, LogEventContext context)
    {
        try
        {
            var tokenAOldAmount = 0L;
            var tokenBOldAmount = 0L;
            foreach (var liquidityId in eventValue.LiquidityIds.Data)
            {
                var id = IdGenerateHelper.GetId(liquidityId.ToHex());
                var liquidityInfoIndex = await _repository.GetFromBlockStateSetAsync(id, context.ChainId);
                tokenAOldAmount += liquidityInfoIndex.TokenAAmount;
                tokenBOldAmount += liquidityInfoIndex.TokenBAmount;
            }

            _logger.Debug("LiquidityRemoved: {eventValue} context: {context}", JsonConvert.SerializeObject(eventValue),
                JsonConvert.SerializeObject(context));
            var tokenANewAmount = eventValue.TokenAAmount;
            var tokenBNewAmount = eventValue.TokenBAmount;
            var tokenALossAmountSum = tokenANewAmount - tokenAOldAmount;
            var tokenBLossAmountSum = tokenBNewAmount - tokenBOldAmount;

            foreach (var liquidityId in eventValue.LiquidityIds.Data)
            {
                var id = IdGenerateHelper.GetId(liquidityId.ToHex());
                var liquidityInfoIndex = await _repository.GetFromBlockStateSetAsync(id, context.ChainId);
                liquidityInfoIndex.LpStatus = LpStatus.Removed;

                var tokenALossAmount =
                    -((decimal)liquidityInfoIndex.TokenAAmount / tokenAOldAmount) * tokenALossAmountSum;
                var tokenBLossAmount =
                    -((decimal)liquidityInfoIndex.TokenBAmount / tokenBOldAmount) * tokenBLossAmountSum;

                liquidityInfoIndex.TokenALossAmount = Math.Ceiling(tokenALossAmount).ToString(CultureInfo.InvariantCulture);
                liquidityInfoIndex.TokenBLossAmount = Math.Ceiling(tokenBLossAmount).ToString(CultureInfo.InvariantCulture);

                _objectMapper.Map(context, liquidityInfoIndex);
                await _repository.AddOrUpdateAsync(liquidityInfoIndex);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "LiquidityRemoved HandleEventAsync error.");
        }
    }
}