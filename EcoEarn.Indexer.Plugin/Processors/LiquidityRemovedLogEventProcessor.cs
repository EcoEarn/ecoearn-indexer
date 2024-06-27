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
            _logger.Debug("LiquidityRemoved: {eventValue} context: {context}", JsonConvert.SerializeObject(eventValue),
                JsonConvert.SerializeObject(context));
            var tokenANewAmount = eventValue.TokenAAmount;
            var tokenBNewAmount = eventValue.TokenBAmount;
            var tokenAOldAmount = eventValue.LiquidityInfos.Data.Sum(x => x.TokenAAmount);
            var tokenBOldAmount = eventValue.LiquidityInfos.Data.Sum(x => x.TokenBAmount);
            var tokenALossAmountSum = (tokenANewAmount - tokenAOldAmount).ToString();
            var tokenBLossAmountSum = (tokenBNewAmount - tokenBOldAmount).ToString();

            foreach (var liquidityInfo in eventValue.LiquidityInfos.Data)
            {
                var id = IdGenerateHelper.GetId(liquidityInfo.LiquidityId.ToHex());

                var liquidityInfoIndex = new LiquidityInfoIndex
                {
                    Id = id,
                    Address = liquidityInfo.Account == null ? "" : liquidityInfo.Account.ToBase58(),
                    LiquidityId = liquidityInfo.LiquidityId == null
                        ? ""
                        : liquidityInfo.LiquidityId.ToHex(),
                    StakeId = liquidityInfo.StakeId == null ? "" : liquidityInfo.StakeId.ToHex(),
                    Seed = liquidityInfo.Seed == null ? "" : liquidityInfo.Seed.ToHex(),
                    LpAmount = liquidityInfo.LpAmount,
                    LpSymbol = liquidityInfo.LpSymbol,
                    RewardSymbol = liquidityInfo.RewardSymbol,
                    TokenAAmount = liquidityInfo.TokenAAmount,
                    TokenASymbol = liquidityInfo.TokenASymbol,
                    TokenBAmount = liquidityInfo.TokenBAmount,
                    TokenBSymbol = liquidityInfo.TokenBSymbol,
                    AddedTime = liquidityInfo.AddedTime == null
                        ? 0
                        : liquidityInfo.AddedTime.ToDateTime().ToUtcMilliSeconds(),
                    RemovedTime = liquidityInfo.RemovedTime == null
                        ? 0
                        : liquidityInfo.RemovedTime.ToDateTime().ToUtcMilliSeconds(),
                    DappId = liquidityInfo.DappId == null ? "" : liquidityInfo.DappId.ToHex(),
                    SwapAddress = liquidityInfo.SwapAddress == null
                        ? ""
                        : liquidityInfo.SwapAddress.ToBase58(),
                    TokenAddress = liquidityInfo.TokenAddress == null
                        ? ""
                        : liquidityInfo.TokenAddress.ToBase58(),
                    LpStatus = LpStatus.Removed,
                    TokenALossAmount =
                        (decimal.Parse(liquidityInfo.TokenAAmount.ToString()) /
                            decimal.Parse(tokenAOldAmount.ToString()) * decimal.Parse(tokenALossAmountSum))
                        .ToString(CultureInfo.InvariantCulture),
                    TokenBLossAmount = (decimal.Parse(liquidityInfo.TokenBAmount.ToString()) /
                            decimal.Parse(tokenBOldAmount.ToString()) * decimal.Parse(tokenBLossAmountSum))
                        .ToString(CultureInfo.InvariantCulture),
                };

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