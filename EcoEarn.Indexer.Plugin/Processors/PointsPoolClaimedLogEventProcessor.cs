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

public class PointsPoolClaimedLogEventProcessor : AElfLogEventProcessorBase<Claimed, LogEventInfo>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly ILogger<PointsPoolClaimedLogEventProcessor> _logger;
    private readonly IAElfIndexerClientEntityRepository<RewardsClaimRecordIndex, LogEventInfo> _repository;
    private readonly IAElfIndexerClientEntityRepository<PointsPoolIndex, LogEventInfo> _pointsPoolRepository;


    public PointsPoolClaimedLogEventProcessor(ILogger<PointsPoolClaimedLogEventProcessor> logger,
        IObjectMapper objectMapper, IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<RewardsClaimRecordIndex, LogEventInfo> pointsRewardsClaimRepository,
        IAElfIndexerClientEntityRepository<PointsPoolIndex, LogEventInfo> pointsPoolRepository) :
        base(logger)
    {
        _logger = logger;
        _contractInfoOptions = contractInfoOptions.Value;
        _objectMapper = objectMapper;
        _repository = pointsRewardsClaimRepository;
        _pointsPoolRepository = pointsPoolRepository;
    }

    public override string GetContractAddress(string chainId)
    {
        return _contractInfoOptions.ContractInfos.First(c => c.ChainId == chainId).EcoEarnPointsContractAddress;
    }

    protected override async Task HandleEventAsync(Claimed eventValue, LogEventContext context)
    {
        try
        {
            _logger.Debug("PointsPoolClaimed: {eventValue} context: {context}", JsonConvert.SerializeObject(eventValue),
                JsonConvert.SerializeObject(context));
            var rewardsClaimRecordIndex = new RewardsClaimRecordIndex
            {
                Id = Guid.NewGuid().ToString(),
                PoolId = eventValue.PoolId.ToHex(),
                Account = eventValue.Account.ToBase58(),
                Amount = eventValue.Amount.ToString(),
                Seed = eventValue.Seed == null ? "" : eventValue.Seed.ToHex(),
                PoolType = PoolType.Points,
            };
            
            _objectMapper.Map(context, rewardsClaimRecordIndex);
            await _repository.AddOrUpdateAsync(rewardsClaimRecordIndex);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "PointsPoolClaimed HandleEventAsync error.");
        }
    }
}