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

            foreach (var liquidityId in eventValue.LiquidityIds.Data)
            {
                var id = IdGenerateHelper.GetId(liquidityId.ToHex());
                var liquidityInfoIndex = await _repository.GetFromBlockStateSetAsync(id, context.ChainId);
                liquidityInfoIndex.StakeId = eventValue.StakeId == null ? "" : eventValue.StakeId.ToHex();
                liquidityInfoIndex.LpStatus = LpStatus.Added;
                
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