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

public class LiquidityAddedLogEvenProcessor : AElfLogEventProcessorBase<LiquidityAdded, LogEventInfo>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly ILogger<LiquidityAddedLogEvenProcessor> _logger;
    private readonly IAElfIndexerClientEntityRepository<LiquidityInfoIndex, LogEventInfo> _repository;
    private readonly IAElfIndexerClientEntityRepository<RewardsClaimIndex, LogEventInfo> _rewardsClaimRepository;

    public LiquidityAddedLogEvenProcessor(ILogger<LiquidityAddedLogEvenProcessor> logger,
        IObjectMapper objectMapper, IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<LiquidityInfoIndex, LogEventInfo> repository,
        IAElfIndexerClientEntityRepository<RewardsClaimIndex, LogEventInfo> rewardsClaimRepository) :
        base(logger)
    {
        _logger = logger;
        _contractInfoOptions = contractInfoOptions.Value;
        _objectMapper = objectMapper;
        _repository = repository;
        _rewardsClaimRepository = rewardsClaimRepository;
    }

    public override string GetContractAddress(string chainId)
    {
        return _contractInfoOptions.ContractInfos.First(c => c.ChainId == chainId).EcoEarnRewardsContractAddress;
    }

    protected override async Task HandleEventAsync(LiquidityAdded eventValue, LogEventContext context)
    {
        try
        {
            _logger.Debug("LiquidityAdded: {eventValue} context: {context}", JsonConvert.SerializeObject(eventValue),
                JsonConvert.SerializeObject(context));
            var id = IdGenerateHelper.GetId(eventValue.LiquidityInfo.LiquidityId.ToHex());

            var liquidityInfoIndex = new LiquidityInfoIndex
            {
                Id = id,
                Address = eventValue.Account == null ? "" : eventValue.Account.ToBase58(),
                LiquidityId = eventValue.LiquidityInfo.LiquidityId == null
                    ? ""
                    : eventValue.LiquidityInfo.LiquidityId.ToHex(),
                Seed = eventValue.LiquidityInfo.Seed == null ? "" : eventValue.LiquidityInfo.Seed.ToHex(),
                LpAmount = eventValue.LiquidityInfo.LpAmount,
                LpSymbol = eventValue.LiquidityInfo.LpSymbol,
                RewardSymbol = eventValue.LiquidityInfo.RewardSymbol,
                TokenAAmount = eventValue.LiquidityInfo.TokenAAmount,
                TokenASymbol = eventValue.LiquidityInfo.TokenASymbol,
                TokenBAmount = eventValue.LiquidityInfo.TokenBAmount,
                TokenBSymbol = eventValue.LiquidityInfo.TokenBSymbol,
                AddedTime = eventValue.LiquidityInfo.AddedTime == null
                    ? 0
                    : eventValue.LiquidityInfo.AddedTime.ToDateTime().ToUtcMilliSeconds(),
                DappId = eventValue.LiquidityInfo.DappId == null ? "" : eventValue.LiquidityInfo.DappId.ToHex(),
                SwapAddress = eventValue.LiquidityInfo.SwapAddress == null
                    ? ""
                    : eventValue.LiquidityInfo.SwapAddress.ToBase58(),
                TokenAddress = eventValue.LiquidityInfo.TokenAddress == null
                    ? ""
                    : eventValue.LiquidityInfo.TokenAddress.ToBase58(),
                LpStatus = LpStatus.Added,
            };

            _objectMapper.Map(context, liquidityInfoIndex);
            await _repository.AddOrUpdateAsync(liquidityInfoIndex);

            foreach (var claimInfoId in eventValue.ClaimIds.Data)
            {
                var claimId = IdGenerateHelper.GetId(claimInfoId.ToHex());
                var rewardsClaim = await _rewardsClaimRepository.GetFromBlockStateSetAsync(claimId, context.ChainId);
                rewardsClaim.LiquidityAddedSeed = eventValue.LiquidityInfo.Seed == null
                    ? ""
                    : eventValue.LiquidityInfo.Seed.ToHex();
                rewardsClaim.LiquidityId = eventValue.LiquidityInfo.LiquidityId == null
                    ? ""
                    : eventValue.LiquidityInfo.LiquidityId.ToHex();
                _objectMapper.Map(context, rewardsClaim);
                await _rewardsClaimRepository.AddOrUpdateAsync(rewardsClaim);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "LiquidityAdded HandleEventAsync error. {}", e.Message);
        }
    }
}