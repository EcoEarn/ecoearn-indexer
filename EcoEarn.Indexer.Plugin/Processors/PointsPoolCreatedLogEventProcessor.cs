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
using PointsPoolConfig = EcoEarn.Indexer.Plugin.Entities.PointsPoolConfig;

namespace EcoEarn.Indexer.Plugin.Processors;

public class PointsPoolCreatedLogEventProcessor : AElfLogEventProcessorBase<PointsPoolCreated, LogEventInfo>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly ILogger<PointsPoolCreatedLogEventProcessor> _logger;
    private readonly IAElfIndexerClientEntityRepository<PointsPoolIndex, LogEventInfo> _pointsPoolRepository;


    public PointsPoolCreatedLogEventProcessor(ILogger<PointsPoolCreatedLogEventProcessor> logger,
        IObjectMapper objectMapper, IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<PointsPoolIndex, LogEventInfo> pointsPoolRepository) : base(logger)
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

    protected override async Task HandleEventAsync(PointsPoolCreated eventValue, LogEventContext context)
    {
        try
        {
            _logger.Debug("PointsPoolCreated: {eventValue} context: {context}", JsonConvert.SerializeObject(eventValue),
                JsonConvert.SerializeObject(context));
            var id = IdGenerateHelper.GetId(eventValue.PoolId.ToHex());
            if (await _pointsPoolRepository.GetAsync(id) != null)
            {
                _logger.LogWarning("Points Pool {id} of {DApp} exists", eventValue.PoolId.ToHex(),
                    eventValue.DappId.ToHex());
                return;
            }

            var user = new PointsPoolIndex
            {
                Id = id,
                DappId = eventValue.DappId.ToHex(),
                PoolId = eventValue.PoolId.ToHex(),
                PointsName = eventValue.PointsName,
                Amount = eventValue.Amount.ToString(),
                PointsPoolConfig = new PointsPoolConfig()
                {
                    RewardToken = eventValue.Config.RewardToken,
                    StartBlockNumber = eventValue.Config.StartTime == null ? 0 : eventValue.Config.StartTime.ToDateTime().ToUtcMilliSeconds(),
                    EndBlockNumber = eventValue.Config.EndTime == null ? 0 : eventValue.Config.EndTime.ToDateTime().ToUtcMilliSeconds(),
                    RewardPerBlock = eventValue.Config.RewardPerSecond,
                    UpdateAddress = eventValue.Config.UpdateAddress.ToBase58(),
                    ReleasePeriod = eventValue.Config.ReleasePeriods.Max(),
                    ReleasePeriods = eventValue.Config.ReleasePeriods.ToList(),
                    ClaimInterval = eventValue.Config.ClaimInterval,
                },
                CreateTime = context.BlockTime.ToUtcMilliSeconds()
            };

            _objectMapper.Map(context, user);
            await _pointsPoolRepository.AddOrUpdateAsync(user);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "PointsPoolCreated HandleEventAsync error.");
        }
    }
}