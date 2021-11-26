using AutoMapper;
using ParkyAPI.Model;
using ParkyAPI.Model.Dtos;

namespace ParkyAPI.ParkyMappers
{
    public class ParkyMappings : Profile
    {
        public ParkyMappings()
        {
            CreateMap<NationalPark, NationalParkDto>().ReverseMap();
            CreateMap<Trail, TrailDto>().ReverseMap();
            CreateMap<Trail, TrailUpsertDto>().ReverseMap();
        }
    }
}
