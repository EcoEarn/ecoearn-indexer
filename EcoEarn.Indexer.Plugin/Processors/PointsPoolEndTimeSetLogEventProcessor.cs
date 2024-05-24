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

public class PointsPoolEndTimeSetLogEventProcessor : AElfLogEventProcessorBase<PointsPoolEndTimeSet,
    LogEventInfo>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly ILogger<PointsPoolEndTimeSetLogEventProcessor> _logger;
    private readonly IAElfIndexerClientEntityRepository<PointsPoolIndex, LogEventInfo> _pointsPoolRepository;

    public PointsPoolEndTimeSetLogEventProcessor(
        ILogger<PointsPoolEndTimeSetLogEventProcessor> logger,
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

    protected override async Task HandleEventAsync(PointsPoolEndTimeSet eventValue,
        LogEventContext context)
    {
        try
        {
            _logger.Debug("PointsPoolEndTimeSet: {eventValue} context: {context}",
                JsonConvert.SerializeObject(eventValue),
                JsonConvert.SerializeObject(context));
            var id = IdGenerateHelper.GetId(eventValue.PoolId.ToHex());
            var tokenPoolIndex = await _pointsPoolRepository.GetFromBlockStateSetAsync(id, context.ChainId);

            tokenPoolIndex.PointsPoolConfig.EndBlockNumber = eventValue.EndTime.ToDateTime().ToUtcMilliSeconds();
            _objectMapper.Map(context, tokenPoolIndex);
            await _pointsPoolRepository.AddOrUpdateAsync(tokenPoolIndex);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "PointsPoolEndTimeSet HandleEventAsync error.");
        }
    }
}