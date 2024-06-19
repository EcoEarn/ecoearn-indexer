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

public class TokenPoolClaimedLogEventProcessor : AElfLogEventProcessorBase<Claimed, LogEventInfo>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly ILogger<TokenPoolClaimedLogEventProcessor> _logger;
    private readonly IAElfIndexerClientEntityRepository<RewardsClaimRecordIndex, LogEventInfo> _repository;
    private readonly IAElfIndexerClientEntityRepository<TokenPoolIndex, LogEventInfo> _tokenPoolRepository;


    public TokenPoolClaimedLogEventProcessor(ILogger<TokenPoolClaimedLogEventProcessor> logger,
        IObjectMapper objectMapper, IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<RewardsClaimRecordIndex, LogEventInfo> pointsRewardsClaimRepository,
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

    protected override async Task HandleEventAsync(Claimed eventValue, LogEventContext context)
    {
        try
        {
            _logger.Debug("TokenPoolClaimed: {eventValue} context: {context}", JsonConvert.SerializeObject(eventValue),
                JsonConvert.SerializeObject(context));

            var rewardsClaimRecordIndex = new RewardsClaimRecordIndex
            {
                Id = Guid.NewGuid().ToString(),
                PoolId = eventValue.PoolId.ToHex(),
                Account = eventValue.Account.ToBase58(),
                Amount = eventValue.Amount.ToString(),
                Seed = "",
            };


            var tokenPoolIndex =
                await _tokenPoolRepository.GetFromBlockStateSetAsync(rewardsClaimRecordIndex.PoolId, context.ChainId);
            rewardsClaimRecordIndex.PoolType = tokenPoolIndex.PoolType;
            _objectMapper.Map(context, rewardsClaimRecordIndex);
            await _repository.AddOrUpdateAsync(rewardsClaimRecordIndex);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "TokenPoolClaimed HandleEventAsync error.");
        }
    }
}