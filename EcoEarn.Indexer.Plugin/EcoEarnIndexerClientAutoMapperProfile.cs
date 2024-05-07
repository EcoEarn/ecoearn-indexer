using AutoMapper;
using EcoEarn.Indexer.Plugin.Entities;
using EcoEarn.Indexer.Plugin.GraphQL.Dto;

namespace EcoEarn.Indexer.Plugin;

public class EcoEarnIndexerClientAutoMapperProfile : Profile
{
    public EcoEarnIndexerClientAutoMapperProfile()
    {
        CreateMap<PointsPoolIndex, PointsPoolDto>();
        CreateMap<PointsPoolConfig, PointsPoolConfigDto>();
    }
}