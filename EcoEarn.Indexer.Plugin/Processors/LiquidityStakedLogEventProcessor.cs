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

public class LiquidityStakedLogEventProcessor : AElfLogEventProcessorBase<LiquidityStaked, LogEventInfo>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly ILogger<LiquidityStakedLogEventProcessor> _logger;
    private readonly IAElfIndexerClientEntityRepository<LiquidityInfoIndex, LogEventInfo> _repository;
    private readonly IAElfIndexerClientEntityRepository<RewardsClaimIndex, LogEventInfo> _rewardsClaimRepository;
    private readonly IAElfIndexerClientEntityRepository<TokenPoolIndex, LogEventInfo> _tokenPoolRepository;

    public LiquidityStakedLogEventProcessor(ILogger<LiquidityStakedLogEventProcessor> logger,
        IObjectMapper objectMapper, IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<LiquidityInfoIndex, LogEventInfo> repository,
        IAElfIndexerClientEntityRepository<RewardsClaimIndex, LogEventInfo> rewardsClaimRepository,
        IAElfIndexerClientEntityRepository<TokenPoolIndex, LogEventInfo> tokenPoolRepository) :
        base(logger)
    {
        _logger = logger;
        _contractInfoOptions = contractInfoOptions.Value;
        _objectMapper = objectMapper;
        _repository = repository;
        _rewardsClaimRepository = rewardsClaimRepository;
        _tokenPoolRepository = tokenPoolRepository;
    }

    public override string GetContractAddress(string chainId)
    {
        return _contractInfoOptions.ContractInfos.First(c => c.ChainId == chainId).EcoEarnRewardsContractAddress;
    }

    protected override async Task HandleEventAsync(LiquidityStaked eventValue, LogEventContext context)
    {
        try
        {
            _logger.Debug("LiquidityStaked: {eventValue} context: {context}", JsonConvert.SerializeObject(eventValue),
                JsonConvert.SerializeObject(context));
            
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
                    LpStatus = LpStatus.Added,
                };

                _objectMapper.Map(context, liquidityInfoIndex);
                await _repository.AddOrUpdateAsync(liquidityInfoIndex);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "LiquidityStaked HandleEventAsync error.");
        }
    }
}