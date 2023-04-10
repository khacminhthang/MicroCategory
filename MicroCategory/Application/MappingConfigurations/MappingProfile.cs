using AutoMapper;
using MicroCategory.Domain.Dtos;
using MicroCategory.Domain.Models;

namespace MicroCategory.Application.MappingConfigurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CTerm, CTermDto>();
        }
    }
}
