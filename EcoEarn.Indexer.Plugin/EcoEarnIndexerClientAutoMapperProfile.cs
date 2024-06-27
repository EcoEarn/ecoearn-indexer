using AElfIndexer.Client.Handlers;
using AutoMapper;
using EcoEarn.Indexer.Plugin.Entities;
using EcoEarn.Indexer.Plugin.GraphQL.Dto;

namespace EcoEarn.Indexer.Plugin;

public class EcoEarnIndexerClientAutoMapperProfile : Profile
{
    public EcoEarnIndexerClientAutoMapperProfile()
    {
        CreateMap<LogEventContext, PointsPoolIndex>();
        CreateMap<LogEventContext, RewardsClaimIndex>();
        CreateMap<LogEventContext, RewardsClaimRecordIndex>();
        CreateMap<LogEventContext, TokenPoolIndex>();
        CreateMap<LogEventContext, TokenStakedIndex>();
        CreateMap<LogEventContext, TokenPoolStakeInfoIndex>();
        CreateMap<LogEventContext, LiquidityInfoIndex>();

        CreateMap<PointsPoolIndex, PointsPoolDto>();
        CreateMap<PointsPoolConfig, PointsPoolConfigDto>();

        CreateMap<TokenPoolIndex, TokenPoolDto>();
        CreateMap<TokenPoolConfig, TokenPoolConfigDto>();
        CreateMap<TokenStakedIndex, StakedInfoDto>();
        CreateMap<RewardsClaimIndex, ClaimInfoDto>();
        CreateMap<TokenPoolStakeInfoIndex, TokenPoolStakeInfoDto>();
        CreateMap<SubStakeInfo, SubStakeInfoDto>();
        CreateMap<LiquidityInfoIndex, LiquidityInfoDto>();
    }
}