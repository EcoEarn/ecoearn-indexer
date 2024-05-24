using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using EcoEarn.Contracts.Points;
using EcoEarn.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Orleans.Runtime;
using Volo.Abp.ObjectMapping;

namespace EcoEarn.Indexer.Plugin.Processors;

public class PointsPoolRewardPerSecondSetLogEventProcessor : AElfLogEventProcessorBase<PointsPoolRewardPerSecondSet,
    LogEventInfo>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly ILogger<PointsPoolRewardPerSecondSetLogEventProcessor> _logger;
    private readonly IAElfIndexerClientEntityRepository<PointsPoolIndex, LogEventInfo> _pointsPoolRepository;

    public PointsPoolRewardPerSecondSetLogEventProcessor(
        ILogger<PointsPoolRewardPerSecondSetLogEventProcessor> logger,
        IObjectMapper objectMapper, IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<PointsPoolIndex, LogEventInfo> pointsPoolRepository) :
        base(logger)
    {
        _logger = logger;
        _contractInfoOptions = contractInfoOptions.Value;
        _objectMapper = objectMapper;
        _pointsPoolRepository = pointsPoolRepository;
    }

    public override string GetContractAddress(string chainId)
    {
        return _contractInfoOptions.ContractInfos.First(c => c.ChainId == chainId).EcoEarnPointsContractAddress;
    }

    protected override async Task HandleEventAsync(PointsPoolRewardPerSecondSet eventValue,
        LogEventContext context)
    {
        try
        {
            _logger.Debug("PointsPoolRewardPerSecondSet: {eventValue} context: {context}",
                JsonConvert.SerializeObject(eventValue),
                JsonConvert.SerializeObject(context));
            var id = IdGenerateHelper.GetId(eventValue.PoolId.ToHex());
            var tokenPoolIndex = await _pointsPoolRepository.GetFromBlockStateSetAsync(id, context.ChainId);

            tokenPoolIndex.PointsPoolConfig.RewardPerBlock = eventValue.RewardPerSecond;
            _objectMapper.Map(context, tokenPoolIndex);
            await _pointsPoolRepository.AddOrUpdateAsync(tokenPoolIndex);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "PointsPoolRewardPerSecondSet HandleEventAsync error.");
        }
    }
}